namespace Forms
{
    /// <summary>
    /// Type of <see cref="FormField"/> (<see cref="https://ionwg.org/#types"/>)
    /// </summary>
    public enum FormFieldType
    {
        String,
        Date,
        DateTime,
        Integer,
        Number,
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