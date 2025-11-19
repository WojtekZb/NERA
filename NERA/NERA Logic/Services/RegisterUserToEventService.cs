using Domain.Entities;
using Domain.Interfaces;

namespace Logic.Services
{
    public class RegisterUserToEventService
    {
        private readonly IRegisterUserToEventRepo _repo;

        public RegisterUserToEventService(IRegisterUserToEventRepo repo)
        {
            _repo = repo;
        }

        public async Task RegisterForEvent(string UserSub, int EventId)
        {
            await _repo.RegisterUserAsync(UserSub, EventId);
        }
    }
}
