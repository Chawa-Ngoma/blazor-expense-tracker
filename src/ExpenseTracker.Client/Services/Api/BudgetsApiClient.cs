using System.Net.Http.Json;
using ExpenseTracker.Shared.Budgets;

namespace ExpenseTracker.Client.Services.Api;

public class BudgetsApiClient(HttpClient http) : IBudgetsApiClient
{
    public async Task<List<BudgetDto>> GetAllAsync(int? year = null, int? month = null)
    {
        var parameters = new List<string>();
        if (year.HasValue) parameters.Add($"year={year.Value}");
        if (month.HasValue) parameters.Add($"month={month.Value}");

        var url = parameters.Count > 0 ? $"api/budgets?{string.Join('&', parameters)}" : "api/budgets";
        return await http.GetFromJsonAsync<List<BudgetDto>>(url) ?? [];
    }

    public async Task<BudgetDto> CreateAsync(BudgetCreateRequest request)
    {
        var response = await http.PostAsJsonAsync("api/budgets", request);
        await response.EnsureSuccessOrThrowAsync();
        return (await response.Content.ReadFromJsonAsync<BudgetDto>())!;
    }

    public async Task<BudgetDto> UpdateAsync(int id, BudgetUpdateRequest request)
    {
        var response = await http.PutAsJsonAsync($"api/budgets/{id}", request);
        await response.EnsureSuccessOrThrowAsync();
        return (await response.Content.ReadFromJsonAsync<BudgetDto>())!;
    }

    public async Task DeleteAsync(int id)
    {
        var response = await http.DeleteAsync($"api/budgets/{id}");
        await response.EnsureSuccessOrThrowAsync();
    }
}
