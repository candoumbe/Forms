using FluentAssertions;

using FsCheck;
using FsCheck.Xunit;

using System.Linq;

using Xunit;
using Xunit.Abstractions;

namespace Forms.UnitTests
{
    public class FormFieldTests
    {
        private readonly ITestOutputHelper _outputHelper;

        public FormFieldTests(ITestOutputHelper outputHelper) => _outputHelper = outputHelper;

        [Fact]
        public void Ctor_build_valid_instance()
        {
            // Act
            FormField instance = new();

            // Assert
            instance.Description.Should().BeNull();
            instance.Enabled.Should().BeNull();
            instance.Label.Should().BeNull();
            instance.Max.Should().BeNull();
            instance.MaxLength.Should().BeNull();
            instance.MinLength.Should().BeNull();
            instance.Min.Should().BeNull();
            instance.Name.Should().BeNull();
            instance.Pattern.Should().BeNull();
            instance.Placeholder.Should().BeNull();
            instance.Required.Should().BeNull();
            instance.Secret.Should().BeNull();
            instance.Type.Should().Be(FormFieldType.String);
        }

        [Property]
        public Property Setting_Options_should_change_type_to_Array(NonEmptyArray<string> values)
        {
            // Act
            FormField field = new()
            {
                // Act
                Options = values.Item.Select(value => new FormFieldOption(value, value))
            };

            // Assert
            return (field.Type == FormFieldType.Array).ToProperty();
        }
    }
}
