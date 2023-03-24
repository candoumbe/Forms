using Optional;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using static Candoumbe.Forms.FormFieldType;

namespace Candoumbe.Forms
{
    /// <summary>
    /// Helper class to build a <see cref="Form"/> instance
    /// </summary>
    /// <typeparam name="T">Type to build a <see cref="Form"/> for.</typeparam>
    public class FormBuilder<T>
    {
        private static readonly HashSet<Type> DateTypes = new HashSet<Type>
        {
            typeof(DateTime), typeof(DateTime?),
            typeof(DateTimeOffset), typeof(DateTimeOffset?),
        };

        private static readonly HashSet<Type> NumericTypes = new HashSet<Type>
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
                field = UpdateAttributesField(property, field, optionalFormFieldAttribute, attributes.SomeNotNull());

#if NET5_0_OR_GREATER
                field = field with
                {
                    Label = field.Name,
                    Enabled = attributes?.Enabled
                };
#else
                field.Label = field.Name;
                field.Enabled = attributes?.Enabled;
#endif

                _fields.Add(field);
            }

            return this;
        }

        private static FormField UpdateAttributesField<TProperty>(Expression<Func<T, TProperty>> property,
                                                             FormField field,
                                                             Option<FormFieldAttribute> optionalFormFieldAttribute,
                                                             Option<FormFieldAttributeOverrides> optionalAttributesOverride)
        {
            if (DateTypes.Contains(property.ReturnType))
            {
                field.Type = FormFieldType.DateTime;
            }
            else if (NumericTypes.Contains(property.ReturnType))
            {
                field.Type = Integer;
            }

            optionalFormFieldAttribute.MatchSome(
                attr =>
                {
                    if (attr.IsDescriptionSet)
                    {
#if NET5_0_OR_GREATER
                        field = field with { Description = attr.Description };
#else
                        field.Description = attr.Description;
#endif
                    }
                    if (attr.IsSecretSet)
                    {
#if NET5_0_OR_GREATER
                        field = field with { Secret = attr.Secret };
#else
                        field.Secret = attr.Secret;
#endif
                    }

                    if (attr.IsMinSet)
                    {
#if NET5_0_OR_GREATER
                        field = field with { MinSize = attr.MinSize };
#else
                        field.MinSize = attr.MinSize;
#endif
                    }

#if NET5_0_OR_GREATER
                    field = field with { Pattern = attr.Pattern };
#else
                    field.Pattern = attr.Pattern;
#endif

                    if (attr.IsTypeSet)
                    {
#if NET5_0_OR_GREATER
                        field = field with { Type = attr.Type };
#else
                        field.Type = attr.Type;
#endif
                    }

                    if (attr.IsMinSet)
                    {
#if NET5_0_OR_GREATER
                        field = field with { Min = attr.Min };
#else
                        field.Min = attr.Min;
#endif
                    }

                    if (attr.IsMaxSet)
                    {
#if NET5_0_OR_GREATER
                        field = field with { Max = attr.Max };
#else
                        field.Max = attr.Max;
#endif
                    }

                    if (attr.IsMinLengthSet)
                    {
#if NET5_0_OR_GREATER
                        field = field with { MinLength = attr.MinLength };
#else
                        field.MinLength = attr.MinLength;
#endif
                    }

                    if (attr.IsMaxLengthSet)
                    {
#if NET5_0_OR_GREATER
                        field = field with { MaxLength = attr.MaxLength };
#else
                        field.MaxLength = attr.MaxLength;
#endif
                    }

                    if (attr.IsTypeSet)
                    {
#if NET5_0_OR_GREATER
                        field = field with { Type = attr.Type };
#else
                        field.Type = attr.Type;
#endif
                    }

                    if (attr.IsMaxSizeSet)
                    {
#if NET5_0_OR_GREATER
                        field = field with { MaxSize = attr.MaxSize };
#else
                        field.MaxSize = attr.MaxSize;
#endif
                    }

                    if (attr.IsEnabledSet)
                    {
#if NET5_0_OR_GREATER
                        field = field with { Enabled = attr.Enabled };
#else
                        field.Enabled = attr.Enabled;
#endif
                    }
                });

            optionalAttributesOverride.MatchSome((attr) =>
            {
                if (attr.Min.HasValue)
                {
#if NET5_0_OR_GREATER
                    field = field with { Min = attr.Min };
#else
                    field.Min = attr.Min;
#endif
                }
                if (attr.Secret.HasValue)
                {
#if NET5_0_OR_GREATER
                    field = field with { Secret = attr.Secret };
#else
                    field.Secret = attr.Secret;
#endif
                }
                if (attr.IsDescriptionSet)
                {
#if NET5_0_OR_GREATER
                    field = field with { Description = attr.Description };
#else
                    field.Description = attr.Description;
#endif
                }

#if NET5_0_OR_GREATER
                field = field with { Label = attr.Label ?? field.Name };
#else
                field.Label = attr.Label ?? field.Name;
#endif

                if (attr.Max.HasValue)
                {
#if NET5_0_OR_GREATER
                    field = field with { Max = attr.Max };
#else
                    field.Max = attr.Max;
#endif
                }
                if (attr.Pattern is not null)
                {
#if NET5_0_OR_GREATER
                    field = field with { Pattern = attr.Pattern };
#else
                    field.Pattern = attr.Pattern;
#endif
                }

                // if (attrs.IsMinLengthSet)
                // {
                //     field.MaxLength = attrs.MaxLength;
                // }

                if (attr.IsMaxLengthSet)
                {
#if NET5_0_OR_GREATER
                    field = field with { MaxLength = attr.MaxLength };
#else
                    field.MaxLength = attr.MaxLength;
#endif
                }
            });

            return field;
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

#if NET5_0_OR_GREATER
            field = field with
            {
                Options = options,
#if NET6_0_OR_GREATER
                MaxSize = options.TryGetNonEnumeratedCount(out int count) ? count : options.Count()
#else
                MaxSize = options.Count()
#endif

            };

#else
            field.Options = options;
#endif

            field = UpdateAttributesField(property, field, optionalFormFieldAttribute, attributeOverrides.SomeNotNull());

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
