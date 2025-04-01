using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingApp.Data.Enums;

namespace BookingApp.Business.Operations.Feature.Dtos
{
    public class UpdateHotelDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? Stars { get; set; }
        public string Location { get; set; }       
        public AccomodationType AccomodationType { get; set; }
        public List<int> FeatureIds { get; set; }

    }
}
