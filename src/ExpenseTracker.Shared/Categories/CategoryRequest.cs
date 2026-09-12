using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Shared.Categories;

public class CategoryRequest
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    [RegularExpression("^#[0-9A-Fa-f]{6}$", ErrorMessage = "Color must be a hex value like #3B82F6.")]
    public string Color { get; set; } = string.Empty;
}
