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
        public Dictionary<string, HypermediaLink> Links { get; set; } = new();
        public Dictionary<string, string> Errors { get; set; } = new();

        public Hypermedia AddLink(string name, string url, string method = "GET", string? rel = null)
        {
            Links.Add(name, new HypermediaLink(url, rel, method));
            return this;
        }
    }

    public record HypermediaLink(string Href, string? Rel = null, string Method = "GET", string? Type = null)
    {
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class HypermediaLinkAttribute : Attribute
    {
        public string? Name { get; set; }

    }

}

namespace System.Runtime.CompilerServices
{
    public struct IsExternalInit { }
}
