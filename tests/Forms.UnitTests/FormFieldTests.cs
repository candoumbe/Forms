﻿using System.Linq;
using FluentAssertions;
using FsCheck;
using FsCheck.Xunit;
using Xunit;
using Xunit.Categories;

namespace Candoumbe.Forms.UnitTests;

[UnitTest]
public class FormFieldTests
{
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