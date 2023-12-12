using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using eToolsWebApp.Data;
using Microsoft.EntityFrameworkCore;
using PurchasingSystem;
using PurchasingSystem.BLL;
using ReceivingSystem;
using ReceivingSystem.BLL;
using SalesSystem;
using SalesSystem.BLL;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// get connection string
var connectionString = builder.Configuration.GetConnectionString("eTools2023") ??
                       throw new Exception("Could not find connection string 'eTools2023'." +
                                           " Did you remember to add your secret");

// app state
builder.Services.AddScoped<AppState>();
// db contexts
builder.Services.AddPurchasingDependencies(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddReceivingDependencies(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddSalesDependencies(options =>
    options.UseSqlServer(connectionString));

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddMudServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
