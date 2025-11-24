
using Auth0.AspNetCore.Authentication;
using Data;
using Data.Repositories;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Configuration;
using Logic.Services;
using Logic.SimpleMailTransferProtocol;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllers();

// Get Auth0 settings from configuration
var auth0Section = builder.Configuration.GetSection("Auth0");

// Configure Auth0 authentication
builder.Services
    .AddAuth0WebAppAuthentication(options =>
    {
        options.Domain = auth0Section["Domain"];
        options.ClientId = auth0Section["ClientId"];
        options.ClientSecret = auth0Section["ClientSecret"];
        options.Scope = "openid profile email"; // ensures email claim is available
    });

builder.Services.Configure<CookieAuthenticationOptions>(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.SlidingExpiration = true;
});

// Database connection
builder.Services.AddDbContext<AppDbContext>(opts =>
    opts.UseSqlServer(builder.Configuration.GetConnectionString("DbConnection")));

// Configure SMTP settings from appsettings.json
builder.Services.Configure<SmtpSettings>(
    builder.Configuration.GetSection("Email"));

// Register Email Sender (DI)
builder.Services.AddScoped<IEmailSender>(sp =>
{
    var options = sp.GetRequiredService<IOptions<SmtpSettings>>().Value;
    return new SmtpEmailSender(options);
});

// Register Repositories and Services
builder.Services.AddScoped<ICreateEventRepo, CreateEventRepo>();
builder.Services.AddScoped<CreateEventService>();
builder.Services.AddScoped<UpdateEventService>();
builder.Services.AddScoped<IRegisterUserToEventRepo, RegisterUserToEventRepo>();
builder.Services.AddScoped<RegisterUserToEventService>();

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

// Optional: Check DB connectivity
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    try
    {
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
