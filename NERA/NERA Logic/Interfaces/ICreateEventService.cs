using Domain.Entities;
using Logic.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Interfaces
{
    public interface ICreateEventService
    {
        public Event CreateEvent(EventDTO eventdto);
    }
}
