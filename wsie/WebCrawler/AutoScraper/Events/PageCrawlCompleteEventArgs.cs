using System;

namespace AutoScraper.Events {
    public class PageCrawlCompleteEventArgs : EventArgs {
        public Uri Url { get; set; }
        public string Html { get; set; }
    }
}