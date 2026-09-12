using System.ComponentModel.DataAnnotations;
using ExpenseTracker.Shared;

namespace ExpenseTracker.Shared.Expenses;

public class ExpenseRequest
{
    [Required]
    public string Description { get; set; } = string.Empty;

    [PositiveAmount]
    public decimal Amount { get; set; }

    [Required]
    public DateOnly Date { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "CategoryId must reference an existing category.")]
    public int CategoryId { get; set; }

    public string? Notes { get; set; }
}
