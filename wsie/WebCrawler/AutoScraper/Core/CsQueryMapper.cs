using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoScraper.Attributes;
using AutoScraper.Models;
using CsQuery;

namespace AutoScraper.Core {
    internal class CsQueryMapper<T> where T : new() {
        private IEnumerable<PropertyMapping> _propertyMappings;

        public Dictionary<string, Func<CQ, string>> CustomMappings;

        public CsQueryMapper() {
            CustomMappings = new Dictionary<string, Func<CQ, string>>();

            PreloadPropertyMappings();
        }

        private void PreloadPropertyMappings() {
            if (_propertyMappings == null) {
                _propertyMappings = typeof(T)
                    .GetProperties()
                    .Where(prop => GetMappingAttribute(prop) != null)
                    .Select(prop => new PropertyMapping {
                        Property = prop,
                        Attribute = GetMappingAttribute(prop)
                    });
            }
        }

        public bool TryMap(string html, out T model) {
            return TryMap(CQ.CreateDocument(html), out model);
        }

        public bool TryMap(CQ dom, out T model) {
            model = (T) Activator.CreateInstance(typeof(T));

            /* Map Values */
            foreach (var propertyMapping in _propertyMappings) {
                var elementValue = CustomMappings.ContainsKey(propertyMapping.Property.Name)
                    ? CustomMappings[propertyMapping.Property.Name](dom[propertyMapping.Attribute.Selector])
                    : dom[propertyMapping.Attribute.Selector].Text();

                propertyMapping.Property.SetValue(model, elementValue, null);
            }

            /* Validate Values */
            foreach (var propertyMapping in _propertyMappings.Where(p => p.Attribute.Required)) {
                if (String.IsNullOrWhiteSpace(propertyMapping.Property.GetValue(model) as string)) {
                    return false;
                }
            }

            return true;
        }

        private static AutoScraperMappingAttribute GetMappingAttribute(MemberInfo prop) {
            return prop.GetCustomAttributes<AutoScraperMappingAttribute>(true).FirstOrDefault();
        }
    }
}