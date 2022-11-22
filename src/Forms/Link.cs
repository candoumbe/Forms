using System;
using System.Collections.Generic;

namespace Forms
{
    /// <summary>
    /// Link representation
    /// </summary>
    /// <remarks>
    ///     Inspired by ION spec (see http://ionwg.org/draft-ion.html#links for more details)
    /// </remarks>
    public class Link
    {
        /// <summary>
        /// Url of the resource the current <see cref="Link"/> points to.
        /// </summary>
        public string Href { get; set; }

        /// <summary>
        /// Relations of the resource that <see cref="Link"/> points to with the current resource
        /// </summary>
#if NETSTANDARD
        public IEnumerable<string> Relations { get; set; } 
#else
        public IReadOnlySet<string> Relations { get; set; }
#endif

        /// <summary>
        /// Http method to used in conjunction with <see cref="Href"/>.
        /// </summary>
        /// <remarks>
        /// </remarks>
        public string Method { get; set; }

        /// <summary>
        /// Title associated with the link
        /// </summary>
        /// <remarks>
        /// Should be a friendly name suitable to used in a HTML a tag.
        /// </remarks>
        public string Title { get; set; }

        /// <summary>
        /// Indicates if the current <see cref="Href"/> is a template url
        /// </summary>
        /// <remarks>
        /// <para>
        /// A template url is a url with generic placeholder.
        /// <c>api/patients/{id?}</c> is a template url as it contiains one placeholder named <c>id</c>.
        /// </para>
        /// </remarks>
        public bool? Template => Href?.Like("*{?*}*");

        /// <summary>
        /// Builds a new <see cref="Link"/> instance.
        /// </summary>
        public Link()
        {
            Relations = new HashSet<string>();
        }

        /// <inheritdoc/>
        public override string ToString() => this.Jsonify();
    }
}
