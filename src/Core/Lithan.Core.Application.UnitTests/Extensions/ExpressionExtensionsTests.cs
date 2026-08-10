using System.Linq.Expressions;
using Xunit;
using AwesomeAssertions;
using Lithan.Core.Application.Extensions;
using Lithan.Core.Application.UnitTests.Fakes;

namespace Lithan.Core.Application.UnitTests.Extensions;

public class ExpressionExtensionsTests
{
    [Fact]
    public void CreateEqualsCondition_GivenMatchingValue_ShouldReturnTrue()
    {
        // Arrange
        var entity = new FakeEntity { Age = 30 };

        // Act
        var (parameter, predicate) = typeof(FakeEntity).CreateEqualsCondition("Age", 30);
        var compiled = Compile(parameter, predicate);

        // Assert
        compiled(entity).Should().BeTrue();
    }

    [Fact]
    public void CreateEqualsCondition_GivenNonMatchingValue_ShouldReturnFalse()
    {
        // Arrange
        var entity = new FakeEntity { Age = 30 };

        // Act
        var (parameter, predicate) = typeof(FakeEntity).CreateEqualsCondition("Age", 40);
        var compiled = Compile(parameter, predicate);

        // Assert
        compiled(entity).Should().BeFalse();
    }

    [Fact]
    public void CreateEqualsCondition_GivenNestedProperty_ShouldEvaluateNestedMember()
    {
        // Arrange
        var entity = new FakeEntity { Child = new FakeEntityChild { Name = "Nested" } };

        // Act
        var (parameter, predicate) = typeof(FakeEntity).CreateEqualsCondition("Child.Name", "Nested");
        var compiled = Compile(parameter, predicate);

        // Assert
        compiled(entity).Should().BeTrue();
    }

    [Fact]
    public void CreateEqualsCondition_GivenEnumValue_ShouldEvaluateEnum()
    {
        // Arrange
        var entity = new FakeEntity { Status = FakeEntityStatus.Active };

        // Act
        var (parameter, predicate) = typeof(FakeEntity).CreateEqualsCondition("Status", FakeEntityStatus.Active);
        var compiled = Compile(parameter, predicate);

        // Assert
        compiled(entity).Should().BeTrue();
    }

    [Fact]
    public void CreateEqualsCondition_GivenGuidString_ShouldConvertAndEvaluate()
    {
        // Arrange
        var id     = Guid.NewGuid();
        var entity = new FakeEntity { Id = id };

        // Act
        var (parameter, predicate) = typeof(FakeEntity).CreateEqualsCondition("Id", id.ToString());
        var compiled = Compile(parameter, predicate);

        // Assert
        compiled(entity).Should().BeTrue();
    }

    [Fact]
    public void CreateEqualsCondition_GivenEmptyPropertyPath_ShouldThrowArgumentException()
    {
        // Arrange

        // Act
        var exception = Assert.Throws<ArgumentException>(() => typeof(FakeEntity).CreateEqualsCondition("  ", 1));

        // Assert
        exception.ParamName.Should().Be("propertyPath");
    }

    [Fact]
    public void CreateGreaterThanOrEqualCondition_GivenValueOnBoundary_ShouldReturnTrue()
    {
        // Arrange
        var entity = new FakeEntity { Age = 21 };

        // Act
        var (parameter, predicate) = typeof(FakeEntity).CreateGreaterThanOrEqualCondition("Age", 21);
        var compiled = Compile(parameter, predicate);

        // Assert
        compiled(entity).Should().BeTrue();
    }

    [Fact]
    public void CreateLessThanOrEqualCondition_GivenValueAboveBoundary_ShouldReturnFalse()
    {
        // Arrange
        var entity = new FakeEntity { Age = 30 };

        // Act
        var (parameter, predicate) = typeof(FakeEntity).CreateLessThanOrEqualCondition("Age", 21);
        var compiled = Compile(parameter, predicate);

        // Assert
        compiled(entity).Should().BeFalse();
    }

    [Fact]
    public void CreateContainsCondition_GivenMatchingString_ShouldReturnTrue()
    {
        // Arrange
        var entity = new FakeEntity { Name = "Lithan Solutions" };

        // Act
        var (parameter, predicate) = typeof(FakeEntity).CreateContainsCondition("Name", "Solutions");
        var compiled = Compile(parameter, predicate);

        // Assert
        compiled(entity).Should().BeTrue();
    }

    [Fact]
    public void CreateContainsCondition_GivenNullPropertyValue_ShouldReturnFalse()
    {
        // Arrange
        var entity = new FakeEntity { Name = null };

        // Act
        var (parameter, predicate) = typeof(FakeEntity).CreateContainsCondition("Name", "Solutions");
        var compiled = Compile(parameter, predicate);

        // Assert
        compiled(entity).Should().BeFalse();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CreateContainsCondition_GivenBlankSearchValue_ShouldReturnTrue(string? searchValue)
    {
        // Arrange
        var entity = new FakeEntity { Name = null };

        // Act
        var (parameter, predicate) = typeof(FakeEntity).CreateContainsCondition("Name", searchValue);
        var compiled = Compile(parameter, predicate);

        // Assert
        compiled(entity).Should().BeTrue();
    }

    [Fact]
    public void CreateContainsCondition_GivenNonStringProperty_ShouldThrowArgumentException()
    {
        // Arrange

        // Act
        var exception = Assert.Throws<ArgumentException>(() => typeof(FakeEntity).CreateContainsCondition("Age", "10"));

        // Assert
        exception.ParamName.Should().Be("propertyName");
        exception.Message.Should().Contain("string property");
    }

    [Fact]
    public void AddContainsFilterCondition_GivenBlankSearchValue_ShouldReturnOriginalPredicate()
    {
        // Arrange
        var parameter = Expression.Parameter(typeof(FakeEntity), "e");
        var original  = Expression.Constant(true);

        // Act
        var result = original.AddContainsFilterCondition(parameter, "Name", " ");

        // Assert
        result.Should().BeSameAs(original);
    }

    [Fact]
    public void AddContainsFilterCondition_GivenSearchValue_ShouldAndContainsCheck()
    {
        // Arrange
        var parameter = Expression.Parameter(typeof(FakeEntity), "e");
        var original  = Expression.Constant(true);
        var entity    = new FakeEntity { Name = "Lithan Core" };

        // Act
        var predicate = original.AddContainsFilterCondition(parameter, "Name", "Core");
        var compiled  = Compile(parameter, predicate);

        // Assert
        compiled(entity).Should().BeTrue();
    }

    [Fact]
    public void AddContainsFilterCondition_Generic_GivenPredicateWithoutParameter_ShouldThrowArgumentNullException()
    {
        // Arrange
        Expression predicate = Expression.Constant(true);

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => predicate.AddContainsFilterCondition<FakeEntity>(null, "Name", "Core"));

        // Assert
        exception.ParamName.Should().Be("parameter");
    }

    [Fact]
    public void AddContainsFilterCondition_Generic_GivenNullPredicateAndSearchValue_ShouldCreateParameterAndPredicate()
    {
        // Arrange
        var entity = new FakeEntity { Name = "Lithan" };

        // Act
        var (parameter, predicate) = ((Expression?)null).AddContainsFilterCondition<FakeEntity>(null, "Name", "Lith");
        var compiled = Compile(parameter!, predicate!);

        // Assert
        parameter.Should().NotBeNull();
        compiled(entity).Should().BeTrue();
    }

    [Fact]
    public void AddEqualsFilterCondition_GivenNullSearchValue_ShouldReturnOriginalPredicate()
    {
        // Arrange
        var parameter = Expression.Parameter(typeof(FakeEntity), "e");
        var original  = Expression.Constant(true);

        // Act
        var result = original.AddEqualsFilterCondition(parameter, "Age", null);

        // Assert
        result.Should().BeSameAs(original);
    }

    [Fact]
    public void AddEqualsFilterCondition_GivenValue_ShouldFilterMatchingEntities()
    {
        // Arrange
        var parameter = Expression.Parameter(typeof(FakeEntity), "e");
        var original  = Expression.Constant(true);
        var entity    = new FakeEntity { Age = 42 };

        // Act
        var predicate = original.AddEqualsFilterCondition(parameter, "Age", 42);
        var compiled  = Compile(parameter, predicate);

        // Assert
        compiled(entity).Should().BeTrue();
    }

    [Fact]
    public void AddGreaterThanOrEqualFilterCondition_Generic_GivenValue_ShouldEvaluate()
    {
        // Arrange
        var entity = new FakeEntity { Age = 50 };

        // Act
        var (parameter, predicate) = ((Expression?)null).AddGreaterThanOrEqualFilterCondition<FakeEntity>(null, "Age", 40);
        var compiled = Compile(parameter!, predicate!);

        // Assert
        compiled(entity).Should().BeTrue();
    }

    [Fact]
    public void AddLessThanOrEqualFilterCondition_Generic_GivenValueAboveBoundary_ShouldReturnFalse()
    {
        // Arrange
        var entity = new FakeEntity { Age = 50 };

        // Act
        var (parameter, predicate) = ((Expression?)null).AddLessThanOrEqualFilterCondition<FakeEntity>(null, "Age", 40);
        var compiled = Compile(parameter!, predicate!);

        // Assert
        compiled(entity).Should().BeFalse();
    }

    [Fact]
    public void AddEqualsFilterCondition_Generic_GivenPredicateWithoutParameter_ShouldThrowArgumentNullException()
    {
        // Arrange
        Expression predicate = Expression.Constant(true);

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() => predicate.AddEqualsFilterCondition<FakeEntity>(null, "Age", 10));

        // Assert
        exception.ParamName.Should().Be("parameter");
    }

    [Fact]
    public void CreateEqualsCondition_GivenIncompatibleValue_ShouldThrowArgumentException()
    {
        // Arrange

        // Act
        var exception = Assert.Throws<ArgumentException>(() => typeof(FakeEntity).CreateEqualsCondition("Age", "not-a-number"));

        // Assert
        exception.ParamName.Should().Be("value");
    }

    private static Func<FakeEntity, bool> Compile(ParameterExpression parameter, Expression predicate)
    {
        return Expression.Lambda<Func<FakeEntity, bool>>(predicate, parameter).Compile();
    }
}
