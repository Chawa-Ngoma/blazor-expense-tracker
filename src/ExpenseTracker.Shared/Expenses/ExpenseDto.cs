namespace ExpenseTracker.Shared.Expenses;

public record ExpenseDto(
    int Id,
    string Description,
    decimal Amount,
    DateOnly Date,
    int CategoryId,
    string CategoryName,
    string? Notes);
