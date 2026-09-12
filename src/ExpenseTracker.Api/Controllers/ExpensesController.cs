using ExpenseTracker.Api.Data;
using ExpenseTracker.Api.Models;
using ExpenseTracker.Shared.Expenses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExpensesController(ExpenseTrackerDbContext db) : ControllerBase
{
    /// <summary>List expenses, optionally filtered by year, month, and/or category.</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ExpenseDto>>> GetAll(int? year, int? month, int? categoryId)
    {
        var query = db.Expenses.Include(e => e.Category).AsQueryable();

        if (year.HasValue)
            query = query.Where(e => e.Date.Year == year.Value);

        if (month.HasValue)
            query = query.Where(e => e.Date.Month == month.Value);

        if (categoryId.HasValue)
            query = query.Where(e => e.CategoryId == categoryId.Value);

        var expenses = await query
            .OrderByDescending(e => e.Date)
            .Select(e => ToDto(e))
            .ToListAsync();

        return Ok(expenses);
    }

    /// <summary>Total spend per month for the trailing window of months ending at year/month (inclusive).
    /// Months with no expenses are included with a total of 0.</summary>
    [HttpGet("monthly-totals")]
    public async Task<ActionResult<IEnumerable<MonthlyTotalDto>>> GetMonthlyTotals(int year, int month, int months = 6)
    {
        var end = new DateOnly(year, month, 1);
        var start = end.AddMonths(-(months - 1));
        var endExclusive = end.AddMonths(1);

        var expensesInRange = await db.Expenses
            .Where(e => e.Date >= start && e.Date < endExclusive)
            .Select(e => new { e.Date.Year, e.Date.Month, e.Amount })
            .ToListAsync();

        var totalsByMonth = expensesInRange
            .GroupBy(e => (e.Year, e.Month))
            .ToDictionary(g => g.Key, g => g.Sum(e => e.Amount));

        var result = new List<MonthlyTotalDto>();
        for (var i = 0; i < months; i++)
        {
            var current = start.AddMonths(i);
            totalsByMonth.TryGetValue((current.Year, current.Month), out var total);
            result.Add(new MonthlyTotalDto(current.Year, current.Month, total));
        }

        return Ok(result);
    }

    /// <summary>Get a single expense by id.</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ExpenseDto>> GetById(int id)
    {
        var expense = await db.Expenses.Include(e => e.Category).FirstOrDefaultAsync(e => e.Id == id);
        if (expense is null)
            return NotFound();

        return Ok(ToDto(expense));
    }

    /// <summary>Create a new expense.</summary>
    [HttpPost]
    public async Task<ActionResult<ExpenseDto>> Create(ExpenseRequest request)
    {
        var category = await db.Categories.FindAsync(request.CategoryId);
        if (category is null)
        {
            ModelState.AddModelError(nameof(request.CategoryId), "CategoryId does not reference an existing category.");
            return ValidationProblem(ModelState);
        }

        var expense = new Expense
        {
            Description = request.Description,
            Amount = request.Amount,
            Date = request.Date,
            CategoryId = request.CategoryId,
            Notes = request.Notes
        };

        db.Expenses.Add(expense);
        await db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = expense.Id }, ToDto(expense, category));
    }

    /// <summary>Update an existing expense.</summary>
    [HttpPut("{id:int}")]
    public async Task<ActionResult<ExpenseDto>> Update(int id, ExpenseRequest request)
    {
        var expense = await db.Expenses.FindAsync(id);
        if (expense is null)
            return NotFound();

        var category = await db.Categories.FindAsync(request.CategoryId);
        if (category is null)
        {
            ModelState.AddModelError(nameof(request.CategoryId), "CategoryId does not reference an existing category.");
            return ValidationProblem(ModelState);
        }

        expense.Description = request.Description;
        expense.Amount = request.Amount;
        expense.Date = request.Date;
        expense.CategoryId = request.CategoryId;
        expense.Notes = request.Notes;

        await db.SaveChangesAsync();

        return Ok(ToDto(expense, category));
    }

    /// <summary>Delete an expense.</summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var expense = await db.Expenses.FindAsync(id);
        if (expense is null)
            return NotFound();

        db.Expenses.Remove(expense);
        await db.SaveChangesAsync();

        return NoContent();
    }

    private static ExpenseDto ToDto(Expense expense) =>
        new(expense.Id, expense.Description, expense.Amount, expense.Date, expense.CategoryId, expense.Category!.Name, expense.Notes);

    private static ExpenseDto ToDto(Expense expense, Category category) =>
        new(expense.Id, expense.Description, expense.Amount, expense.Date, expense.CategoryId, category.Name, expense.Notes);
}
