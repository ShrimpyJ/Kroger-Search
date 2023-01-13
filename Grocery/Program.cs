using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.IdleTimeout = TimeSpan.FromMinutes(30);
});

builder.Services.AddHttpClient("KrogerToken", httpClient =>
{
    httpClient.BaseAddress = new Uri("https://api.kroger.com/v1/");
    httpClient.DefaultRequestHeaders.Add(
        HeaderNames.Accept, "application/x-www-form-urlencoded");
    httpClient.DefaultRequestHeaders.Add(
        HeaderNames.Authorization, "Basic Z3JvY2VyeXdhZ29uLThjMGRhZGIzYzZhMjY1NjQxMzVmMjY4ZWIyZmM5NjQ4ODkzMTAxMjM1ODc0NjMzMTk2Mjo4RVRZczA3ZkRGWDVaVnU1eGY1RTdjX2wtMlhETkxldVRVaWg1QTlM");
});
builder.Services.AddHttpClient("KrogerProduct", httpClient =>
{
    httpClient.BaseAddress = new Uri("https://api.kroger.com/v1/");
    httpClient.DefaultRequestHeaders.Add(
        HeaderNames.Accept, "application/json");
});

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

app.UseAuthorization();

app.UseSession();

app.MapRazorPages();

app.Run();
