namespace Candoumbe.Forms
{
    /// <summary>
    /// Type of <see cref="FormField"/> (<see href="https://ionspec.org/#__code_type_code_3"/>)
    /// </summary>
    public enum FormFieldType
    {
        /// <summary>
        /// The field is a text input field.
        /// </summary>
        String,

        /// <summary>
        /// The field is a text input field with a date value.
        /// </summary>
        Date,

        /// <summary>
        /// The field is a text input field with a datetime value.
        /// </summary>
        DateTime,

        /// <summary>
        /// The field is a text input with an integer value
        /// </summary>
        Integer,

        /// <summary>
        /// The field is a text with a numeric value of some sort
        /// </summary>
        Number,

        /// <summary>
        /// The field is a text with a decimal value
        /// </summary>
        Decimal,

        /// <summary>
        /// Indicates that the field's value will be set using one or more values coming from a array
        /// </summary>
        Array,
        /// <summary>
        /// Indicates that the field's value will be set using one or more values coming from a array.
        /// Each value should be unique in the selection
        /// </summary>
        Set,
    }
}