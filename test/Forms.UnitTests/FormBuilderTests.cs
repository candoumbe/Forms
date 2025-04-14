using FluentAssertions;

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text.Json;

using Xunit;
using Xunit.Abstractions;
using Xunit.Categories;

using static Candoumbe.Forms.FormFieldType;

namespace Candoumbe.Forms.UnitTests;

[UnitTest]
public class FormBuilderTests(ITestOutputHelper outputHelper)
{
    public class SuperHero
    {
        public Guid Id { get; init; }
        
        public string Nickname { get; set; }

        [FormField(Secret = true)]
        public string RealName { get; set; }

        [FormField(MaxSize = 2)]
        public IEnumerable<string> Cities { get; set; }

        public DateOnly? BirthDate { get; set; }

        public DateTime? LastBattleDate { get; set; }

        public int? CurrentWinningStreakCount { get; set; }
    }

    public static TheoryData<FormBuilder<SuperHero>, Form> StronglyTypedBuilderCases
        => new()
        {
            {
                new FormBuilder<SuperHero>()
                    .AddField(x => x.Nickname),
                new Form
                {
                    Fields =
                    [
                        new FormField
                        {
                            Name = nameof(SuperHero.Nickname),
                            Type = FormFieldType.String,
                            Label = nameof(SuperHero.Nickname)
                        } 
                    ]
                }
            },
            {
                new FormBuilder<SuperHero>()
                    .AddField(x => x.RealName),
                new Form
                {
                    Fields =
                    [
                        new FormField
                        {
                            Name = nameof(SuperHero.RealName),
                            Type = FormFieldType.String,
                            Label = nameof(SuperHero.RealName),
                            Secret = true
                        } 
                    ]
                }
            },
            {
                new FormBuilder<SuperHero>()
                    .AddField(x => x.RealName, new FormFieldAttributeOverrides { Secret = false }),
                new Form
                {
                    Fields =
                    [
                        new FormField
                        {
                            Name = nameof(SuperHero.RealName),
                            Type = FormFieldType.String ,
                            Label = nameof(SuperHero.RealName),
                            Secret = false
                        } 
                    ]
                }
            },
            {
                new FormBuilder<SuperHero>()
                    .AddField(x => x.RealName, new FormFieldAttributeOverrides { Description = "Secret identity of the hero" }),
                new Form
                {
                    Fields =
                    [
                        new FormField
                        {
                            Name = nameof(SuperHero.RealName),
                            Type = FormFieldType.String,
                            Label = nameof(SuperHero.RealName),
                            Secret = true,
                            Description = "Secret identity of the hero"
                        } 
                    ]
                }
            },
            {
                new FormBuilder<SuperHero>()
                    .AddField(x => x.BirthDate),
                new Form
                {
                    Fields =
                    [
                        new FormField
                        {
                            Name = nameof(SuperHero.BirthDate),
                            Type = Date,
                            Label = nameof(SuperHero.BirthDate)
                        } 
                    ]
                }
            },
            {
                new FormBuilder<SuperHero>()
                    .AddField(x => x.CurrentWinningStreakCount),
                new Form
                {
                    Fields =
                    [
                        new FormField
                        {
                            Name = nameof(SuperHero.CurrentWinningStreakCount),
                            Type = Integer,
                            Label = nameof(SuperHero.CurrentWinningStreakCount)
                        } 
                    ]
                }
            },
            {
                new FormBuilder<SuperHero>()
                    .AddOptions(x => x.Cities, options: ["Metropolis", "Gotham", "Central city"]),
                new Form
                {
                    Fields =
                    [
                        new FormField
                        {
                            Name = nameof(SuperHero.Cities),
                            Type = FormFieldType.Array,
                            Label = nameof(SuperHero.Cities),
                            MaxSize = 2,
                            Options =
                            [
                                new FormFieldOption("Metropolis", "Metropolis"),
                                new FormFieldOption("Gotham", "Gotham"),
                                new FormFieldOption("Central city", "Central city")
                            ]
                        } 
                    ]
                }
            },
            {
                new FormBuilder<SuperHero>()
                    .AddField(x => x.Id),
                new Form
                {
                    Fields = [
                        new FormField { Name = "Id", Type = FormFieldType.String, Label = nameof(SuperHero.Id) },
                    ]
                }
            }
        };

    [Theory]
    [MemberData(nameof(StronglyTypedBuilderCases))]
    public void StronglyTypedBuilder(FormBuilder<SuperHero> builder, Form expected)
    {
        outputHelper.WriteLine($"FormBuilder :  {builder.Jsonify(new JsonSerializerOptions(JsonSerializerDefaults.Web))}");

        // Act
        Form actual = builder.Build();

        // Assert
        actual.Fields.Should()
            .NotBeNull().And
            .NotContainNulls();
        actual.Should().BeEquivalentTo(expected);
    }
}