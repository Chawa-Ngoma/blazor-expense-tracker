using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Api.Models;

public class Category
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Color { get; set; } = string.Empty;
}
