using System;
using AutoScraper.Interfaces;

namespace AutoScraper.TransformFilters {
    public class RemoveLastSlashTransform : ITranformFilter {
        public Uri Tranform(Uri url) {
            return url.AbsoluteUri.EndsWith("/") ? new Uri(url.AbsoluteUri.TrimEnd('/')) : url;
        }
    }
}