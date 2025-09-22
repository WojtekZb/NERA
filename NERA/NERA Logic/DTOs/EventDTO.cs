using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace NERA_Logic.DTOs
{
    public class EventDTO
    {
        public string Name { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Location { get; set; } = string.Empty;
        public double Cost { get; set; }
        public int MaxPeople { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
