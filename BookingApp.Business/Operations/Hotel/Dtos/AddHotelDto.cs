using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingApp.Data.Enums;

namespace BookingApp.Business.Operations.Hotel.Dtos
{
    public class AddHotelDto
    {
        [Required]
        public string Name { get; set; }
        public int? Stars { get; set; }
        [Required]
        public string Location { get; set; }
        [Required]
        public AccomodationType AccomodationType { get; set; }

        public List<int> FeatureIds { get; set; }
    }
}
