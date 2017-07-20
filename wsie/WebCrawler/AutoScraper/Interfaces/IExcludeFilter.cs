using System;

namespace AutoScraper.Interfaces
{
    public interface IExcludeFilter {
        bool IsMatch(Uri url);
    }
}