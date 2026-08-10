using Xunit;
using Xunit.Sdk;
using AwesomeAssertions;
using Lithan.Core.TestUtilities.XUnit;
using Lithan.Core.TestUtilities.UnitTests.Fakes;

namespace Lithan.Core.TestUtilities.UnitTests;

public class XUnitConstructorTestHelperTests
{
    [Fact]
    public void ValidateArgumentNullExceptionIfParameterIsNull_GivenParameterWhereExceptionIsNotThrown_ShouldFailTest()
    {
        //  Arrange
        var parameterName = "complexObjectNotTested";
    
        //  Act
        var exception = Assert.Throws<FailException>(() => XUnitConstructorTestHelper.ValidateArgumentNullExceptionIfParameterIsNull<FakeTestClass>(parameterName));

        //  Assert
        exception.Message.Should().Be($"ArgumentNullException not throw for Constructor Parameter [{parameterName}] on {typeof(FakeTestClass).FullName}");
    }

    [Theory]
    [InlineData("testName")]
    [InlineData("complexObject")]
    [InlineData("complexInterface")]
    [InlineData("testDictionary")]
    [InlineData("testDictionary2")]
    public void ValidateArgumentNullExceptionIfParameterIsNull_GivenParameterWhereExceptionIsThrown_ShouldPassTest(string parameterName)
    {
        //  Arrange
    
        //  Act
        var exception = Record.Exception(() => XUnitConstructorTestHelper.ValidateArgumentNullExceptionIfParameterIsNull<FakeTestClass>(parameterName));

        //  Assert
        exception.Should().BeNull();
    }
    
    [Fact]
    public void ValidateArgumentNullExceptionIfParameterIsNull_GivenParameterValuesAndExceptionNotThrown_ShouldFailTest()
    {
        //  Arrange
        var parameterName = "notSetParameter";
        var testDateTime  = DateTime.Now;
        var fakeComplex   = new FakeComplex();
    
        var parameterValues = new List<(string paramName, object? paramValue)>
                              {
                                  ("testDateTime", testDateTime), ("complexObject", fakeComplex)
                              };
        //  Act
        var exception = Assert.Throws<FailException>(() => XUnitConstructorTestHelper.ValidateArgumentNullExceptionIfParameterIsNull<FakeTestClass>(parameterName,
                                                                                                                                                                        parameterValues.ToArray()));
    
        //  Assert
        exception.Message.Should().Contain($"ArgumentNullException not throw for Constructor Parameter [{parameterName}] on {typeof(FakeTestClass).FullName}");
    }
    
    [Theory]
    [InlineData("testName")]
    [InlineData("complexObject")]
    [InlineData("complexInterface")]
    public void ValidateArgumentNullExceptionIfParameterIsNull_GivenParameterWhereExceptionIsThrownAndParameterValues_ShouldPassTest(string parameterName)
    {
        //  Arrange
    
        //  Act
        var exception = Record.Exception(() => XUnitConstructorTestHelper.ValidateArgumentNullExceptionIfParameterIsNull<FakeTestClass>(parameterName, ("testDateTime", DateTime.Now)));

        //  Assert
        exception.Should().BeNull();
    }
        
    [Fact]
    public void ValidatePropertySetWithParameter_GivenParameterNotSettingProperty_ShouldFailTest()
    {
        //  Arrange
        var parameterName = "notSetParameter";
        var propertyName  = "NotSetProperty";
    
        //  Act
        var exception = Assert.Throws<XunitException>(() => XUnitConstructorTestHelper.ValidatePropertySetWithParameter<FakeTestClass>(parameterName, propertyName));
    
        //  Assert
        exception.Message.Should().Contain($"because parameter [{parameterName}] of the constructor of [{typeof(FakeTestClass).FullName}] should set property [{propertyName}]");
    }
        
    [Fact]
    public void ValidatePropertySetWithParameter_GivenParameterNotSettingProperty_ShouldPassTest()
    {
        //  Arrange
        var parameterName = "complexObject";
        var propertyName  = "ComplexObject2";
    
        //  Act
        var exception = Record.Exception(() => XUnitConstructorTestHelper.ValidatePropertySetWithParameter<FakeTestClass>(parameterName, propertyName));

        //  Assert
        exception.Should().BeNull();
    }
    
    [Theory]
    [InlineData("testName")]
    [InlineData("complexObject")]
    [InlineData("complexInterface")]
    [InlineData("testDictionary")]
    [InlineData("testDictionary2")]
    public void ValidateExceptionIsThrownIfParameterIsNull_GivenParameterWhereExceptionIsThrown_ShouldPassTest(string parameterName)
    {
        //  Arrange
    
        //  Act
        var exception = Record.Exception(() => XUnitConstructorTestHelper.ValidateExceptionIsThrownIfParameterIsNull<FakeTestClass, ArgumentNullException>(parameterName));

        //  Assert
        exception.Should().BeNull();
    }
    
    [Fact]
    public void ValidateExceptionIsThrownIfParameterIsNull_GivenParameterValuesAndExceptionNotThrown_ShouldFailTest()
    {
        //  Arrange
        var parameterName = "notSetParameter";
        var testDateTime  = DateTime.Now;
        var fakeComplex   = new FakeComplex();
    
        var parameterValues = new List<(string paramName, object? paramValue)>
                              {
                                  ("testDateTime", testDateTime), ("complexObject", fakeComplex)
                              };
    
        //  Act
        var exception = Assert.Throws<FailException>(() => XUnitConstructorTestHelper.ValidateExceptionIsThrownIfParameterIsNull<FakeTestClass, ArgumentNullException>(parameterName,
                                                                                                                                                                       constructorParams: parameterValues.ToArray()));
    
        //  Assert
        exception.Message.Should().Contain($"ArgumentNullException Exception not throw for Constructor Parameter [{parameterName}] on {typeof(FakeTestClass).FullName}");
    }
    
    [Theory]
    [InlineData("testName")]
    [InlineData("complexObject")]
    [InlineData("complexInterface")]
    public void ValidateExceptionIsThrownIfParameterIsNull_GivenParameterWhereExceptionIsThrownAndParameterValues_ShouldPassTest(string parameterName)
    {
        //  Arrange
    
        //  Act
        var exception = Record.Exception(() => XUnitConstructorTestHelper.ValidateExceptionIsThrownIfParameterIsNull<FakeTestClass, ArgumentNullException>(parameterName,
                                                                                                                                                           constructorParams: ("testDateTime", DateTime.Now)));

        //  Assert
        exception.Should().BeNull();
    }

    [Fact]
    public void ValidateExceptionIsThrownIfParameterIsNull_GivenParameterWhereExceptionIsNotThrown_ShouldFailTest()
    {
        // Arrange
        var parameterName = "complexObjectNotTested";

        // Act
        var exception = Assert.Throws<FailException>(() => XUnitConstructorTestHelper.ValidateExceptionIsThrownIfParameterIsNull<FakeTestClass, ArgumentNullException>(parameterName));

        // Assert
        exception.Message.Should().Be($"ArgumentNullException Exception not throw for Constructor Parameter [{parameterName}] on {typeof(FakeTestClass).FullName}");
    }

    [Fact]
    public void ValidateExceptionIsThrownIfParameterIsNull_GivenWrongExceptionType_ShouldFailTest()
    {
        // Arrange
        var parameterName = "testName";

        // Act
        var exception = Assert.Throws<FailException>(() => XUnitConstructorTestHelper.ValidateExceptionIsThrownIfParameterIsNull<FakeTestClass, ArgumentOutOfRangeException>(parameterName));

        // Assert
        exception.Message.Should().Contain($"ArgumentOutOfRangeException Exception not throw for Constructor Parameter [{parameterName}] on {typeof(FakeTestClass).FullName}");
    }

    [Fact]
    public void ValidateArgumentNullExceptionIfParameterIsNull_GivenMultipleConstructors_ShouldUseMatchingConstructor()
    {
        // Arrange

        // Act
        var exception = Record.Exception(() => XUnitConstructorTestHelper.ValidateArgumentNullExceptionIfParameterIsNull<FakeTestClass3>("fakeComplex", ("testId", 1)));

        // Assert
        exception.Should().BeNull();
    }

    [Fact]
    public void ValidateExceptionIsThrownIfParameterIsNull_GivenAllParametersMatch_ShouldPassTest()
    {
        // Arrange

        // Act
        var exception = Record.Exception(() => XUnitConstructorTestHelper.ValidateExceptionIsThrownIfParameterIsNull<FakeTestClass3, ArgumentNullException>("fakeComplex",
                                                                                                                                                            true,
                                                                                                                                                            ("testId", 1),
                                                                                                                                                            ("fakeComplex", new FakeComplex())));

        // Assert
        exception.Should().BeNull();
    }

    [Theory]
    [InlineData("testName", "Name")]
    [InlineData("complexObject", "ComplexObject2")]
    public void ValidatePropertySetWithParameter_GivenParameterSettingProperty_ShouldPassTest(string parameterName, string propertyName)
    {
        // Arrange

        // Act
        var exception = Record.Exception(() => XUnitConstructorTestHelper.ValidatePropertySetWithParameter<FakeTestClass>(parameterName, propertyName));

        // Assert
        exception.Should().BeNull();
    }
}