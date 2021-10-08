using System;

namespace Essentials
{
    /// <summary>
    /// override of <see cref="UriBuilder"/> that doesn't include common web ports 80, 443 in output string
    /// </summary>
    public class UrlBuilder : UriBuilder
    {
        public UrlBuilder(string uri) : base(uri) { }

        public override string ToString() => base.ToString().Replace(":80", "").Replace(":443", "");

    }
}
