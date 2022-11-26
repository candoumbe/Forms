using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using System.Collections.Generic;

namespace Forms
{
    /// <summary>
    /// Form field representation.
    /// <para>
    ///     Inspired by ION spec (see http://ionwg.org/draft-ion.html#form-fields for more details)
    /// </para>
    /// </summary>
    [JsonObject]
#if NET5_0_OR_GREATER
    public record FormField
#else
    public class FormField
#endif
    {
        private IEnumerable<FormFieldOption> _options;

        /// <summary>
        /// indicates whether or not the field value may be modified or submitted to a linked resource location. 
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? Enabled
        {
            get;
#if NET5_0_OR_GREATER
            init;
#else
            set;
#endif
        }

        /// <summary>
        /// Type of the field
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public FormFieldType Type { get; set; }

        /// <summary>
        /// Description of the field
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Label
        {
            get;
#if NET5_0_OR_GREATER
            init;
#else
            set;
#endif
        }

        /// <summary>
        /// Name of the field that should be submitted
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Name
        {
            get;
#if NET5_0_OR_GREATER
            init;
#else
            set;
#endif
        }

        /// <summary>
        /// Regular expression that the field should be validated against.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Pattern
        {
            get;
#if NET5_0_OR_GREATER
            init;
#else
            set;
#endif
        }

        /// <summary>
        /// Short hint that described the expected value of the field.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Placeholder
        {
            get;
#if NET5_0_OR_GREATER
            init;
#else
            set;
#endif
        }

        /// <summary>
        /// Indicates if the field must be submitted
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? Required
        {
            get;
#if NET5_0_OR_GREATER
            init;
#else
            set;
#endif
        }

        /// <summary>
        /// Indicates the maximum length of the value
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? MaxLength
        {
            get;
#if NET5_0_OR_GREATER
            init;
#else
            set;
#endif
        }

        /// <summary>
        /// Indicates the minimum length of the value
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? MinLength
        {
            get;
#if NET5_0_OR_GREATER
            init;
#else
            set;
#endif
        }

        /// <summary>
        /// Indicates that value must be greater than or equal to the specified <see cref="Min"/> value
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public float? Min
        {
            get;
#if NET5_0_OR_GREATER
            init;
#else
            set;
#endif
        }

        /// <summary>
        /// Indicates that value must be less than or equal to the specified <see cref="Max"/> value
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public float? Max
        {
            get;
#if NET5_0_OR_GREATER
            init;
#else
            set;
#endif
        }

        /// <summary>
        /// Indicates whether or not the field value is considered sensitive information 
        /// and should be kept secret.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? Secret
        {
            get;
#if NET5_0_OR_GREATER
            init;
#else
            set;
#endif
        }

        /// <summary>
        /// a string description of the field that may be used to enhance usability.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Description
        {
            get;
#if NET5_0_OR_GREATER
            init;
#else
            set;
#endif
        }

        /// <summary>
        /// List of options
        /// </summary>
        public IEnumerable<FormFieldOption> Options
        {
            get => _options;
#if NET5_0_OR_GREATER
            init
#else
            set
#endif
            {
                _options = value;
                Type = FormFieldType.Array;
            }
        }

        /// <summary>
        /// a non-negative integer that specifies the minimum number of field values that may be submitted when the field type value 
        /// equals <see cref="FormFieldType.Array"/> or <see cref="FormFieldType.Set"/>
        /// </summary>
        public int? MinSize { get; set; }

        /// <summary>
        /// a non-negative integer that specifies the maximum number of field values that may be submitted when the field type value 
        /// equals <see cref="FormFieldType.Array"/> or <see cref="FormFieldType.Set"/>
        /// </summary>
        public int? MaxSize { get; set; }
    }
}