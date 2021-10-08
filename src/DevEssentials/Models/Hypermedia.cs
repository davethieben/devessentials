using System;
using System.Collections.Generic;

namespace Essentials.Models
{
    public class Hypermedia<T> : Hypermedia
        where T : class
    {
        public Hypermedia(T? model = default)
        {
            Model = model;
        }

        public T? Model { get; set; }
    
        public new Hypermedia<T> AddLink(string name, string url, string method = "GET", string? rel = null)
        {
            Links.Add(name, new HypermediaLink(url, rel, method));
            return this;
        }
    }

    public class Hypermedia
    {
        public Dictionary<string, HypermediaLink> Links { get; set; } = new Dictionary<string, HypermediaLink>();
        public List<KeyValuePair<string, string>> Errors { get; set; } = new List<KeyValuePair<string, string>>();

        public Hypermedia AddLink(string name, string url, string method = "GET", string? rel = null)
        {
            Links.Add(name, new HypermediaLink(url, rel, method));
            return this;
        }
    }

    public class HypermediaLink
    {
        public HypermediaLink(string href, string? rel = null, string? method = null, string? type = null)
        {
            Href = href;
            Rel = rel;
            if (method != null)
                Method = method;
        }

        public string Href { get; set; } = default!;
        public string? Rel { get; set; }
        public string Method { get; set; } = "GET";
        public string? Type { get; set; }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class HypermediaLinkAttribute : Attribute
    {
        public string? Name { get; set; }

    }

}
