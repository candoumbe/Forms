using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using Optional;

using static Forms.FormFieldType;

namespace Forms
{
    /// <summary>
    /// Helper class to build a <see cref="Form"/> instance
    /// </summary>
    /// <typeparam name="T">Type to build a <see cref="Form"/> for.</typeparam>
    public class FormBuilder<T>
    {
        private static readonly HashSet<Type> _dateTypes = new HashSet<Type>
        {
            typeof(DateTime), typeof(DateTime?),
            typeof(DateTimeOffset), typeof(DateTimeOffset?),
        };

        private static readonly HashSet<Type> _numericTypes = new HashSet<Type>
        {
            typeof(int), typeof(int?),
            typeof(float), typeof(float?),
            typeof(long), typeof(long?),
            typeof(double), typeof(double?),
            typeof(short), typeof(short?),
            typeof(decimal), typeof(decimal?)
        };

        private readonly IList<FormField> _fields;
        private readonly Link _meta;

        /// <summary>
        /// Creates a new <see cref="FormBuilder{T}"/> instance
        /// </summary>
        /// <param name="meta">describes where and how to send the form's data</param>
        public FormBuilder(Link meta = null)
        {
            _fields = new List<FormField>();
            _meta = meta;
        }

        /// <summary>
        /// Adds a field to the <see cref="Form"/>'s configuration.
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="property"></param>
        /// <param name="attributes">Overrides field's attributes</param>
        /// <returns>The current instance.</returns>
        public FormBuilder<T> AddField<TProperty>(Expression<Func<T, TProperty>> property, FormFieldAttributeOverrides attributes = null)
        {
            if (property.Body is MemberExpression me)
            {
                FormField field = new FormField { Name = me.Member.Name };
                Option<FormFieldAttribute> optionalFormFieldAttribute = me.Member.GetCustomAttribute<FormFieldAttribute>()
                    .SomeNotNull();
                UpdateAttributesField(property, field, optionalFormFieldAttribute, attributes.SomeNotNull());

                field.Label = field.Name;
                field.Enabled = attributes?.Enabled;

                _fields.Add(field);
            }

            return this;
        }

        private static void UpdateAttributesField<TProperty>(Expression<Func<T, TProperty>> property,
                                                             FormField field,
                                                             Option<FormFieldAttribute> optionalFormFieldAttribute,
                                                             Option<FormFieldAttributeOverrides> optionalAttributesOverride)
        {
            if (_dateTypes.Contains(property.ReturnType))
            {
                field.Type = FormFieldType.DateTime;
            }
            else if (_numericTypes.Contains(property.ReturnType))
            {
                field.Type = Integer;
            }

            optionalFormFieldAttribute.MatchSome(
                attr =>
                {
                    if (attr.IsDescriptionSet)
                    {
                        field.Description = attr.Description;
                    }
                    if (attr.IsSecretSet)
                    {
                        field.Secret = attr.Secret;
                    }

                    if (attr.IsMinSet)
                    {
                        field.Min = attr.Min;
                    }

                    field.Pattern = attr.Pattern;
                    if (attr.IsTypeSet)
                    {
                        field.Type = attr.Type;
                    }
                    if (attr.IsMinSet)
                    {
                        field.Min = attr.Min;
                    }

                    if (attr.IsMaxSet)
                    {
                        field.Max = attr.Max;
                    }

                    if (attr.IsMinLengthSet)
                    {
                        field.MinLength = attr.MinLength;
                    }

                    if (attr.IsMaxLengthSet)
                    {
                        field.MaxLength = attr.MaxLength;
                    }
                    if (attr.IsTypeSet)
                    {
                        field.Type = attr.Type;
                    }

                    if (attr.IsMaxSizeSet)
                    {
                        field.MaxSize = attr.MaxSize;
                    }

                    if (attr.IsEnabledSet)
                    {
                        field.Enabled = attr.Enabled;
                    }
                });

            optionalAttributesOverride.MatchSome((attrs) =>
            {
                if (attrs.Min.HasValue)
                {
                    field.Min = attrs.Min;
                }
                if (attrs.Secret.HasValue)
                {
                    field.Secret = attrs.Secret;
                }
                if (attrs.IsDescriptionSet)
                {
                    field.Description = attrs.Description;
                }
                field.Label = attrs.Label ?? field.Name;
                if (attrs.Max.HasValue)
                {
                    field.Max = attrs.Max;
                }
                if (attrs.Pattern != null)
                {
                    field.Pattern = attrs.Pattern;
                }

                // if (attrs.IsMinLengthSet)
                // {
                //     field.MaxLength = attrs.MaxLength;
                // }

                if (attrs.IsMaxLengthSet)
                {
                    field.MaxLength = attrs.MaxLength;
                }

                if (attrs.IsTypeSet)
                {
                    field.Type = attrs.Type;
                }
            });
        }

        /// <summary>
        /// Ands a field to the <see cref="Form"/>'s configuration.
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="property"></param>
        /// <param name="options"></param>
        /// <param name="attributeOverrides"></param>
        /// <returns></returns>
        public FormBuilder<T> AddOptions<TProperty>(Expression<Func<T, TProperty>> property,
                                                    IEnumerable<FormFieldOption> options,
                                                    FormFieldAttributeOverrides attributeOverrides = null)
        {
            MemberExpression me = property.Body as MemberExpression;
            FormField field = new() { Name = me.Member.Name, Type = FormFieldType.Array };
            Option<FormFieldAttribute> optionalFormFieldAttribute = me.Member.GetCustomAttribute<FormFieldAttribute>()
                                                                             .SomeNotNull();

            field.Options = options;

            UpdateAttributesField(property, field, optionalFormFieldAttribute, attributeOverrides.SomeNotNull());

            _fields.Add(field);

            return this;
        }

        /// <summary>
        /// Adds a field to the <see cref="Form"/>'s configuration.
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="property">Defines the property for which options will be added</param>
        /// <param name="options"></param>
        /// <param name="attributeOverrides"></param>
        /// <returns></returns>
        public FormBuilder<T> AddOptions<TProperty>(Expression<Func<T, TProperty>> property,
                                                    IEnumerable<string> options,
                                                    FormFieldAttributeOverrides attributeOverrides = null)
                                                    => AddOptions(property, options.Select(opt => new FormFieldOption(opt, opt)), attributeOverrides);

        /// <summary>
        /// Builds a <see cref="Form"/> instance according to the current configuration.
        /// </summary>
        /// <returns></returns>
        public Form Build() => new()
        {
            Meta = _meta,
            Fields = _fields,
        };
    }
}
