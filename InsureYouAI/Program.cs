using InsureYouAI.Context;
using InsureYouAI.Entities;
using InsureYouAI.Models;
using InsureYouAI.Services;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddSignalR();
builder.Services.AddHttpClient("openai", c =>
{
    c.BaseAddress = new Uri("https://api.openai.com/");
});

// Add services to the container.
builder.Services.AddDbContext<InsureContext>();
builder.Services.AddControllersWithViews();

builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<InsureContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<AiMessageClassifierService>();

var app = builder.Build();

app.UseExceptionHandler("/Errors/500");
//app.UseStatusCodePagesWithReExecute("/Errors/Page{0}");

// *** EN ÜSTE GELMESİ GEREKENLER ***
app.UseHttpsRedirection();
app.UseStaticFiles();   // ← Bunu ekle (çok önemli)
app.UseRouting();

app.UseAuthentication();  // ← Mutlaka UseAuthorization'dan önce olmalı
app.UseAuthorization();

// SignalR route
app.MapHub<ChatHub>("/chathub");

// *** CONTROLLER ROUTE BURADA OLMALI ***
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");



app.Run();
