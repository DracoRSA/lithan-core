using System.ComponentModel.DataAnnotations;
using Xunit;
using Xunit.Sdk;
using AwesomeAssertions;
using Lithan.Core.TestUtilities.XUnit;
using Lithan.Core.TestUtilities.UnitTests.Fakes;

namespace Lithan.Core.TestUtilities.UnitTests;

public class XUnitPropertyTestHelperTests
{
    [Fact]
    public void ValidateGetAndSet_GivenInstanceAndWritableProperty_ShouldPassTest()
    {
        // Arrange
        var objectUnderTest = new FakeComplex();

        // Act
        var exception = Record.Exception(() => objectUnderTest.ValidateGetAndSet(nameof(FakeComplex.Name)));

        // Assert
        exception.Should().BeNull();
    }

    [Fact]
    public void ValidateGetAndSet_GivenNullPropertyName_ShouldThrowArgumentNullException()
    {
        // Arrange
        var objectUnderTest = new FakeComplex();

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => objectUnderTest.ValidateGetAndSet(null!));

        // Assert
        exception.ParamName.Should().Be("propertyName");
    }

    [Fact]
    public void ValidateGetAndSet_GivenPropertyThatDoesNotExist_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var objectUnderTest = new FakeComplex();
        var propertyName    = "MissingProperty";

        // Act
        var exception = Assert.Throws<InvalidOperationException>(() => objectUnderTest.ValidateGetAndSet(propertyName));

        // Assert
        exception.Message.Should().Contain($"Property [{propertyName}] does not exists on");
    }

    [Theory]
    [InlineData(nameof(FakeComplex.Id))]
    [InlineData(nameof(FakeComplex.Name))]
    public void ValidateGetAndSet_GivenGenericTypeAndWritableProperty_ShouldPassTest(string propertyName)
    {
        // Arrange

        // Act
        var exception = Record.Exception(() => XUnitPropertyTestHelper.ValidateGetAndSet<FakeComplex>(propertyName));

        // Assert
        exception.Should().BeNull();
    }

    [Theory]
    [InlineData("TestDateTime", typeof(FakeTestAttribute))]
    [InlineData("ComplexObject1", typeof(FakeTestAttribute))]
    [InlineData("FakeList", typeof(FakeTestAttribute))]
    public void ValidateDecoratedWithAttribute_GivenPropertyNotDecorated_ShouldFailTest(string propertyName, Type attributeType)
    {
        // Arrange

        // Act
        var exception = Assert.Throws<FailException>(() => XUnitPropertyTestHelper.ValidateDecoratedWithAttribute<FakeTestClass>(propertyName, attributeType));

        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().Contain($"Property {propertyName} is not decorated with {attributeType.Name} Attribute");
    }

    [Theory]
    [InlineData("ComplexObject2", typeof(FakeTestAttribute))]
    [InlineData("TestDictionary", typeof(FakeTestAttribute))]
    [InlineData("ComplexObject1", typeof(RequiredAttribute))]
    public void ValidateDecoratedWithAttribute_GivenPropertyDecorated_ShouldPassTest(string propertyName, Type attributeType)
    {
        // Arrange

        // Act
        var exception = Record.Exception(() => XUnitPropertyTestHelper.ValidateDecoratedWithAttribute<FakeTestClass>(propertyName, attributeType));

        // Assert
        exception.Should().BeNull();
    }

    [Fact]
    public void ValidateDecoratedWithAttribute_GivenPropertyDecoratedWithAttributeWithExpectedParameters_ShouldPassTest()
    {
        // Arrange

        // Act
        var exception = Record.Exception(() => XUnitPropertyTestHelper.ValidateDecoratedWithAttribute<FakeTestClass>("TestDictionary",
                                                                                                                     typeof(FakeTestAttribute),
                                                                                                                     [
                                                                                                                         ("PropertyName", "TestDictionary"),
                                                                                                                         ("Sequence", 23)
                                                                                                                     ]));

        // Assert
        exception.Should().BeNull();
    }

    [Fact]
    public void ValidateDecoratedWithAttribute_GivenPropertyDecoratedWithAttributeButPropertyDoesNotExist_ShouldFailTest()
    {
        // Arrange

        // Act
        var exception = Assert.Throws<FailException>(() => XUnitPropertyTestHelper.ValidateDecoratedWithAttribute<FakeTestClass>("TestDictionary",
                                                                                                                                 typeof(FakeTestAttribute),
                                                                                                                                 [
                                                                                                                                     ("PropertyName", "TestDictionary"),
                                                                                                                                     ("Sequence", 23),
                                                                                                                                     ("UnknownProperty", "SomeValue")
                                                                                                                                 ]));

        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().Contain("TestDictionary Property is decorated with FakeTestAttribute " +
                                           "but the attribute property UnknownProperty does not exist on the attribute");
    }

    [Fact]
    public void ValidateDecoratedWithAttribute_GivenPropertyDecoratedWithAttributeButPropertyValueIsNotExpected_ShouldFailTest()
    {
        // Arrange

        // Act
        var exception = Assert.Throws<FailException>(() => XUnitPropertyTestHelper.ValidateDecoratedWithAttribute<FakeTestClass>("TestDictionary",
                                                                                                                                 typeof(FakeTestAttribute),
                                                                                                                                 [
                                                                                                                                     ("PropertyName", "TestDictionary"),
                                                                                                                                     ("Sequence", 55)
                                                                                                                                 ]));

        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().Contain("TestDictionary Property is decorated with FakeTestAttribute " +
                                           "but the attribute property Sequence is not set to 55");
    }

    [Fact]
    public void ValidateDecoratedWithAttribute_GivenNullPropertyName_ShouldThrowArgumentNullException()
    {
        // Arrange

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => XUnitPropertyTestHelper.ValidateDecoratedWithAttribute<FakeTestClass>(null!, typeof(FakeTestAttribute)));

        // Assert
        exception.ParamName.Should().Be("propertyName");
    }

    [Fact]
    public void ValidateDecoratedWithAttribute_GivenNullAttributeType_ShouldThrowArgumentNullException()
    {
        // Arrange

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => XUnitPropertyTestHelper.ValidateDecoratedWithAttribute<FakeTestClass>("TestDictionary", null!));

        // Assert
        exception.ParamName.Should().Be("attributeType");
    }

    [Fact]
    public void ValidateDecoratedWithAttribute_GivenPropertyThatDoesNotExist_ShouldFailTest()
    {
        // Arrange
        var propertyName = "MissingProperty";

        // Act
        var exception = Assert.Throws<FailException>(() => XUnitPropertyTestHelper.ValidateDecoratedWithAttribute<FakeTestClass>(propertyName, typeof(FakeTestAttribute)));

        // Assert
        exception.Message.Should().Contain($"Property [{propertyName}] does not exists on");
    }
}
