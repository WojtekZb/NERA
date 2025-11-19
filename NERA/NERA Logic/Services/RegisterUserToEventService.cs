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

        public void RegisterForEvent(EventRegistration er)
        {
            _repo.RegisterUserAsync(er.UserSub, er.EventId);
        }
    }
}
