
using Domain.Entities;

namespace Domain.Interfaces
{
    public interface ICreateEventRepo
    {
        Task<Event> SaveAsync(Event evenement);
        Task UpdateEventAsync(Event evenement);
        Task<Event?> GetByIdAsync(int id);
        Task<Event> SaveChangeAsync(Event evenement);
        Task RegisterUserToEventAsync(string userId, int eventId);


    }
}
