using System.ComponentModel.DataAnnotations;
using ExpenseTracker.Shared;

namespace ExpenseTracker.Shared.Budgets;

public class BudgetUpdateRequest
{
    [PositiveAmount]
    public decimal Amount { get; set; }
}
