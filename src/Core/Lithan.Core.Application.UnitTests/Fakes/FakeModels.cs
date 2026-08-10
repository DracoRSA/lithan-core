using System.ComponentModel.DataAnnotations;

namespace Lithan.Core.Application.UnitTests.Fakes;

public enum FakeEntityStatus
{
    Unknown = 0,
    Active = 1,
    Inactive = 2
}

public class FakeEntityChild
{
    public string? Name { get; set; }
}

public class FakeEntity
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public int Age { get; set; }
    public DateTime CreatedOn { get; set; }
    public FakeEntityStatus Status { get; set; }
    public FakeEntityChild? Child { get; set; }
}

public class FakeRequiredModel
{
    [Required(ErrorMessage = "Name is required")]
    public string? Name { get; set; }

    [Required(ErrorMessage = "CreatedOn is required")]
    public DateTime CreatedOn { get; set; }

    [Required(ErrorMessage = "Count is required")]
    public int Count { get; set; }

    [Required(ErrorMessage = "Amount is required")]
    public long Amount { get; set; }

    [Required(ErrorMessage = "Ratio is required")]
    public double Ratio { get; set; }

    public string? OptionalNote { get; set; }
}
