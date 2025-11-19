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

    public async Task RegisterUserAsync(string userId, int eventId)
    {
        var evnt = await _context.Event.FindAsync(eventId);

        if (evnt == null)
            throw new InvalidOperationException("Event not found.");

        var alreadyRegistered = await _context.EventRegistration
            .AnyAsync(r => r.UserSub == userId && r.EventId == eventId);

        if (alreadyRegistered)
            throw new InvalidOperationException("User already registered for this event.");

        var registration = new EventRegistration
        {
            UserSub = userId,
            EventId = eventId,
        };

        _context.EventRegistration.Add(registration);
        await _context.SaveChangesAsync();
    }


}