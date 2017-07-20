using System;
using System.Collections.Generic;
using AutoScraper.Core;
using AutoScraper.Events;
using AutoScraper.Interfaces;
using CsQuery;

namespace AutoScraper {
    public class Scraper<TScheduler, TWebClient, TModel> : Crawler<TScheduler, TWebClient> 
        where TScheduler: IScheduler, new() 
        where TWebClient : IWebClient, new() 
        where TModel : new() {

        #region Private Fields
        private readonly object _eventLock = new object();
        private readonly CsQueryMapper<TModel> _csQueryMapper = new CsQueryMapper<TModel>();
        #endregion

        #region Events
        public delegate void PageScrapeCompleteHandler(Crawler<TScheduler, TWebClient> sender, PageScrapeCompleteEventArgs<TModel> e);
        public event PageScrapeCompleteHandler OnPageScrapeComplete; 
        #endregion

        public Dictionary<string, Func<CQ, string>> CustomMappings {
            get { return _csQueryMapper.CustomMappings; }
            set { _csQueryMapper.CustomMappings = value; }
        }

        public Scraper(string url) : base(url) {
            base.OnPageCrawlComplete += Scraper_OnPageCrawlComplete;
        }

        public Scraper(string url, string baseUrl) : base(url, baseUrl) {
            base.OnPageCrawlComplete += Scraper_OnPageCrawlComplete;
        }

        private void Scraper_OnPageCrawlComplete(Crawler<TScheduler, TWebClient> sender, PageCrawlCompleteEventArgs e) {
            TModel model;
            if (_csQueryMapper.TryMap(e.Html, out model)) {
                RaiseOnPageScrapeComplete(new PageScrapeCompleteEventArgs<TModel> {
                    Url = e.Url,
                    Model = model
                });
            }
        }

        #region Event Invocation
        protected virtual void RaiseOnPageScrapeComplete(PageScrapeCompleteEventArgs<TModel> e) {
            lock (_eventLock) {
                OnPageScrapeComplete?.Invoke(this, e);
            }
        }
        #endregion
    }
}