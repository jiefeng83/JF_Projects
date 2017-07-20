using System;
using AutoScraper.Interfaces;

namespace AutoScraper.TransformFilters {
    public class RemoveAnchorTranform : ITranformFilter {
        public Uri Tranform(Uri url) {
            var indexOfAnchor = url.AbsoluteUri.IndexOf("#", StringComparison.Ordinal);
            return indexOfAnchor >= 0 ? new Uri(url.AbsoluteUri.Substring(0, indexOfAnchor)) : url;
        }
    }
}