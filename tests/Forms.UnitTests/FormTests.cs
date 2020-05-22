using FluentAssertions;
using Xunit;

namespace Forms.Tests
{
    public class FormTests
    {
        [Fact]
        public void Default_constructor_sets_items_to_empty() => new Form().Items
            .Should().BeEmpty();
    }
}
