using FluentAssertions;
using FluentAssertions.Execution;
using FsCheck;
using FsCheck.Xunit;
using Xunit.Categories;

namespace Candoumbe.Forms.UnitTests;

[UnitTest]
public class FormFieldOptionTests
{
    [Property]
    public void Ctor_should_set_properties(NonEmptyString labelGenerator, object value)
    {
        // Arrange
        string label = labelGenerator.Item;
            
        // Act
        FormFieldOption option = new(label, value);

        // Assert
        using AssertionScope assertionScope = new();
        option.Label.Should().Be(label);
        option.Value.Should().Be(value);
    }
}