# Expense Tracker

A personal expense tracking app built with a classic hosted Blazor architecture:
a standalone Blazor WebAssembly client talking to a separate ASP.NET Core Web
API over REST, with shared DTOs in a common library.

## Solution layout

```
src/
├── ExpenseTracker.Api/      ASP.NET Core Web API (Swagger enabled)
├── ExpenseTracker.Client/   Blazor WASM client (standalone, calls the API over HTTP)
└── ExpenseTracker.Shared/   DTOs shared by the API and the client
```

The API hosts the client's static assets, so running the API serves the full
app at the same origin — no CORS configuration needed in development.

## Running locally

```
dotnet run --project src/ExpenseTracker.Api
```

Then open the URL printed in the console. Swagger UI is available at `/swagger`.

## Technical decisions

**Separate WASM client + API, not the unified "Blazor Web App" template.** Even
though .NET now offers a single-project model with mixed Server/WASM/Auto render
modes, this app deliberately uses the older three-project shape (API / Client /
Shared) so the client is a genuinely separate SPA calling a REST API — this is
the same integration pattern used elsewhere and keeps the API reusable by other
future clients.

More decisions will be recorded here as the app takes shape.
