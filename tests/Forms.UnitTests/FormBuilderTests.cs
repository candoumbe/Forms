﻿using FluentAssertions;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Xunit;
using Xunit.Abstractions;
using Xunit.Categories;
using Xunit.Sdk;
using static Forms.FormFieldType;
using static Newtonsoft.Json.JsonConvert;

namespace Forms.UnitTests
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
                    (Expression<Func<Form, bool>>)(form => form.Fields != null && form.Fields.Count() == 1
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
                    (Expression<Func<Form, bool>>)(form => form.Fields != null && form.Fields.Count() == 1
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
                    (Expression<Func<Form, bool>>)(form => form.Fields != null && form.Fields.Count() == 1
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
                    (Expression<Func<Form, bool>>)(form => form.Fields != null && form.Fields.Count() == 1
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
                    (Expression<Func<Form, bool>>)(form => form.Fields != null && form.Fields.Count() == 1
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
                    (Expression<Func<Form, bool>>)(form => form.Fields != null && form.Fields.Exactly(1)
                                                          && form.Fields.Once(field => field.Name == nameof(SuperHero.Cities)
                                                                                       && field.Type == FormFieldType.Array
                                                                                       && field.MinSize == null
                                                                                       && field.MaxSize == 2
                                                          )
                    )
                };
            }
        }

        [Theory]
        [MemberData(nameof(StronglyTypedBuilderCases))]
        public void StronglyTypedBuilder(FormBuilder<SuperHero> builder, Expression<Func<Form, bool>> formExpectation)
        {
            _outputHelper.WriteLine($"FormBuilder : {SerializeObject(builder, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore })}");

            // Act
            Form form = builder.Build();
            _outputHelper.WriteLine($"Form built : {form}");

            // Assert
            form.Fields.Should()
                       .NotBeNull().And
                       .NotContainNulls();
            form.Should()
                .Match(formExpectation);
        }
    }
}
