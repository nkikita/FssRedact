using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using FssRedact.Services;
using Radzen;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<DialogService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddHttpClient<UserService>();
builder.Services.AddScoped<AddressService>();
builder.Services.AddHttpClient("GetAddress", client => client.BaseAddress = new Uri("https://data.pbprog.ru/api/address/full-address/"));
builder.Services.AddScoped<AddressService>();



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
