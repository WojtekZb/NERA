using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NERA_Logic.Entities;
using NERA_Logic.Interfaces;
using NERA_Logic.DTOs;
using System.Security.Cryptography.X509Certificates;

namespace NERA_Logic.Services
{
    public class CreateEventService : ICreateEventService
    {
        public Event CreateEvent(EventDTO eventdto)
        {
            Event evenement = new Event
            {
                Name = eventdto.Name,
                Date = eventdto.Date,
                Description = eventdto.Description,
                Cost = eventdto.Cost,
                MaxPeople = eventdto.MaxPeople,
                Location = eventdto.Location
            };
            return evenement;

        }
    }
}
