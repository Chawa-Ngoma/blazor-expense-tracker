using ExpenseTracker.Api.Data;
using ExpenseTracker.Api.Models;
using ExpenseTracker.Shared.Categories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController(ExpenseTrackerDbContext db) : ControllerBase
{
    /// <summary>List all categories.</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAll()
    {
        var categories = await db.Categories
            .OrderBy(c => c.Name)
            .Select(c => ToDto(c))
            .ToListAsync();

        return Ok(categories);
    }

    /// <summary>Create a new category.</summary>
    [HttpPost]
    public async Task<ActionResult<CategoryDto>> Create(CategoryRequest request)
    {
        var category = new Category { Name = request.Name, Color = request.Color };
        db.Categories.Add(category);
        await db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAll), new { id = category.Id }, ToDto(category));
    }

    /// <summary>Update an existing category.</summary>
    [HttpPut("{id:int}")]
    public async Task<ActionResult<CategoryDto>> Update(int id, CategoryRequest request)
    {
        var category = await db.Categories.FindAsync(id);
        if (category is null)
            return NotFound();

        category.Name = request.Name;
        category.Color = request.Color;
        await db.SaveChangesAsync();

        return Ok(ToDto(category));
    }

    /// <summary>Delete a category. Blocked if any expense or budget still references it.</summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var category = await db.Categories.FindAsync(id);
        if (category is null)
            return NotFound();

        var isReferenced = await db.Expenses.AnyAsync(e => e.CategoryId == id)
            || await db.Budgets.AnyAsync(b => b.CategoryId == id);

        if (isReferenced)
        {
            return Conflict(new ProblemDetails
            {
                Title = "Category is in use",
                Detail = "This category cannot be deleted because it is referenced by one or more expenses or budgets.",
                Status = StatusCodes.Status409Conflict
            });
        }

        db.Categories.Remove(category);
        await db.SaveChangesAsync();

        return NoContent();
    }

    private static CategoryDto ToDto(Category category) => new(category.Id, category.Name, category.Color);
}
