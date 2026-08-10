using System.Reflection;
using Xunit;
using AwesomeAssertions;
using Thuria.Zitidar.Extensions;
using Lithan.Core.TestUtilities.UnitTests.Fakes;

namespace Lithan.Core.TestUtilities.UnitTests;

public class ConstructorTestHelperTests
{
    [Fact]
    public void ConstructObject_GivenGenericType_ShouldNotThrowExceptionAndConstructObject()
    {
        //  Arrange
        FakeTestClass? testClass = null;

        //  Act
        var exception = Record.Exception(() => testClass = ConstructorTestHelper.ConstructObject<FakeTestClass>());

        //  Assert
        exception.Should().BeNull();
        testClass.Should().NotBeNull();
        testClass.Should().BeOfType<FakeTestClass>();
    }

    [Theory]
    [InlineData("TestDictionary", typeof(Dictionary<string, string>))]
    [InlineData("TestDictionary2", typeof(Dictionary<string, object>))]
    public void ConstructObject_GivenGenericType_ShouldNotThrowExceptionAndConstructObjectWithValuesAsExpected(string propertyName, Type expectedType)
    {
        //  Arrange
        FakeTestClass? testClass = null;

        //  Act
        var exception = Record.Exception(() => testClass = ConstructorTestHelper.ConstructObject<FakeTestClass>());

        //  Assert
        exception.Should().BeNull();
        testClass.Should().NotBeNull();

        var propertyValue = testClass?.GetPropertyValue(propertyName);
        propertyValue.Should().NotBeNull();
        propertyValue.Should().BeOfType(expectedType);
    }

    [Fact]
    public void ConstructObject_GivenGenericAndParameterAndNoValue_ShouldConstructObjectWithNullValue()
    {
        //  Arrange

        //  Act
        var testClass = ConstructorTestHelper.ConstructObject<FakeTestClass>("allFakes");

        //  Assert
        testClass.Should().NotBeNull();
        testClass.FakeList.Should().BeNullOrEmpty();
    }

    [Fact]
    public void ConstructObject_GivenGenericAndParameterAndValue_ShouldConstructObjectWithExpectedValue()
    {
        //  Arrange
        var parameterValue = "FakeTestName";

        //  Act
        var testClass = ConstructorTestHelper.ConstructObject<FakeTestClass>("testName", parameterValue);

        //  Assert
        testClass.Should().NotBeNull();
        testClass.Name.Should().Be(parameterValue);
    }

    [Fact]
    public void ConstructObject_GivenType_ShouldNotThrowExceptionAndConstructObject()
    {
        //  Arrange
        FakeTestClass? testClass = null;

        //  Act
        var exception = Record.Exception(() => testClass = ConstructorTestHelper.ConstructObject(typeof(FakeTestClass)) as FakeTestClass);

        //  Assert
        exception.Should().BeNull();
        testClass.Should().NotBeNull();
    }

    [Fact]
    public void ConstructObject_GivenTypeAndParameterAndNoValue_ShouldConstructObjectWithNullValue()
    {
        //  Arrange

        //  Act
        var testClass = ConstructorTestHelper.ConstructObject(typeof(FakeTestClass), "allFakes") as FakeTestClass;

        //  Assert
        testClass.Should().NotBeNull();
        testClass.FakeList.Should().BeNullOrEmpty();
    }

    [Fact]
    public void ConstructObject_GivenTypeAndParameterAndValue_ShouldConstructObjectWithExpectedValue()
    {
        //  Arrange
        var parameterValue = "FakeTestName";

        //  Act
        var testClass = ConstructorTestHelper.ConstructObject(typeof(FakeTestClass), "testName", parameterValue) as FakeTestClass;

        //  Assert
        testClass.Should().NotBeNull();
        testClass.Name.Should().Be(parameterValue);
    }

    [Fact]
    public void ConstructObject_GivenParameterValues_ShouldConstructObjectWithParameterValues()
    {
        //  Arrange
        var testDateTime = DateTime.Now;
        var fakeComplex  = new FakeComplex();
        var parameterValues = new List<(string paramName, object? paramValue)>
        {
            ("testDateTime", testDateTime), ("complexObject", fakeComplex)
        };

        //  Act
        var testClass = ConstructorTestHelper.ConstructObject<FakeTestClass>(constructorParams: parameterValues.ToArray());

        //  Assert
        testClass.Should().NotBeNull();
        testClass.TestDateTime.Should().BeSameDateAs(testDateTime);
        testClass.ComplexObject2.Should().Be(fakeComplex);
    }

    [Fact]
    public void ConstructObject_GivenException_ShouldConstructObjectAndNotThrowException()
    {
        //  Arrange

        //  Act
        var exception = Record.Exception(() =>
                                         {
                                             var fakeException = ConstructorTestHelper.ConstructObject<FakeException>();

                                             //  Assert
                                             fakeException.Should().NotBeNull();
                                         });

        // Assert
        exception.Should().BeNull();
    }

    [Fact]
    public void ConstructObject_GivenArray_ShouldNotThrowException()
    {
        //  Arrange

        //  Act
        var exception = Record.Exception(() => ConstructorTestHelper.ConstructObject<FakeTestClass2>());

        //  Assert
        exception.Should().BeNull();
    }

    [Theory]
    [InlineData("fakeComplex")]
    [InlineData("fakeClass")]
    public void ConstructObject_GivenObjectWithMultipleConstructors_ShouldConstructUsingMatchingConstructor(string parameterName)
    {
        //  Arrange

        //  Act
        var exception = Assert.Throws<TargetInvocationException>(() => ConstructorTestHelper.ConstructObject<FakeTestClass3>(parameterName));

        //  Assert
        exception.Should().NotBeNull();
        exception.InnerException.Should().BeOfType<ArgumentNullException>();
        (exception.InnerException as ArgumentNullException)?.ParamName.Should().Be(parameterName);
    }

    [Fact]
    public void ConstructObject_GivenNonZeroParameter_ShouldConstructCorrectly()
    {
        //  Arrange

        //  Act
        var exception = Record.Exception(() => ConstructorTestHelper.ConstructObject<FakeTestClass4>());

        //  Assert
        exception.Should().BeNull();
    }

    [Fact]
    public void ConstructObject_GivenAllParametersMustMatch_ShouldConstructCorrectly()
    {
        //  Arrange

        //  Act
        var exception = Record.Exception(() => ConstructorTestHelper.ConstructObject<FakeTestClass4>(null, null, true, ("someTestValue", 200)));

        //  Assert
        exception.Should().BeNull();
    }
}