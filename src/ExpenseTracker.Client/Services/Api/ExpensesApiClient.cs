using System.Net.Http.Json;
using ExpenseTracker.Shared.Expenses;

namespace ExpenseTracker.Client.Services.Api;

public class ExpensesApiClient(HttpClient http) : IExpensesApiClient
{
    public async Task<List<ExpenseDto>> GetAllAsync(int? year = null, int? month = null, int? categoryId = null)
    {
        var parameters = new List<string>();
        if (year.HasValue) parameters.Add($"year={year.Value}");
        if (month.HasValue) parameters.Add($"month={month.Value}");
        if (categoryId.HasValue) parameters.Add($"categoryId={categoryId.Value}");

        var url = parameters.Count > 0 ? $"api/expenses?{string.Join('&', parameters)}" : "api/expenses";
        return await http.GetFromJsonAsync<List<ExpenseDto>>(url) ?? [];
    }

    public async Task<ExpenseDto> GetByIdAsync(int id)
    {
        return (await http.GetFromJsonAsync<ExpenseDto>($"api/expenses/{id}"))!;
    }

    public async Task<ExpenseDto> CreateAsync(ExpenseRequest request)
    {
        var response = await http.PostAsJsonAsync("api/expenses", request);
        await response.EnsureSuccessOrThrowAsync();
        return (await response.Content.ReadFromJsonAsync<ExpenseDto>())!;
    }

    public async Task<ExpenseDto> UpdateAsync(int id, ExpenseRequest request)
    {
        var response = await http.PutAsJsonAsync($"api/expenses/{id}", request);
        await response.EnsureSuccessOrThrowAsync();
        return (await response.Content.ReadFromJsonAsync<ExpenseDto>())!;
    }

    public async Task DeleteAsync(int id)
    {
        var response = await http.DeleteAsync($"api/expenses/{id}");
        await response.EnsureSuccessOrThrowAsync();
    }

    public async Task<List<MonthlyTotalDto>> GetMonthlyTotalsAsync(int year, int month, int months = 6)
    {
        return await http.GetFromJsonAsync<List<MonthlyTotalDto>>(
            $"api/expenses/monthly-totals?year={year}&month={month}&months={months}") ?? [];
    }
}
