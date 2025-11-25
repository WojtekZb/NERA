using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IRegisterUserToEventRepo
    {
        //add qr as parameter
        Task RegisterUserAsync(string userId, int eventId, byte[] qr, bool attandance);
        Task ChangeAttandance(string userId, int attandance);
    }
}
