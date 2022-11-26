using System;
using System.Collections.Generic;
using System.Linq;

namespace Forms
{
    /// <summary>
    /// Link representation
    /// </summary>
    /// <remarks>
    ///     Inspired by ION spec (see http://ionwg.org/draft-ion.html#links for more details)
    /// </remarks>
#if NETSTANDARD2_1_OR_GREATER
    public record Link
#else
    public class Link
#endif
    {
        /// <summary>
        /// Url of the resource the current <see cref="Link"/> points to.
        /// </summary>
        public string Href
        {
            get;
#if NET5_0_OR_GREATER
            init;
#else
            set;
#endif
        }

        /// <summary>
        /// Relations of the resource that <see cref="Link"/> points to with the current resource
        /// </summary>
#if NETSTANDARD
        public IEnumerable<string> Relations
        {
            get => _relations.Distinct();
            set => _relations = value?.Distinct() ?? Enumerable.Empty<string>();
        }

        private IEnumerable<string> _relations;
#else
        public IReadOnlySet<string> Relations { get; init; }
#endif

        /// <summary>
        /// Http method to used in conjunction with <see cref="Href"/>.
        /// </summary>
        /// <remarks>
        /// </remarks>
        public string Method
        {
            get;
#if NET5_0_OR_GREATER
            init;
#else
            set;
#endif
        }

        /// <summary>
        /// Title associated with the link
        /// </summary>
        /// <remarks>
        /// Should be a friendly name suitable to used in a HTML a tag.
        /// </remarks>
        public string Title
        {
            get;
#if NET5_0_OR_GREATER
            init;
#else
            set;
#endif
        }

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
#if NET5_0_OR_GREATER
            Relations = new HashSet<string>();
#else
            _relations = new HashSet<string>();
#endif
        }

        /// <inheritdoc/>
        public override string ToString() => this.Jsonify();
    }
}
