namespace ExpenseTracker.Client.Services.Api;

/// <summary>Thrown when the API returns a non-success status. Carries the status code and any
/// problem-details message so callers (e.g. the state service, UI) can surface it without
/// having to know about HttpResponseMessage.</summary>
public class ApiException(int statusCode, string message) : Exception(message)
{
    public int StatusCode { get; } = statusCode;
}
