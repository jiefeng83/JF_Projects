using System;

namespace AutoScraper.Interfaces {
    public interface IScheduler {
        bool TryGetNext(out Uri uri, TimeSpan timeout);
        void ReScheduleUri(Uri uri, int maxRetries = 3);
        void ScheduleUri(Uri uri);
    }
}