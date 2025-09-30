using NERA_Logic.Entities;
using NERA_Logic.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NERA_Logic.Interfaces
{
    public interface ICreateEventService
    {
        public Event CreateEvent(EventDTO eventdto);
    }
}
