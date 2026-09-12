using System.ComponentModel.DataAnnotations;
using ExpenseTracker.Shared;

namespace ExpenseTracker.Shared.Budgets;

public class BudgetCreateRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "CategoryId must reference an existing category.")]
    public int CategoryId { get; set; }

    /// <summary>Any day within the target month; normalized server-side to the 1st.</summary>
    [Required]
    public DateOnly Month { get; set; }

    [PositiveAmount]
    public decimal Amount { get; set; }
}
