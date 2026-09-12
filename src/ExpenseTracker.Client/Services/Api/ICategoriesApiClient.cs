using ExpenseTracker.Shared.Categories;

namespace ExpenseTracker.Client.Services.Api;

public interface ICategoriesApiClient
{
    Task<List<CategoryDto>> GetAllAsync();
    Task<CategoryDto> CreateAsync(CategoryRequest request);
    Task<CategoryDto> UpdateAsync(int id, CategoryRequest request);
    Task DeleteAsync(int id);
}
