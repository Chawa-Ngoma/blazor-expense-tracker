using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Shared;

/// <summary>Validates that a decimal amount is strictly greater than zero.</summary>
public class PositiveAmountAttribute : ValidationAttribute
{
    public PositiveAmountAttribute() : base("Amount must be greater than 0.")
    {
    }

    public override bool IsValid(object? value) => value is decimal amount && amount > 0;
}
