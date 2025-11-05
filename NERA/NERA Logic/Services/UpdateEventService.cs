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
            try
            {
                return await _repo.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine("⚠️ Database not reachable: " + ex.Message);
                return null;
            }
        }
        public async Task<bool> UpdateEventAsync(Event ev)
        {
            try
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
                    CGI = ev.CGI,
                    Adress = ev.Adress,
                    Status = ev.Status
                };
                await _repo.UpdateEventAsync(evenement);
                return true;
            }
            catch (KeyNotFoundException)
            {
                //the event was not found in the database
                return false;
            }

        }
    }
}


