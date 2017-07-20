using System;
using AutoScraper.Interfaces;

namespace AutoScraper.ExcludeFilters {
    public class ExcludeAnchors : IExcludeFilter {
        public bool IsMatch(Uri url) {
            return url.AbsoluteUri.Contains("#");
        }
    }
}