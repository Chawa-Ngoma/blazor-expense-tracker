using ExpenseTracker.Client.Services.Api;
using ExpenseTracker.Shared.Budgets;
using ExpenseTracker.Shared.Categories;
using ExpenseTracker.Shared.Expenses;

namespace ExpenseTracker.Client.Services.State;

/// <summary>Holds the client's current view of the selected month's data. Every mutating method
/// calls the API, then refreshes in-memory state from the server response and raises
/// <see cref="OnChange"/> so subscribed components re-render.</summary>
public class ExpenseTrackerState(
    ICategoriesApiClient categoriesApi,
    IExpensesApiClient expensesApi,
    IBudgetsApiClient budgetsApi)
{
    public event Action? OnChange;

    public DateOnly SelectedMonth { get; private set; } = new(DateTime.Today.Year, DateTime.Today.Month, 1);

    public IReadOnlyList<CategoryDto> Categories { get; private set; } = [];
    public IReadOnlyList<ExpenseDto> Expenses { get; private set; } = [];
    public IReadOnlyList<BudgetDto> Budgets { get; private set; } = [];

    /// <summary>Total spend per month for the 6 months ending at <see cref="SelectedMonth"/>, oldest first.</summary>
    public IReadOnlyList<MonthlyTotalDto> MonthlyTotals { get; private set; } = [];

    public bool IsLoading { get; private set; }

    public async Task InitializeAsync()
    {
        await LoadAllAsync();
    }

    public async Task SetSelectedMonthAsync(DateOnly month)
    {
        SelectedMonth = new DateOnly(month.Year, month.Month, 1);
        await LoadMonthDataAsync();
    }

    public async Task<CategoryDto> CreateCategoryAsync(CategoryRequest request)
    {
        var created = await categoriesApi.CreateAsync(request);
        await LoadCategoriesAsync();
        return created;
    }

    public async Task<CategoryDto> UpdateCategoryAsync(int id, CategoryRequest request)
    {
        var updated = await categoriesApi.UpdateAsync(id, request);
        await LoadCategoriesAsync();
        return updated;
    }

    public async Task DeleteCategoryAsync(int id)
    {
        await categoriesApi.DeleteAsync(id);
        await LoadCategoriesAsync();
    }

    public async Task<ExpenseDto> CreateExpenseAsync(ExpenseRequest request)
    {
        var created = await expensesApi.CreateAsync(request);
        await LoadMonthDataAsync();
        return created;
    }

    public async Task<ExpenseDto> UpdateExpenseAsync(int id, ExpenseRequest request)
    {
        var updated = await expensesApi.UpdateAsync(id, request);
        await LoadMonthDataAsync();
        return updated;
    }

    public async Task DeleteExpenseAsync(int id)
    {
        await expensesApi.DeleteAsync(id);
        await LoadMonthDataAsync();
    }

    public async Task<BudgetDto> CreateBudgetAsync(BudgetCreateRequest request)
    {
        var created = await budgetsApi.CreateAsync(request);
        await LoadMonthDataAsync();
        return created;
    }

    public async Task<BudgetDto> UpdateBudgetAsync(int id, BudgetUpdateRequest request)
    {
        var updated = await budgetsApi.UpdateAsync(id, request);
        await LoadMonthDataAsync();
        return updated;
    }

    public async Task DeleteBudgetAsync(int id)
    {
        await budgetsApi.DeleteAsync(id);
        await LoadMonthDataAsync();
    }

    private async Task LoadAllAsync()
    {
        IsLoading = true;
        NotifyChanged();
        try
        {
            Categories = await categoriesApi.GetAllAsync();
            await LoadExpensesAndBudgetsAsync();
        }
        finally
        {
            IsLoading = false;
            NotifyChanged();
        }
    }

    private async Task LoadCategoriesAsync()
    {
        Categories = await categoriesApi.GetAllAsync();
        NotifyChanged();
    }

    private async Task LoadMonthDataAsync()
    {
        IsLoading = true;
        NotifyChanged();
        try
        {
            await LoadExpensesAndBudgetsAsync();
        }
        finally
        {
            IsLoading = false;
            NotifyChanged();
        }
    }

    private async Task LoadExpensesAndBudgetsAsync()
    {
        Expenses = await expensesApi.GetAllAsync(SelectedMonth.Year, SelectedMonth.Month);
        Budgets = await budgetsApi.GetAllAsync(SelectedMonth.Year, SelectedMonth.Month);
        MonthlyTotals = await expensesApi.GetMonthlyTotalsAsync(SelectedMonth.Year, SelectedMonth.Month);
    }

    private void NotifyChanged() => OnChange?.Invoke();
}
