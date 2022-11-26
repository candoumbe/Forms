using Bogus;

using FluentAssertions;

using System.Collections.Generic;

using Xunit;
using Xunit.Categories;

namespace Forms.UnitTests
{
    [Feature(nameof(Forms))]
    [Feature(nameof(Link))]
    public class LinkTests
    {
        private static readonly Faker Faker = new();

        [Fact]
        public void Ctor_builds_valid_instance()
        {
            // Act
            Link link = new();

            // Assert
            link.Href.Should()
                     .BeNull();
            link.Method.Should()
                       .BeNull();
            link.Relations.Should()
                         .BeEmpty();
            link.Template.Should()
                         .BeNull();
            link.Title.Should()
                      .BeNull();
            link.Relations.Should().BeEmpty();
        }

        [Theory]
        [InlineData("a/link/", false, "a relative link with no placeholder")]
        [InlineData("a/link/{id}", true, "the relative link contains a placeholder")]
        [InlineData("a/link/?id={id}", true, "the relative link contains a placeholder in its query string")]
        public void Compute_template_correctly(string href, bool expected, string reason)
        {
            // Arrange

            // Act
            Link link = new() { Href = href };

            // Assert
            link.Template.Should()
                         .Be(expected, reason);
        }

        [Fact]
        public void Given_existing_link_already_relations_When_adding_a_relation_that_already_exists_Then_there_should_be_no_duplicates()
        {
            // Arrange
            string relation = Faker.Lorem.Word();

            // Act
#if NET5_0_OR_GREATER
            Link link = new() { Relations = new HashSet<string> { relation, relation } };
#else
            Link link = new() { Relations = new[] { relation, relation } }; 
#endif

            // Assert
            link.Relations.Should()
                          .HaveCount(1).And
                          .OnlyContain(rel => rel == relation);

        }

    }
}