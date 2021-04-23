using System;
using System.Collections.Generic;

namespace Essentials
{
    public class Hypermedia<T>
        where T : class
    {
        public T? Model { get; set; }
        public List<HypermediaLink> Links { get; set; } = new List<HypermediaLink>();
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
