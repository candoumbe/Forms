using System;
using FluentAssertions;
using Xunit;
using Newtonsoft.Json.Schema;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;

namespace Forms.Tests
{
    public class FormTests
    {
        [Fact]
        public void Default_constructor_sets_items_to_empty()
        {
            // Arrange

            // Act
            Type typeForm = typeof(Form);

            // Assert
            typeForm.Should()
                    .HaveDefaultConstructor().And
                    .NotBeAbstract();
        }


        public static IEnumerable<object[]> FormToSchemaCases
        {
            get
            {
                yield return new object[]
                {
                    Enumerable.Empty<FormField>(),
                    (Expression<Func<JSchema, bool>>)(schema => schema != null && schema.Properties.Exactly(0))
                };

                yield return new object[]
                {
                    new []
                    {
                        new FormField { Name = "Prop1", Description = "Description of the property" }
                    },
                    (Expression<Func<JSchema, bool>>)(schema => schema != null && schema.Properties.Exactly(1)
                                                                && schema.Properties.Once(prop => prop.Key == "Prop1" && prop.Value != null
                                                                    && prop.Value.Description == "Description of the property"
                                                                    && prop.Value.Required.Exactly(0)
                                                                )
                    )
                };

                yield return new object[]
                {
                    new []
                    {
                        new FormField { Name = "Prop1", Required = true, Pattern = "/d{3}" },
                        new FormField { Name = "Prop2", Enabled = false },
                        new FormField { Name = "Prop3", Max = 10, Type = FormFieldType.Decimal }
                    },
                    (Expression<Func<JSchema, bool>>)(schema => schema != null && schema.Properties.Exactly(3)
                        && schema.Required.Exactly(1) && schema.Required.Once(propName => propName == "Prop1")
                        && schema.Properties.Once(prop => prop.Key == "Prop1" && prop.Value.Pattern == "/d{3}")
                        && schema.Properties.Once(prop => prop.Key == "Prop2"
                                                        && prop.Value.ReadOnly.HasValue
                                                        && prop.Value.ReadOnly.Value)
                        && schema.Properties.Once(prop => prop.Key == "Prop3"
                                                        && prop.Value.Maximum == 10
                                                        && prop.Value.Type == JSchemaType.Number)
                    )
                };
            }
        }

        [Theory]
        [MemberData(nameof(FormToSchemaCases))]
        public void Should_generate_json_schema(IEnumerable<FormField> fields, Expression<Func<JSchema, bool>> schemaExpectation)
        {
            // Arrange
            Form form = new Form { Fields = fields };

            // Act
            JSchema schema = form.CompileSchema();

            // Assert
            schema.Should()
                  .Match(schemaExpectation);
        }
    }
}
