using System;

namespace Forms
{
#if NET5_0
    public record FormFieldOption(string Label, object Value);
#elif NETSTANDARD1_0 || NETSTANDARD2_0 || NETSTANDARD2_1
    public class FormFieldOption
    {
        public string Label { get; }

        public object Value { get; }

        public FormFieldOption(string label, object value)
        {
            Label = label;
            Value = value;
        }

        public override string ToString() => this.Jsonify();
    }
#else
#error "Unsupported framework"
#endif
}