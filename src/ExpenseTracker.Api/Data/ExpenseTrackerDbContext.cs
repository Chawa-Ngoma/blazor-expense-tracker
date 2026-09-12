using ExpenseTracker.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Api.Data;

public class ExpenseTrackerDbContext(DbContextOptions<ExpenseTrackerDbContext> options) : DbContext(options)
{
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Expense> Expenses => Set<Expense>();
    public DbSet<Budget> Budgets => Set<Budget>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Budget>()
            .HasIndex(b => new { b.CategoryId, b.Month })
            .IsUnique();

        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Groceries", Color = "#22C55E" },
            new Category { Id = 2, Name = "Rent", Color = "#3B82F6" },
            new Category { Id = 3, Name = "Transport", Color = "#F59E0B" },
            new Category { Id = 4, Name = "Utilities", Color = "#06B6D4" },
            new Category { Id = 5, Name = "Entertainment", Color = "#EC4899" },
            new Category { Id = 6, Name = "Other", Color = "#6B7280" }
        );
    }
}
