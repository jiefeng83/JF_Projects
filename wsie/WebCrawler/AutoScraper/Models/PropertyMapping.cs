using System.Reflection;
using AutoScraper.Attributes;

namespace AutoScraper.Models {
    internal class PropertyMapping {
        public PropertyInfo Property { get; set; }
        public AutoScraperMappingAttribute Attribute { get; set; }
    }
}