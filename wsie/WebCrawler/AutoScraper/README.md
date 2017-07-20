# AutoScraper

AutoScraper is now also available on NuGet. 

It can be found at https://www.nuget.org/packages/AutoScraper


In this example we scrape movie titles from IMDB.

    using System;
    using AutoScraper;
    using AutoScraper.Attributes;
    using AutoScraper.Events;
    using AutoScraper.Schedulers;
    using AutoScraper.TransformFilters;
    using AutoScraper.WebClients;

    namespace AutoScraperExample {
        internal class Program {
            private static void Main() {
                using (var scraper = new Scraper<InMemoryScheduler, DefaultWebClient, MovieWebModel>("http://www.imdb.com/")) {
                    scraper.OnPageScrapeComplete += ScraperOnOnPageScrapeComplete;

                    // These are IMBD specific, not needed most of the time.
                    scraper.TransformFilters.Add(new RemoveQueryTranform());
                    scraper.TransformFilters.Add(new RemoveLastSlashTransform());

                    scraper.Run();
                }
            }

            private static void ScraperOnOnPageScrapeComplete(Crawler<InMemoryScheduler, DefaultWebClient> sender,
                PageScrapeCompleteEventArgs<MovieWebModel> scrapedMovie) {
                    Console.WriteLine($"{scrapedMovie.Model.Name} - {scrapedMovie.Url}");
            }
        }

        internal class MovieWebModel {
            [AutoScraperMapping("h1[itemprop='name']")]
            public string Name { get; set; }
        }
    }
