using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using AutoScraper.Events;
using AutoScraper.ExcludeFilters;
using AutoScraper.Interfaces;
using AutoScraper.TransformFilters;

namespace AutoScraper {
    public class Crawler<TScheduler, TWebClient> : IDisposable 
        where TScheduler: IScheduler, new() 
        where TWebClient : IWebClient, new() {

        #region Private Fields

        private readonly object _eventLock = new object();

        private const string Pattern = "href\\s*=\\s*(?:[\"'](?<1>[^\"']*)[\"']|(?<1>\\S+))";
        private readonly Uri _uri;
        private readonly string _baseUrl;

        private bool _closingThreads;
        private List<Thread> _threads;

        #endregion

        #region Public Properties

        public List<IExcludeFilter> ExcludeFilters = new List<IExcludeFilter> {
            new ExcludeFilesFilter()
        };

        public List<ITranformFilter> TransformFilters = new List<ITranformFilter> {
            new RemoveAnchorTranform()
        };

        public TScheduler Scheduler { get; private set; }
        public int CrawlDelayInSeconds { get; set; } = 0;

        #endregion

        #region Events

        public delegate void PageCrawlCompleteHandler(Crawler<TScheduler, TWebClient> sender, PageCrawlCompleteEventArgs e);

        public delegate void PageCrawlErrorHandler(Crawler<TScheduler, TWebClient> sender, PageCrawlErrorEventArgs e);

        public event PageCrawlCompleteHandler OnPageCrawlComplete;
        public event PageCrawlErrorHandler OnPageCrawlError;

        #endregion

        public Crawler(string url) {
            _uri = new Uri(url);
            _baseUrl = _uri.GetLeftPart(UriPartial.Authority);
        }

        public Crawler(string url, string baseUrl) {
            _uri = new Uri(url);
            _baseUrl = baseUrl;
        }

        //public void Run(int threads = 5, bool async = false) {
        public void Run(int threads = 10, bool async = false) {
            Initialize(threads);

            Scheduler.ScheduleUri(_uri);

            /* Start all threads */
            foreach (var thread in _threads) {
                thread.Start();
            }

            if (!async) {
                WaitForStoppingThreads();
            }
        }

        public void Stop() {
            _closingThreads = true;

            WaitForStoppingThreads();
        }

        private void Initialize(int threads) {
            Scheduler = (TScheduler)Activator.CreateInstance(typeof(TScheduler));
            _threads = new List<Thread>();
            _closingThreads = false;

            /* Create requested threads */
            for (var i = 0; i < threads; i++) {
                _threads.Add(new Thread(Worker) {
                    IsBackground = true,
                });
            }
        }

        private void Worker() {
            using (var webclient = (TWebClient) Activator.CreateInstance(typeof(TWebClient))) {
                Uri uri;
                while (Scheduler.TryGetNext(out uri, TimeSpan.FromSeconds(5)) && !_closingThreads) {
                    ProcessNextUrl(webclient, uri);
                    Thread.Sleep(CrawlDelayInSeconds*1000);
                }
            }
        }

        private void ProcessNextUrl(TWebClient webclient, Uri uri) {
            string html;
            WebException exception;
            if (TryCrawlUrl(webclient, uri, out html, out exception)) {
                RaiseOnPageCrawlComplete(new PageCrawlCompleteEventArgs {
                    Url = uri,
                    Html = html
                });
            }
            else {
                Scheduler.ReScheduleUri(uri);

                RaiseOnPageCrawlError(new PageCrawlErrorEventArgs {
                    Url = uri,
                    Exception = exception
                });
            }
        }

        private void WaitForStoppingThreads() {
            while (_threads.Any(t => t.IsAlive)) {
                Thread.Sleep(100);
            }
        }

        private bool TryCrawlUrl(TWebClient webclient, Uri uri, out string html, out WebException exception) {
            try {
                html = DownloadContentFromUrl(webclient, uri);
                exception = null;

                ScheduleUrlsFromHtml(html);

                return true;
            }
            catch (WebException ex) {
                html = null;
                exception = ex;
                return false;
            }
        }

        private static string DownloadContentFromUrl(TWebClient webClient, Uri uri) {
            try
            {
                return webClient.DownloadString(uri);
            }
            catch (Exception e)
            {
                WebException ex = e as WebException;
                Console.WriteLine("Error: " + e.Message);
                if (ex != null && !ex.Message.Contains("404") && !ex.Message.Contains("405") && !ex.Message.Contains("503"))
                {
                    //Console.WriteLine($"*** {DateTime.Now.ToShortTimeString()} Wait 5 minutes ***");
                    //Thread.Sleep(300000);
                }
                return "";
            }
        }

        private void ScheduleUrlsFromHtml(string html) {
            foreach (var url in FindAllLinks(html)) {
                if (url.StartsWith("/")) {
                    ScheduleUrl(UrlCombine(_baseUrl, url));
                }
                else if (url.StartsWith(_baseUrl)) {
                    ScheduleUrl(url);
                }
            }
        }

        private void ScheduleUrl(string url) {
            var uri = new Uri(url);

            foreach (var transformFilter in TransformFilters) {
                uri = transformFilter.Tranform(uri);
            }

            if (ExcludeFilters.Any(excludeFilter => excludeFilter.IsMatch(uri))) {
                return;
            }

            Scheduler.ScheduleUri(uri);
        }

        private static IEnumerable<string> FindAllLinks(string html) {
            var match = Regex.Match(html, Pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);

            while (match.Success) {
                yield return match.Groups[1].Value;
                match = match.NextMatch();
            }
        }

        private static string UrlCombine(string url1, string url2) {
            if (String.IsNullOrWhiteSpace(url1)) {
                throw new ArgumentNullException(url1);
            }

            if (String.IsNullOrWhiteSpace(url2)) {
                throw new ArgumentNullException(url2);
            }

            url1 = url1.TrimEnd('/');
            url2 = url2.TrimStart('/');

            return $"{url1}/{url2}";
        }

        #region Event Invocation

        protected virtual void RaiseOnPageCrawlComplete(PageCrawlCompleteEventArgs e) {
            lock (_eventLock) {
                OnPageCrawlComplete?.Invoke(this, e);
            }
        }

        protected virtual void RaiseOnPageCrawlError(PageCrawlErrorEventArgs e) {
            lock (_eventLock) {
                OnPageCrawlError?.Invoke(this, e);
            }
        }

        #endregion

        public void Dispose() {}
    }
}