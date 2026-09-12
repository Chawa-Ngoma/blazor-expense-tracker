namespace ExpenseTracker.Shared;

public record PingResponse(string Message, DateTimeOffset ServerTimeUtc);
