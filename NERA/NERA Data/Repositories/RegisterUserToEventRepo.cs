using Data;
using Domain.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

public class RegisterUserToEventRepo : IRegisterUserToEventRepo
{
    private readonly AppDbContext _context;

    public RegisterUserToEventRepo(AppDbContext context)
    {
        _context = context;
    }

    public async Task RegisterUserAsync(int userId, int eventId)
    {
        var user = await _context.Users.FindAsync(userId);
        var evnt = await _context.Events.FindAsync(eventId);

        if (user == null || evnt == null)
            throw new InvalidOperationException("User or Event not found.");

        var alreadyRegistered = await _context.Registrations
            .AnyAsync(r => r.UserId == userId && r.EventId == eventId);

        if (alreadyRegistered)
            throw new InvalidOperationException("User already registered for this event.");

        var registration = new Registration
        {
            UserId = userId,
            EventId = eventId,
            Status = "Delete Status",
            Time = DateTime.UtcNow
        };

        _context.Registrations.Add(registration);
        await _context.SaveChangesAsync();
    }


}