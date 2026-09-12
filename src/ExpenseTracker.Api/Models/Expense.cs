using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Api.Models;

public class Expense
{
    public int Id { get; set; }

    [Required]
    public string Description { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public DateOnly Date { get; set; }

    public int CategoryId { get; set; }

    public Category? Category { get; set; }

    public string? Notes { get; set; }
}
