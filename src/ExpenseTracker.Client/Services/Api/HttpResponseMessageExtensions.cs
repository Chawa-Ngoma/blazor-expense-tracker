using System.Net.Http.Json;

namespace ExpenseTracker.Client.Services.Api;

internal static class HttpResponseMessageExtensions
{
    /// <summary>Throws an <see cref="ApiException"/> with the API's ProblemDetails message
    /// (falling back to the reason phrase) if the response was not successful.</summary>
    public static async Task EnsureSuccessOrThrowAsync(this HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
            return;

        string message = response.ReasonPhrase ?? "Request failed.";
        try
        {
            var problem = await response.Content.ReadFromJsonAsync<ProblemDetailsResponse>();
            if (problem is not null)
                message = problem.Detail ?? problem.Title ?? message;
        }
        catch
        {
            // Body wasn't JSON problem-details; fall back to the reason phrase.
        }

        throw new ApiException((int)response.StatusCode, message);
    }

    private class ProblemDetailsResponse
    {
        public string? Title { get; set; }
        public string? Detail { get; set; }
    }
}
