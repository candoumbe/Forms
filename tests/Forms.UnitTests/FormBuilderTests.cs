using FluentAssertions;

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text.Json;

using Xunit;
using Xunit.Abstractions;
using Xunit.Categories;

using static Candoumbe.Forms.FormFieldType;

namespace Candoumbe.Forms.UnitTests
{
    [UnitTest]
    public class FormBuilderTests
    {
        private readonly ITestOutputHelper _outputHelper;

        public class SuperHero
        {
            public string Nickname { get; set; }

            [FormField(Secret = true)]
            public string RealName { get; set; }

            [FormField(MaxSize = 2)]
            public IEnumerable<string> Cities { get; set; }

            public DateTime? BirthDate { get; set; }

            public DateTime? LastBattleDate { get; set; }

            public int? CurrentWinningStreakCount { get; set; }
        }

        public FormBuilderTests(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

        public static IEnumerable<object[]> StronglyTypedBuilderCases
        {
            get
            {
                yield return new object[]
                {
                    new FormBuilder<SuperHero>()
                        .AddField(x => x.Nickname),
                    (Expression<Func<Form, bool>>)(form => form.Fields != null
                                                           && form.Fields.Once()
                                                           && form.Fields.Once(field => field.Name == nameof(SuperHero.Nickname)
                                                               && field.Type == FormFieldType.String
                                                               && field.Label == nameof(SuperHero.Nickname)
                                                           )
                    )
                };

                yield return new object[]
                {
                    new FormBuilder<SuperHero>()
                        .AddField(x => x.RealName),
                    (Expression<Func<Form, bool>>)(form => form.Fields != null && form.Fields.Exactly(1)
                        && form.Fields.Once(field => field.Name == nameof(SuperHero.RealName)
                            && field.Type == FormFieldType.String
                            && field.Label == nameof(SuperHero.RealName)
                            && field.Secret.HasValue && field.Secret.Value
                        )
                    )
                };

                yield return new object[]
                {
                    new FormBuilder<SuperHero>()
                        .AddField(x => x.RealName, new FormFieldAttributeOverrides { Secret = false }),
                    (Expression<Func<Form, bool>>)(form => form.Fields != null && form.Fields.Once()
                        && form.Fields.Once(field => field.Name == nameof(SuperHero.RealName)
                            && field.Type == FormFieldType.String
                            && field.Label == nameof(SuperHero.RealName)
                            && field.Secret.HasValue
                        )
                    )
                };

                yield return new object[]
                {
                    new FormBuilder<SuperHero>()
                        .AddField(x => x.RealName, new FormFieldAttributeOverrides { Description = "Secret identity of the hero" }),
                    (Expression<Func<Form, bool>>)(form => form.Fields != null && form.Fields.Once()
                        && form.Fields.Once(field => field.Name == nameof(SuperHero.RealName)
                            && field.Type == FormFieldType.String
                            && field.Label == nameof(SuperHero.RealName)
                            && field.Description == "Secret identity of the hero"
                        )
                    )
                };

                yield return new object[]
                {
                    new FormBuilder<SuperHero>()
                        .AddField(x => x.BirthDate),
                    (Expression<Func<Form, bool>>)(form => form.Fields != null && form.Fields.Once()
                        && form.Fields.Once(field => field.Name == nameof(SuperHero.BirthDate)
                            && field.Type == FormFieldType.DateTime
                            && field.Label == nameof(SuperHero.BirthDate)
                        )
                    )
                };

                yield return new object[]
                {
                    new FormBuilder<SuperHero>()
                        .AddField(x => x.CurrentWinningStreakCount),
                    (Expression<Func<Form, bool>>)(form => form.Fields != null && form.Fields.Once()
                        && form.Fields.Once(field => field.Name == nameof(SuperHero.CurrentWinningStreakCount)
                                                     && field.Type == Integer
                                                     && field.Label == nameof(SuperHero.CurrentWinningStreakCount)
                        )
                    )
                };

                yield return new object[]
                {
                    new FormBuilder<SuperHero>()
                        .AddOptions(x => x.Cities, options: new []{ "Metropolis", "Gotham", "Central city" }),
                    (Expression<Func<Form, bool>>)(form => form.Fields != null && form.Fields.Once()
                                                          && form.Fields.Once(field => field.Name == nameof(SuperHero.Cities)
                                                                                       && field.Type == FormFieldType.Array
                                                                                       && field.Options != null
                                                                                       && field.Options.Once(opt => opt.Label == "Metropolis" && Equals(opt.Value, opt.Label))
                                                                                       && field.Options.Once(opt => opt.Label == "Gotham" && Equals(opt.Value, opt.Label))
                                                                                       && field.Options.Once(opt => opt.Label == "Central city" && Equals(opt.Value, opt.Label))
                                                          )
                    )
                };
            }
        }

        [Theory]
        [MemberData(nameof(StronglyTypedBuilderCases))]
        public void StronglyTypedBuilder(FormBuilder<SuperHero> builder, Expression<Func<Form, bool>> formExpectation)
        {
            _outputHelper.WriteLine($"FormBuilder :  {builder.Jsonify(new(JsonSerializerDefaults.Web))}");

            // Act
            Form form = builder.Build();
            _outputHelper.WriteLine($"Form built : {form.Jsonify(new(JsonSerializerDefaults.Web) { DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull })}");

            // Assert
            form.Fields.Should()
                       .NotBeNull().And
                       .NotContainNulls();
            form.Should()
                .Match(formExpectation);
        }
    }
}
