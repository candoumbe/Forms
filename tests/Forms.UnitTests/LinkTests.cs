using FluentAssertions;

using Xunit;

namespace Forms.UnitTests
{
    public class LinkTests
    {

        [Theory]
        [InlineData("a/link/", false, "a relative link with no placeholder" )]
        [InlineData("a/link/{id}", true, "the relative link contains a placeholder" )]
        public void Compute_template_correctly(string href, bool expected, string reason)
        {
            // Arrange
            Link link;

            // Act
            link = new Link { Href = href };

            // Assert
            link.Template.Should()
                         .Be(expected, reason);
        }
    }
}
