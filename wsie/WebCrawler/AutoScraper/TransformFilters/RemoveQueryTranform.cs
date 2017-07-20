using System;
using AutoScraper.Interfaces;

namespace AutoScraper.TransformFilters {
    public class RemoveQueryTranform : ITranformFilter {
        public Uri Tranform(Uri url) {
            var indexOfQuery = url.AbsoluteUri.IndexOf("?", StringComparison.Ordinal);
            return indexOfQuery >= 0 ? new Uri(url.AbsoluteUri.Substring(0, indexOfQuery)) : url;
        }
    }
}
