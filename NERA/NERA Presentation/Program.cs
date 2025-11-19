using Auth0.AspNetCore.Authentication;
using Data;
using Data.Repositories;
using Domain.Entities;
using Domain.Interfaces;
using Logic.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages(); 
builder.Services.AddControllers();

// Get Auth0 settings from launch settings.
var auth0Section = builder.Configuration.GetSection("Auth0");

// add authentication service
builder.Services
    .AddAuth0WebAppAuthentication(options =>
    {
    options.Domain = auth0Section["Domain"];
    options.ClientId = auth0Section["ClientId"];
    options.ClientSecret = auth0Section["ClientSecret"];
    options.Scope = "openid profile email";
    });

builder.Services.Configure<CookieAuthenticationOptions>(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.SlidingExpiration = true;
});

// Database connection
builder.Services.AddDbContext<AppDbContext>(opts =>
    opts.UseSqlServer(builder.Configuration.GetConnectionString("DbConnection")));

// Register Repositories and Services
builder.Services.AddScoped<ICreateEventRepo, CreateEventRepo>();
builder.Services.AddScoped<CreateEventService>();
builder.Services.AddScoped<UpdateEventService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    try
    {
        // Try connecting to DB
        if (!db.Database.CanConnect())
        {
            Console.WriteLine("?? Database not reachable. Running in limited mode.");
        }
        else
        {
            // Optional: run migrations or seed data if connected
            // db.Database.Migrate();
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("?? Could not connect to the database: " + ex.Message);
        DbStatus.DbAvailable = false;
    }
}

app.Run();