using Xunit.Categories;
using FsCheck.Xunit;
using FsCheck;

namespace Forms.UnitTests
{
    [UnitTest]
    public class FormFieldOptionTests
    {
        public void Ctor_should_set_properties(string label, object value)
        {
            // Arrange
            FormFieldOption option = new(label, value);

            // Assert
            Equals(option.Label, label).Label("Label")
                .And(Equals(option.Value, value)).Label("Value");
        }
    }
}