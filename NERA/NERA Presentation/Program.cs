using Data;
using Data.Repositories;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Configuration;
using Logic.Services;
using Logic.SimpleMailTransferProtocol;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllers();


// Register DbContext
builder.Services.AddDbContext<AppDbContext>(opts =>
    opts.UseSqlServer(builder.Configuration.GetConnectionString("DbConnection")));

builder.Services.Configure<SmtpSettings>(
    builder.Configuration.GetSection("Email"));

builder.Services.AddScoped<IEmailSender>(sp =>
{
    var options = sp.GetRequiredService<IOptions<SmtpSettings>>().Value;
    return new SmtpEmailSender(options);
});
// Register Repositories and Services
builder.Services.AddScoped<ICreateEventRepo, CreateEventRepo>();
builder.Services.AddScoped<CreateEventService>();
builder.Services.AddScoped<UpdateEventService>();
builder.Services.AddRazorPages();

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