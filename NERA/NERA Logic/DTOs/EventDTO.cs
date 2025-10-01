using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Logic.DTOs
{
    public class EventDTO
    {
        [Required(ErrorMessage = "Event Name is required.")]
        public string Name { get; set; } = string.Empty;
        [Required(ErrorMessage = "Event Date is required")]
        public DateTime Date { get; set; }
        [Required(ErrorMessage = "Event Time is required.")]
        public DateTime Time { get; set; }
        [Required(ErrorMessage = "Event Location is required.")]
        public string Location { get; set; } = string.Empty;
        [Required(ErrorMessage = "Event Cost is required.")]
        public double Cost { get; set; }
        [Required(ErrorMessage = "Maximum amount of people is required.")]
        public int MaxPeople { get; set; }
        [Required(ErrorMessage = "Event Description is required.")]
        public string Description { get; set; } = string.Empty;
    }
}
