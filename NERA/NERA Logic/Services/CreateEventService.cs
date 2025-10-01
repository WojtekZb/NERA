using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Logic.Interfaces;
using Logic.DTOs;
using System.Security.Cryptography.X509Certificates;

namespace NERA_Logic.Services
{
    public class CreateEventService : ICreateEventService
    {
        public Event CreateEvent(EventDTO eventdto)
        {
            Event evenement = new Event
            {
                Title = eventdto.Name,
                Date = eventdto.Date,
                Description = eventdto.Description,
                Cost = eventdto.Cost,
                Capacity = eventdto.MaxPeople,
                Location = eventdto.Location
            };
            return evenement;

        }
    }
}
