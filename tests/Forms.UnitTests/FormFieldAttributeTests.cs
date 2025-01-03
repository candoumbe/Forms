﻿using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;
using Xunit.Categories;

namespace Candoumbe.Forms.UnitTests;

[UnitTest]
[Feature("REST")]
public class FormFieldAttributeTests(ITestOutputHelper outputHelper)
{
    [Fact]
    public void Ctor_Should_Build_Valid_Instance()
    {
        // Act
        FormFieldAttribute attribute = new();

        // Assert
        attribute.Type.Should().Be(FormFieldType.String);
        attribute.Enabled.Should().BeFalse();
        attribute.Required.Should().BeFalse();
        attribute.Pattern.Should().BeNull();
        attribute.Relations.Should()
            .BeAssignableTo<IEnumerable<string>>().And
            .BeEmpty();
        attribute.Min.Should().Be(0);
        attribute.MinLength.Should().Be(0);

        attribute.Max.Should().Be(0);
        attribute.MaxLength.Should().Be(0);

        attribute.MinSize.Should().Be(0);
        attribute.MaxSize.Should().Be(0);
    }

    [Fact]
    public void IsValid() => typeof(FormFieldAttribute).Should()
        .BeDecoratedWith<AttributeUsageAttribute>(attr =>
            attr.AllowMultiple
            && attr.Inherited
            && attr.ValidOn == AttributeTargets.Property
        );
}