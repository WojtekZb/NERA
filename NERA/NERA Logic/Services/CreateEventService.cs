using Domain.Entities;
using Domain.Interfaces;

namespace Logic.Services
{
    public class CreateEventService
    {
        private readonly ICreateEventRepo _repo;

        public CreateEventService(ICreateEventRepo repo)
        {
            _repo = repo;
        }

        public async Task<Event> CreateEventAsync(Event ev)
        {
            var evenement = new Event
            {
                Id = ev.Id,
                Title = ev.Title,
                Date = ev.Date,
                Description = ev.Description,
                Cost = ev.Cost,
                Capacity = ev.Capacity,
                Location = ev.Location
            };
            return await _repo.SaveAsync(evenement);
        }
    }
}