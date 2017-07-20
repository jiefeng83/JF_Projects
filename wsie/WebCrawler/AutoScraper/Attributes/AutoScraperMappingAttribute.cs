using System;

namespace AutoScraper.Attributes {
    [AttributeUsage(AttributeTargets.Property)]
    public class AutoScraperMappingAttribute : Attribute {
        public AutoScraperMappingAttribute() {
            Required = true;
        }

        public AutoScraperMappingAttribute(string selector, bool required = true) {
            Selector = selector;
            Required = required;
        }

        public string Selector { get; set; }

        public bool Required { get; set; }
    }
}