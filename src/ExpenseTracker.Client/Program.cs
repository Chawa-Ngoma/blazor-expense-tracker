using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ExpenseTracker.Client;
using ExpenseTracker.Client.Services.Api;
using ExpenseTracker.Client.Services.State;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddScoped<ICategoriesApiClient, CategoriesApiClient>();
builder.Services.AddScoped<IExpensesApiClient, ExpensesApiClient>();
builder.Services.AddScoped<IBudgetsApiClient, BudgetsApiClient>();

builder.Services.AddScoped<ExpenseTrackerState>();

await builder.Build().RunAsync();
