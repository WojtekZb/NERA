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

        public async Task<Event> CreateEventAsync(Event eventdto)
        {
            var evenement = new Event
            {
                Title = eventdto.Title,
                Date = eventdto.Date,
                Description = eventdto.Description,
                Cost = eventdto.Cost,
                Capacity = eventdto.Capacity,
                Location = eventdto.Location
            };

            return await _repo.SaveAsync(evenement);
        }
    }
}