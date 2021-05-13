using System;
using System.Collections.Generic;

namespace Essentials.Models
{
    public class Hypermedia<T> : Hypermedia
        where T : class
    {
        public T? Model { get; set; }
    }

    public class Hypermedia
    {
        public List<HypermediaLink> Links { get; set; } = new List<HypermediaLink>();
        public List<KeyValuePair<string, string>> Errors { get; set; } = new List<KeyValuePair<string, string>>();

    }

    public class HypermediaLink
    {
        public string Url { get; set; } = default!;
        public string Method { get; set; } = "GET";
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class HypermediaLinkAttribute : Attribute
    {
        public string? Name { get; set; }

    }

}
