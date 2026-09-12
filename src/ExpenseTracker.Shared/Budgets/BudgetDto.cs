namespace ExpenseTracker.Shared.Budgets;

public record BudgetDto(
    int Id,
    int CategoryId,
    string CategoryName,
    DateOnly Month,
    decimal Amount);
