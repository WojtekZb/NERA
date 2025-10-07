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
                Title = ev.Title,
                Date = ev.Date,
                Description = ev.Description,
                Cost = ev.Cost,
                Capacity = ev.Capacity,
                Location = ev.Location
            };

            Console.WriteLine("CreateEventService called");

            return await _repo.SaveAsync(evenement);
        }
    }
}