using ExpenseTracker.Api.Data;
using ExpenseTracker.Api.Models;
using ExpenseTracker.Shared.Budgets;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BudgetsController(ExpenseTrackerDbContext db) : ControllerBase
{
    /// <summary>List budgets, optionally filtered to a single year/month.</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BudgetDto>>> GetAll(int? year, int? month)
    {
        var query = db.Budgets.Include(b => b.Category).AsQueryable();

        if (year.HasValue)
            query = query.Where(b => b.Month.Year == year.Value);

        if (month.HasValue)
            query = query.Where(b => b.Month.Month == month.Value);

        var budgets = await query
            .OrderBy(b => b.Month)
            .ThenBy(b => b.Category!.Name)
            .Select(b => ToDto(b))
            .ToListAsync();

        return Ok(budgets);
    }

    /// <summary>Create a budget for a category and month. One budget per category per month is enforced.</summary>
    [HttpPost]
    public async Task<ActionResult<BudgetDto>> Create(BudgetCreateRequest request)
    {
        var category = await db.Categories.FindAsync(request.CategoryId);
        if (category is null)
        {
            ModelState.AddModelError(nameof(request.CategoryId), "CategoryId does not reference an existing category.");
            return ValidationProblem(ModelState);
        }

        var month = NormalizeToFirstOfMonth(request.Month);

        var duplicateExists = await db.Budgets.AnyAsync(b => b.CategoryId == request.CategoryId && b.Month == month);
        if (duplicateExists)
        {
            return Conflict(new ProblemDetails
            {
                Title = "Budget already exists",
                Detail = $"A budget for category '{category.Name}' in {month:yyyy-MM} already exists.",
                Status = StatusCodes.Status409Conflict
            });
        }

        var budget = new Budget
        {
            CategoryId = request.CategoryId,
            Month = month,
            Amount = request.Amount
        };

        db.Budgets.Add(budget);
        await db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAll), new { year = month.Year, month = month.Month }, ToDto(budget, category));
    }

    /// <summary>Update a budget's amount.</summary>
    [HttpPut("{id:int}")]
    public async Task<ActionResult<BudgetDto>> Update(int id, BudgetUpdateRequest request)
    {
        var budget = await db.Budgets.Include(b => b.Category).FirstOrDefaultAsync(b => b.Id == id);
        if (budget is null)
            return NotFound();

        budget.Amount = request.Amount;
        await db.SaveChangesAsync();

        return Ok(ToDto(budget));
    }

    /// <summary>Delete a budget.</summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var budget = await db.Budgets.FindAsync(id);
        if (budget is null)
            return NotFound();

        db.Budgets.Remove(budget);
        await db.SaveChangesAsync();

        return NoContent();
    }

    private static DateOnly NormalizeToFirstOfMonth(DateOnly date) => new(date.Year, date.Month, 1);

    private static BudgetDto ToDto(Budget budget) =>
        new(budget.Id, budget.CategoryId, budget.Category!.Name, budget.Month, budget.Amount);

    private static BudgetDto ToDto(Budget budget, Category category) =>
        new(budget.Id, budget.CategoryId, category.Name, budget.Month, budget.Amount);
}
