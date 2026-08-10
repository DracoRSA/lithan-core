using System.ComponentModel.DataAnnotations;

namespace Lithan.Core.TestUtilities.UnitTests.Fakes;

public class FakeTestClass
{
    public FakeTestClass()
    {
    }

    public FakeTestClass(Guid fakeId, string testName, int testIntValue, bool isActive,
                         // ReSharper disable once UnusedParameter.Local
                         string notSetParameter,
                         DateTime testDateTime,
                         FakeComplex complexObjectNotTested, FakeComplex complexObject, IFakeComplex complexInterface,
                         FakeTestEnum testEnum,
                         IEnumerable<IFakeComplex>? allFakes = null,
                         Dictionary<string, string>? testDictionary = null,
                         IDictionary<string, object>? testDictionary2 = null)
        : this(testDictionary, testDictionary2)
    {
        Id             = fakeId;
        Name           = testName ?? throw new ArgumentNullException(nameof(testName));
        IntValue       = testIntValue;
        IsActive       = isActive;
        TestDateTime   = testDateTime;
        ComplexObject1 = complexObjectNotTested;
        ComplexObject2 = complexObject    ?? throw new ArgumentNullException(nameof(complexObject));
        ComplexObject3 = complexInterface ?? throw new ArgumentNullException(nameof(complexInterface));
        TestEnum       = testEnum;
        FakeList       = allFakes;
    }

    public FakeTestClass(Dictionary<string, string>? testDictionary = null, IDictionary<string, object>? testDictionary2 = null)
    {
        TestDictionary  = testDictionary  ?? throw new ArgumentNullException(nameof(testDictionary));
        TestDictionary2 = testDictionary2 ?? throw new ArgumentNullException(nameof(testDictionary2));
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public int IntValue { get; private set; }
    public bool IsActive { get; private set; }

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    public string NotSetProperty { get; } = null!;

    public DateTime TestDateTime { get; private set; }

    [Required]
    public FakeComplex ComplexObject1 { get; private set; } = null!;

    [FakeTest("ComplexObject2")] public FakeComplex ComplexObject2 { get; private set; } = null!;

    [FakeTest("ComplexObject2")] public IFakeComplex ComplexObject3 { get; private set; } = null!;

    public FakeTestEnum TestEnum { get; private set; }
    public IEnumerable<IFakeComplex>? FakeList { get; private set; } = null!;

    [FakeTest("TestDictionary", Sequence = 23)]
    public Dictionary<string, string> TestDictionary { get; private set; } = null!;

    public IDictionary<string, object> TestDictionary2 { get; private set; } = null!;

    [FakeTest("TestMethod1", Sequence = 2)]
    [FakeTest("TestMethod1", Sequence = 5)]
    public void TestMethod1(string userName, int userId)
    {
        if (userName == null)
        {
            throw new ArgumentNullException(nameof(userName));
        }
    }

    public void TestMethod2(string userName, int userId, FakeComplex fakeComplex)
    {
        if (fakeComplex == null)
        {
            throw new ArgumentNullException(nameof(fakeComplex));
        }
    }

    public Task TestMethod3Async(string userName, int userId, FakeComplex fakeComplex)
    {
        if (fakeComplex == null)
        {
            throw new ArgumentNullException(nameof(fakeComplex));
        }

        return Task.CompletedTask;
    }
}