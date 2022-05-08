using FluentAssertions;

using Xunit;
using Xunit.Categories;

namespace Forms.UnitTests
{
    [Feature(nameof(Forms))]
    [Feature(nameof(Link))]
    public class LinkTests
    {
        [Fact]
        public void Ctor_builds_valid_instance()
        {
            // Act
            Link link = new Link();

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
        }

        [Theory]
        [InlineData("a/link/", false, "a relative link with no placeholder" )]
        [InlineData("a/link/{id}", true, "the relative link contains a placeholder" )]
        public void Compute_template_correctly(string href, bool expected, string reason)
        {
            // Arrange

            // Act
            Link link = new Link { Href = href };

            // Assert
            link.Template.Should()
                         .Be(expected, reason);
        }
    }
}