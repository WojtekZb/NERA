
using Domain.Entities;

namespace Domain.Interfaces
{
    public interface ICreateEventRepo
    {
        Task<Event> SaveAsync(Event evenement);

    }
}
