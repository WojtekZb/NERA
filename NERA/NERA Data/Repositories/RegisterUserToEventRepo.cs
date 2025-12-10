using Data;
using Domain.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

public class RegisterUserToEventRepo : IRegisterUserToEventRepo
{
    private readonly AppDbContext _context;

    public RegisterUserToEventRepo(AppDbContext context)
    {
        _context = context;
    }

    public async Task RegisterUserAsync(string userId, int eventId, byte[] qr, bool attandance)
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
            Qr = qr,
            Attandance = attandance
        };

        _context.EventRegistration.Add(registration);
        await _context.SaveChangesAsync();
    }

    public async Task ChangeAttandance(string userId, int eventId)
    {
        try
        {
            Console.WriteLine($"Querying user={userId}, event={eventId}");

            var registration = await _context.EventRegistration.FirstOrDefaultAsync(er => er.UserSub == userId && er.EventId == eventId);

            Console.WriteLine(registration == null ? "No record found" : "Record found");

            if (registration != null)
            {
                registration.Attandance = true;
                await _context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("🔥 EXCEPTION 🔥");
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
            throw;
        }
    }

    public async Task<byte[]?> GetQrCodeAsync(string userId, int eventId)
    {
        var registration = await _context.EventRegistration
            .AsNoTracking()
            .FirstOrDefaultAsync(er => er.UserSub == userId && er.EventId == eventId);

        return registration?.Qr;
    }

}