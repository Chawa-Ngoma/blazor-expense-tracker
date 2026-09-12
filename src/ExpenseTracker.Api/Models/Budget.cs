namespace ExpenseTracker.Api.Models;

/// <summary>One budget per category per month. <see cref="Month"/> is always the 1st of the month.</summary>
public class Budget
{
    public int Id { get; set; }

    public int CategoryId { get; set; }

    public Category? Category { get; set; }

    public DateOnly Month { get; set; }

    public decimal Amount { get; set; }
}
