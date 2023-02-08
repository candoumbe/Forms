namespace Forms
{
    /// <summary>
    /// A ION resource
    /// </summary>
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
    public record IonResource
#else
    public class IonResource
#endif
    {
        /// <summary>
        /// Metadata information about the resource
        /// </summary>
        public Link Meta { get; set; }
    }
}
