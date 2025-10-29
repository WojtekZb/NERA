using Domain.Entities;
using Domain.Interfaces;

namespace Logic.Services
{
    public class UpdateEventService
    {
        private readonly ICreateEventRepo _repo;

        public UpdateEventService(ICreateEventRepo repo)
        {
            _repo = repo;
        }
        public async Task<Event?> GetByIdAsync(int id)
        {
            // Assuming the repository has a method to get an event by ID
            return await _repo.GetByIdAsync(id);
        }
        public async Task<bool> UpdateEventAsync(Event ev)
        {
            var evenement = new Event
            {
                Id = ev.Id,
                Title = ev.Title,
                StartDate = ev.StartDate,
                EndDate = ev.EndDate,
                Description = ev.Description,
                Cost = ev.Cost,
                Capacity = ev.Capacity,
                Location = ev.Location,
                Status = ev.Status
            };
            await _repo.UpdateEventAsync(evenement);
            return true;
        }

    }
}


