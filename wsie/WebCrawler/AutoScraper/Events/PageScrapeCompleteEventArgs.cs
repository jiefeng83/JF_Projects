using System;

namespace AutoScraper.Events {
    public class PageScrapeCompleteEventArgs<TModel> : EventArgs {
        public Uri Url { get; set; }
        public TModel Model { get; set; }
    }
}