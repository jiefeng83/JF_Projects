using System;
using System.Net;

namespace AutoScraper.Events {
    public class PageCrawlErrorEventArgs : EventArgs {
        public Uri Url { get; set; }
        public WebException Exception { get; set; }
    }
}