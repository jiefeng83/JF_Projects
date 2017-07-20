using System;

namespace AutoScraper.Interfaces {
    public interface ITranformFilter {
        Uri Tranform(Uri url);
    }
}