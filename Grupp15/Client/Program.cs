using Blazored.LocalStorage;
using Grupp15.Client;
using Grupp15.Client.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddMudServices();
builder.Services.AddScoped<IAccountManager, AccountManager>();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();
builder.Services.AddScoped<IBookManager, BookManager>();
builder.Services.AddScoped<IEBookManager, EBookManager>();
builder.Services.AddScoped<IMovieManager, MovieManager>();
builder.Services.AddScoped<ILectureManager, LectureManager>();
builder.Services.AddScoped<INewsManager, NewsManager>();
builder.Services.AddScoped<IProductManager, ProductManager>();
builder.Services.AddScoped<IBorrowingManager, BorrowingManager>();

await builder.Build().RunAsync();
