namespace Lithan.Core.TestUtilities.UnitTests.Fakes;

public class FakeTestClass2
{
    public FakeTestClass2(int someTestValue)
    {
        SomeTestValue = someTestValue;
    }

    public FakeTestClass2(int someTestValue, FakeComplex[] allFakes)
    {
        SomeTestValue = someTestValue;
        AllFakes      = allFakes;
    }

    public int SomeTestValue { get; }
    public FakeComplex[]? AllFakes { get; set; }

    [FakeTest("FakeTestMethod")]
    public void FakeTestMethod()
    {
    }

    [FakeTest("FakeTestMethod")]
    public void FakeTestMethod(int someParameter)
    {
    }
}