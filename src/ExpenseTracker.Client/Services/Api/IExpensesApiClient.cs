using ExpenseTracker.Shared.Expenses;

namespace ExpenseTracker.Client.Services.Api;

public interface IExpensesApiClient
{
    Task<List<ExpenseDto>> GetAllAsync(int? year = null, int? month = null, int? categoryId = null);
    Task<ExpenseDto> GetByIdAsync(int id);
    Task<ExpenseDto> CreateAsync(ExpenseRequest request);
    Task<ExpenseDto> UpdateAsync(int id, ExpenseRequest request);
    Task DeleteAsync(int id);
}
