using System.Net.Http.Json;
using ExpenseTracker.Shared.Categories;

namespace ExpenseTracker.Client.Services.Api;

public class CategoriesApiClient(HttpClient http) : ICategoriesApiClient
{
    public async Task<List<CategoryDto>> GetAllAsync()
    {
        return await http.GetFromJsonAsync<List<CategoryDto>>("api/categories") ?? [];
    }

    public async Task<CategoryDto> CreateAsync(CategoryRequest request)
    {
        var response = await http.PostAsJsonAsync("api/categories", request);
        await response.EnsureSuccessOrThrowAsync();
        return (await response.Content.ReadFromJsonAsync<CategoryDto>())!;
    }

    public async Task<CategoryDto> UpdateAsync(int id, CategoryRequest request)
    {
        var response = await http.PutAsJsonAsync($"api/categories/{id}", request);
        await response.EnsureSuccessOrThrowAsync();
        return (await response.Content.ReadFromJsonAsync<CategoryDto>())!;
    }

    public async Task DeleteAsync(int id)
    {
        var response = await http.DeleteAsync($"api/categories/{id}");
        await response.EnsureSuccessOrThrowAsync();
    }
}
