using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingApp.Business.Operations.Feature.Dtos;
using BookingApp.Business.Operations.Hotel.Dtos;
using BookingApp.Business.Types;

namespace BookingApp.Business.Operations.Hotel
{
    public interface IHotelService
    {
        Task<ServiceMessage> AddHotel(AddHotelDto hotel);
        Task<HotelDto> GetHotel(int id);
        Task<List<HotelDto>> GetHotels();
        Task<ServiceMessage> AdjustHotelStars(int id, int changeTo);
        Task<ServiceMessage> DeleteHotel(int id);

        Task<ServiceMessage> UpdateHotel(UpdateHotelDto hotel);
        
    }
}
    