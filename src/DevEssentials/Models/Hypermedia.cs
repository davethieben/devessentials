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
    }

    public class Hypermedia
    {
        public List<HypermediaLink> Links { get; set; } = new List<HypermediaLink>();
        public List<KeyValuePair<string, string>> Errors { get; set; } = new List<KeyValuePair<string, string>>();

        public Hypermedia AddLink(string url, string method = "GET", string? rel = null)
        {
            Links.Add(new HypermediaLink(url, rel, method));
            return this;
        }
    }

    public class HypermediaLink
    {
        public HypermediaLink(string url, string? rel = null, string? method = null, string? type = null)
        {
            Url = url;
            Rel = rel;
            if (method != null)
                Method = method;
        }

        public string Url { get; set; } = default!;
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
