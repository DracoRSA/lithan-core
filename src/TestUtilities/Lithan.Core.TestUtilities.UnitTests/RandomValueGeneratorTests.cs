using System.Collections.ObjectModel;
using Xunit;
using AwesomeAssertions;
using Lithan.Core.TestUtilities.UnitTests.Fakes;

namespace Lithan.Core.TestUtilities.UnitTests;

public class RandomValueGeneratorTests
{
    [Theory]
    [InlineData(1, 10)]
    [InlineData(20, 55)]
    [InlineData(255, 1024)]
    public void CreateRandomNumber_GivenMinimumAndMaximumValues_ShouldGenerateValueBetweenGivenValues(int minimumValue, int maximumValue)
    {
        //  Arrange

        //  Act
        var randomNumber = RandomValueGenerator.CreateRandomNumber(minimumValue, maximumValue);

        //  Assert
        randomNumber.Should().BeGreaterThanOrEqualTo(minimumValue);
        randomNumber.Should().BeLessThanOrEqualTo(maximumValue);
    }

    [Theory]
    [InlineData(1, 10)]
    [InlineData(20, 55)]
    [InlineData(255, 1024)]
    public void CreateRandomInt_GivenMinimumAndMaximumValues_ShouldGenerateValueBetweenGivenValues(int minimumValue, int maximumValue)
    {
        //  Arrange

        //  Act
        var randomNumber = RandomValueGenerator.CreateRandomInt(minimumValue, maximumValue);

        //  Assert
        randomNumber.Should().BeGreaterThanOrEqualTo(minimumValue);
        randomNumber.Should().BeLessThanOrEqualTo(maximumValue);
    }

    [Theory]
    [InlineData(1, 10)]
    [InlineData(20, 55)]
    [InlineData(100, 200)]
    [InlineData(255, 1024)]
    public void CreateRandomLong_GivenMinimumAndMaximumValues_ShouldGenerateValueBetweenGivenValues(int minimumValue, int maximumValue)
    {
        //  Arrange

        //  Act
        var randomNumber = RandomValueGenerator.CreateRandomLong(minimumValue, maximumValue);

        //  Assert
        randomNumber.Should().BeGreaterThanOrEqualTo(minimumValue);
        randomNumber.Should().BeLessThanOrEqualTo(maximumValue);
    }

    [Theory]
    [InlineData(1, 10)]
    [InlineData(7, 51)]
    [InlineData(100, 1024)]
    public void CreateRandomString_GivenMinimumAndMaximumValues_ShouldGenerateStringWithLenBetweenGivenValues(int minimumLength, int maximumLength)
    {
        //  Arrange

        //  Act
        var randomString = RandomValueGenerator.CreateRandomString(minimumLength, maximumLength);

        //  Assert
        randomString.Length.Should().BeGreaterThanOrEqualTo(minimumLength);
        randomString.Length.Should().BeLessThanOrEqualTo(maximumLength);
    }

    [Theory]
    [InlineData(3)]
    [InlineData(10)]
    [InlineData(21)]
    public void CreateRandomString_GivenMultipleCalls_ShouldGenerateUniqueStrings(int numberOfStrings)
    {
        //  Arrange
        var generatedStrings = new List<string>();

        //  Act
        for (var loopCount = 0; loopCount < numberOfStrings; loopCount++)
        {
            var randomString = RandomValueGenerator.CreateRandomString(1, 10);
            //  Assert
            if (generatedStrings.Contains(randomString))
            {
                Console.WriteLine($"Duplicate string found: {randomString}");
            }

            generatedStrings.Add(randomString);
        }
    }

    [Theory]
    [InlineData(1, 50)]
    [InlineData(781209, 987654)]
    public void CreateRandomNumber_GivenLongAndMinimumAndMaximumValues_ShouldGenerateValueBetweenGivenValues(int minimumValue, int maximumValue)
    {
        //  Arrange

        //  Act
        var randomNumber = RandomValueGenerator.CreateRandomNumber(minimumValue, maximumValue);

        //  Assert
        randomNumber.Should().BePositive();
        randomNumber.Should().BeGreaterThanOrEqualTo(minimumValue);
        randomNumber.Should().BeLessThanOrEqualTo(maximumValue);
    }

    [Fact]
    public void CreateRandomNumber_GivenLargeLongAndMinimumAndMaximumValues_ShouldGenerateValueBetweenGivenValues()
    {
        //  Arrange
        var minimumValue = new DateTime(1990, 1, 1).Ticks;
        var maximumValue = new DateTime(2020, 12, 31).Ticks;

        //  Act
        var randomNumber = RandomValueGenerator.CreateRandomNumber(minimumValue, maximumValue);
        //  Assert
        randomNumber.Should().BePositive();
        randomNumber.Should().BeGreaterThanOrEqualTo(minimumValue);
        randomNumber.Should().BeLessThanOrEqualTo(maximumValue);
    }

    [Fact]
    public void CreateRandomDate_GivenMultipleCalls_ShouldCreateRandomDates()
    {
        //  Arrange

        //  Act
        var randomDate1 = RandomValueGenerator.CreateRandomValue(typeof(DateTime));
        var randomDate2 = RandomValueGenerator.CreateRandomValue(typeof(DateTime));
        var randomDate3 = RandomValueGenerator.CreateRandomValue(typeof(DateTime));

        //  Assert
        randomDate1.Should().NotBeSameAs(randomDate2);
        randomDate1.Should().NotBeSameAs(randomDate3);
        randomDate2.Should().NotBeSameAs(randomDate3);
    }

    [Fact]
    public void CreateRandomDateOnly_GivenMultipleCalls_ShouldCreateRandomDateOnlys()
    {
        //  Arrange

        //  Act
        var randomDate1 = RandomValueGenerator.CreateRandomValue(typeof(DateOnly));
        var randomDate2 = RandomValueGenerator.CreateRandomValue(typeof(DateOnly));
        var randomDate3 = RandomValueGenerator.CreateRandomValue(typeof(DateOnly));

        //  Assert
        randomDate1.Should().NotBeSameAs(randomDate2);
        randomDate1.Should().NotBeSameAs(randomDate3);
        randomDate2.Should().NotBeSameAs(randomDate3);
    }

    [Theory]
    [InlineData(typeof(uint), typeof(uint))]
    [InlineData(typeof(short), typeof(short))]
    [InlineData(typeof(ushort), typeof(ushort))]
    [InlineData(typeof(long), typeof(long))]
    [InlineData(typeof(ulong), typeof(ulong))]
    [InlineData(typeof(double), typeof(double))]
    [InlineData(typeof(byte), typeof(byte))]
    [InlineData(typeof(byte[]), typeof(byte[]))]
    [InlineData(typeof(FakeException), typeof(FakeException))]
    [InlineData(typeof(Dictionary<string, object>), typeof(Dictionary<string, object>))]
    [InlineData(typeof(IDictionary<string, object>), typeof(Dictionary<string, object>))]
    [InlineData(typeof(ICollection<string>), typeof(Collection<string>))]
    [InlineData(typeof(FakeComplex[]), typeof(FakeComplex[]))]
    public void CreateRandomValue_GivenType_ShouldNotThrowExceptionAndCreateRandomValue(Type objectType, Type expectedType)
    {
        //  Arrange

        //  Act
        var randomValue = RandomValueGenerator.CreateRandomValue(objectType);

        //  Assert
        randomValue.Should().NotBeNull();
        randomValue.Should().BeOfType(expectedType);
    }

    [Theory]
    [InlineData(typeof(bool?), typeof(bool))]
    [InlineData(typeof(int?), typeof(int))]
    [InlineData(typeof(decimal?), typeof(decimal))]
    [InlineData(typeof(DateTime?), typeof(DateTime))]
    [InlineData(typeof(DateOnly?), typeof(DateOnly))]
    public void CreateRandomNullableValue_GivenType_ShouldNotThrowExceptionAndCreateRandomValue(Type objectType, Type expectedType)
    {
        //  Arrange

        //  Act
        var randomValue = RandomValueGenerator.CreateRandomValue(objectType);

        //  Assert
        randomValue.Should().NotBeNull();
        randomValue.Should().BeOfType(expectedType);
    }
}