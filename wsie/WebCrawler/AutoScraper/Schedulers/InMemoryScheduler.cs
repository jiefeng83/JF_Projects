using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using AutoScraper.Helpers;
using AutoScraper.Interfaces;

namespace AutoScraper.Schedulers {
    public class InMemoryScheduler : IScheduler {
        private ConcurrentQueue<Uri> _urlsScheduled;
        private ConcurrentDictionary<Uri, int> _urlsCrawled;

        public int PagesScheduled => _urlsScheduled.Count;
        public int PagesCrawled => _urlsCrawled.Count;

        public InMemoryScheduler() {
            _urlsScheduled = new ConcurrentQueue<Uri>();
            _urlsCrawled = new ConcurrentDictionary<Uri, int>();
        }

        public bool TryGetNext(out Uri uri, TimeSpan timeout) {
            var expireDate = DateTime.Now.Add(timeout);

            while (expireDate > DateTime.Now) {
                if (_urlsScheduled.TryDequeue(out uri)) {
                    _urlsCrawled.AddOrUpdate(uri, 0, (u, i) => i + 1);
                    return true;
                }
                Thread.Sleep(100);
            }

            uri = null;
            return false;
        }

        public void ReScheduleUri(Uri uri, int maxRetries = 3) {
            var retryCount = _urlsCrawled.AddOrUpdate(uri, 0, (u, i) => i + 1);
            if (retryCount <= maxRetries && !_urlsScheduled.Contains(uri)) {
                _urlsScheduled.Enqueue(uri);
            }
        }

        public void ScheduleUri(Uri uri) {
            if (!_urlsCrawled.ContainsKey(uri) && !_urlsScheduled.Contains(uri)) {
                _urlsScheduled.Enqueue(uri);
            }
        }

        public void Save(string filename) {
            var schedulerState = new SchedulerState {
                UrlsCrawled = _urlsCrawled,
                UrlsScheduled = _urlsScheduled
            };

            SerializationHelper.WriteToBinaryFile(filename, schedulerState);
        }

        public void Restore(string filename) {
            var schedulerState = SerializationHelper.ReadFromBinaryFile<SchedulerState>(filename);
            _urlsCrawled = schedulerState.UrlsCrawled;
            _urlsScheduled = schedulerState.UrlsScheduled;
        }
    }

    [Serializable]
    internal class SchedulerState {
        public ConcurrentQueue<Uri> UrlsScheduled;
        public ConcurrentDictionary<Uri, int> UrlsCrawled;
    }
}