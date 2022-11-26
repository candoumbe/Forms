using Newtonsoft.Json.Schema;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Forms
{
    /// <summary>
    /// Form representation
    /// </summary>
    /// <remarks>
    ///     This class, inspired by ION spec (see http://ionwg.org/draft-ion.html#forms for more details), 
    ///     can be used to describe a 
    /// </remarks>
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
    public record Form : IonResource
#else
    public class Form : IonResource 
#endif
    {
        /// <summary>
        /// Fields of the form
        /// </summary>
        public IEnumerable<FormField> Fields { get; set; }

        /// <summary>
        /// Builds a new <see cref="Form"/> instance.
        /// </summary>
        public Form() => Fields = Enumerable.Empty<FormField>();

#if !NETSTANDARD1_0
        /// <summary>
        /// <para>Compile a JSON Schema representation of the current <see cref="Form"/>.</para>
        /// <para>The generated schema can be later used to validate any incoming json against this form</para>
        /// 
        /// </summary>
        /// <returns>JSON Schema that can be used to represent the current <see cref="Form"/>.</returns>
        public JSchema CompileSchema()
        {
            JSchema schema = new JSchema();

            foreach (FormField field in Fields)
            {
                schema.Properties.Add(field.Name, new JSchema
                {
                    Description = field.Description,
                    ReadOnly = field.Enabled switch
                    {
                        null => null,
                        true => false,
                        false => true
                    },
                    Pattern = field.Pattern,
                    Maximum = field.Max,
                    MaximumLength = field.MaxLength,
                    Minimum = field.Min,
                    MinimumLength = field.MinLength,
                    Type = field.Type switch
                    {
                        FormFieldType.Decimal => JSchemaType.Number,
                        FormFieldType.Integer => JSchemaType.Integer,
                        FormFieldType.Number => JSchemaType.Number,
                        _ => JSchemaType.String
                    }
                });

                if (field.Required ?? false)
                {
                    schema.Required.Add(field.Name);
                }
            }

            return schema;
        }
#endif

        ///<inheritdoc/>
        public override string ToString()
#if NETSTANDARD1_0
        => this.Jsonify(new() { NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore });
#else
        => this.Jsonify(new(System.Text.Json.JsonSerializerDefaults.Web) { DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault });
#endif
    }
}
