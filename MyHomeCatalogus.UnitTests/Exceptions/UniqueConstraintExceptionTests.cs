using MyHomeCatalogus.Shared.Exceptions;
using Xunit;

namespace MyHomeCatalogus.UnitTests.Exceptions
{
    public class UniqueConstraintExceptionTests
    {
        [Fact]
        public void Constructor_WithList_ShouldPopulateValidationErrors()
        {
            var message = "Validation failed";
            var errors = new List<(string PropertyName, string ErrorMessage)>
            {
                ("Name", "Name must be unique"),
                ("Code", "Code already exists")
            };

            var exception = new UniqueConstraintException(message, errors);

            Assert.Equal(message, exception.Message);
            Assert.Equal(2, exception.ValidationErrors.Count);
            Assert.Equal("Name", exception.ValidationErrors[0].PropertyName);
        }

        [Fact]
        public void Constructor_WithSingleError_ShouldPopulateValidationErrors()
        {
            var propName = "Sku";
            var errorMsg = "Duplicate SKU detected";

            var exception = new UniqueConstraintException(propName, errorMsg);

            Assert.Single(exception.ValidationErrors);
            Assert.Equal(propName, exception.ValidationErrors[0].PropertyName);
            Assert.Equal(errorMsg, exception.ValidationErrors[0].ErrorMessage);
            Assert.Equal(errorMsg, exception.Message);
        }

        [Fact]
        public void DefaultConstructor_ShouldInitializeEmptyList()
        {
            var exception = new UniqueConstraintException();

            Assert.NotNull(exception.ValidationErrors);
            Assert.Empty(exception.ValidationErrors);
        }
    }
}