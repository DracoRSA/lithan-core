using Xunit;
using Xunit.Sdk;
using AwesomeAssertions;
using Lithan.Core.TestUtilities.XUnit;
using Lithan.Core.TestUtilities.UnitTests.Fakes;

namespace Lithan.Core.TestUtilities.UnitTests;

public class XUnitAttributeTestHelperTests
{
    [Fact]
    public void ValidateMethodAttributes_GivenGenericAttributeAndMethodDecorated_ShouldPassTest()
    {
        // Arrange

        // Act
        var exception = Record.Exception(() => XUnitAttributeTestHelper.ValidateMethodAttributes<FakeTestClass, FakeTestAttribute>("TestMethod1"));

        // Assert
        exception.Should().BeNull();
    }

    [Fact]
    public void ValidateMethodAttributes_GivenGenericAttributeAndMethodNotDecorated_ShouldFailTest()
    {
        // Arrange

        // Act
        var exception = Assert.Throws<XunitException>(() => XUnitAttributeTestHelper.ValidateMethodAttributes<FakeTestClass, FakeTestAttribute>("TestMethod2"));

        // Assert
        exception.Message.Should().Contain("Expected method TestMethod2 to have attribute FakeTestAttribute");
    }

    [Fact]
    public void ValidateMethodAttributes_GivenGenericAttributeAndMethodDoesNotExist_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var methodName = "MissingMethod";

        // Act
        var exception = Assert.Throws<InvalidOperationException>(() => XUnitAttributeTestHelper.ValidateMethodAttributes<FakeTestClass, FakeTestAttribute>(methodName));

        // Assert
        exception.Message.Should().Be($"Method {methodName} not found in {nameof(FakeTestClass)}");
    }

    [Fact]
    public void ValidateMethodAttributes_GivenAttributeTypeAndMethodDecorated_ShouldPassTest()
    {
        // Arrange

        // Act
        var exception = Record.Exception(() => XUnitAttributeTestHelper.ValidateMethodAttributes<FakeTestClass>("TestMethod1", typeof(FakeTestAttribute)));

        // Assert
        exception.Should().BeNull();
    }

    [Fact]
    public void ValidateMethodAttributes_GivenAttributeTypeAndMethodNotDecorated_ShouldFailTest()
    {
        // Arrange

        // Act
        var exception = Assert.Throws<XunitException>(() => XUnitAttributeTestHelper.ValidateMethodAttributes<FakeTestClass>("TestMethod2", typeof(FakeTestAttribute)));

        // Assert
        exception.Message.Should().Contain("Expected method TestMethod2 to have attribute FakeTestAttribute");
    }

    [Fact]
    public void ValidateMethodAttributes_GivenAttributeTypeAndMethodDoesNotExist_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var methodName = "MissingMethod";

        // Act
        var exception = Assert.Throws<InvalidOperationException>(() => XUnitAttributeTestHelper.ValidateMethodAttributes<FakeTestClass>(methodName, typeof(FakeTestAttribute)));

        // Assert
        exception.Message.Should().Be($"Method {methodName} not found in {nameof(FakeTestClass)}");
    }

    [Fact]
    public void ValidateMethodAttributes_GivenAttributePropertyValue_ShouldPassTest()
    {
        // Arrange

        // Act
        var exception = Record.Exception(() => XUnitAttributeTestHelper.ValidateMethodAttributes<FakeTestClass>("TestMethod1",
                                                                                                               typeof(FakeTestAttribute),
                                                                                                               ("PropertyName", "TestMethod1")));

        // Assert
        exception.Should().BeNull();
    }

    [Fact]
    public void ValidateMethodAttributes_GivenMatchingSequencePropertyValue_ShouldPassTest()
    {
        // Arrange

        // Act
        var exception = Record.Exception(() => XUnitAttributeTestHelper.ValidateMethodAttributes<FakeTestClass>("TestMethod1",
                                                                                                               typeof(FakeTestAttribute),
                                                                                                               ("Sequence", 5)));

        // Assert
        exception.Should().BeNull();
    }

    [Fact]
    public void ValidateMethodAttributes_GivenAttributeNotPresent_ShouldThrowInvalidOperationException()
    {
        // Arrange

        // Act
        var exception = Assert.Throws<InvalidOperationException>(() => XUnitAttributeTestHelper.ValidateMethodAttributes<FakeTestClass>("TestMethod2",
                                                                                                                                       typeof(FakeTestAttribute),
                                                                                                                                       ("PropertyName", "TestMethod2")));

        // Assert
        exception.Message.Should().Be($"Attribute {nameof(FakeTestAttribute)} not found on method TestMethod2");
    }

    [Fact]
    public void ValidateMethodAttributes_GivenUnknownAttributeProperty_ShouldThrowInvalidOperationException()
    {
        // Arrange

        // Act
        var exception = Assert.Throws<InvalidOperationException>(() => XUnitAttributeTestHelper.ValidateMethodAttributes<FakeTestClass>("TestMethod1",
                                                                                                                                       typeof(FakeTestAttribute),
                                                                                                                                       ("UnknownProperty", "value")));

        // Assert
        exception.Message.Should().Be($"Property UnknownProperty not found on attribute {nameof(FakeTestAttribute)}");
    }

    [Fact]
    public void ValidateMethodAttributes_GivenUnexpectedAttributePropertyValue_ShouldFailTest()
    {
        // Arrange

        // Act
        var exception = Assert.Throws<FailException>(() => XUnitAttributeTestHelper.ValidateMethodAttributes<FakeTestClass>("TestMethod1",
                                                                                                                            typeof(FakeTestAttribute),
                                                                                                                            ("Sequence", 99)));

        // Assert
        exception.Message.Should().Be("Expected property Sequence to be 99, but was no such value found");
    }

    [Fact]
    public void ValidateMethodAttributes_GivenPropertyValueAndMethodDoesNotExist_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var methodName = "MissingMethod";

        // Act
        var exception = Assert.Throws<InvalidOperationException>(() => XUnitAttributeTestHelper.ValidateMethodAttributes<FakeTestClass>(methodName,
                                                                                                                                       typeof(FakeTestAttribute),
                                                                                                                                       ("PropertyName", "value")));

        // Assert
        exception.Message.Should().Be($"Method {methodName} not found in {nameof(FakeTestClass)}");
    }
}
