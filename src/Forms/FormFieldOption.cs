using System;

namespace Forms
{
    /// <summary>
    /// Wraps a <see cref="Label"/> and a <see cref="Value"/>.
    /// </summary>
#if NET5_0_OR_GREATER
    public record FormFieldOption(string Label, object Value);
#elif NETSTANDARD
    public class FormFieldOption
    {
        ///<summary>
        /// The label for the option.
        ///</summary>
        public string Label { get; }

        ///<summary>
        /// The value for the option.
        ///</summary>
        public object Value { get; }

        ///<summary>
        /// Initializes a new instance of the <see cref="FormFieldOption"/> class.
        ///</summary>
        ///<param name="label">The label.</param>
        ///<param name="value">The value.</param>
        public FormFieldOption(string label, object value)
        {
            Label = label;
            Value = value;
        }

        ///<inheritdoc/>
        public override string ToString() => this.Jsonify();
    }
#else
#error "Unsupported framework"
#endif
}