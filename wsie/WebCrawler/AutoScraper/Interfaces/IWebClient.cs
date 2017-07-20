using System;

namespace AutoScraper.Interfaces {
    public interface IWebClient: IDisposable {
        string DownloadString(Uri address);
    }
}