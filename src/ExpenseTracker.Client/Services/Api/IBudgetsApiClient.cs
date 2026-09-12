using ExpenseTracker.Shared.Budgets;

namespace ExpenseTracker.Client.Services.Api;

public interface IBudgetsApiClient
{
    Task<List<BudgetDto>> GetAllAsync(int? year = null, int? month = null);
    Task<BudgetDto> CreateAsync(BudgetCreateRequest request);
    Task<BudgetDto> UpdateAsync(int id, BudgetUpdateRequest request);
    Task DeleteAsync(int id);
}
