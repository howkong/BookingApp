﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingApp.Business.Operations.Feature.Dtos;
using BookingApp.Business.Operations.Hotel.Dtos;
using BookingApp.Business.Types;
using BookingApp.Data.Entities;
using BookingApp.Data.Repositories;
using BookingApp.Data.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace BookingApp.Business.Operations.Hotel
{
    public class HotelManager : IHotelService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<HotelEntity> _hotelRepository;
        private readonly IRepository<HotelFeatureEntity> _hotelFeatureRepository;

        public HotelManager(IUnitOfWork unitOfWork, IRepository<HotelEntity> hotelRepository, IRepository<HotelFeatureEntity> hotelFeatureRepository)
        {
            _unitOfWork = unitOfWork;
            _hotelRepository = hotelRepository;
            _hotelFeatureRepository = hotelFeatureRepository;
        }

        public async Task<ServiceMessage> AddHotel(AddHotelDto hotel)
        {
            var hasHotel = _hotelRepository.GetAll(x => x.Name.ToLower() == hotel.Name.ToLower()).Any();

            if (hasHotel)
            {
                return new ServiceMessage()
                {
                    IsSucceed = false,
                    Message = "Bu otel zaten sistemde mevcut."
                };
            }

            await _unitOfWork.BeginTransaction();

            var hotelEntity = new HotelEntity()
            {
                Name = hotel.Name,
                Stars = hotel.Stars,
                Location = hotel.Location,
                AccomodationType = hotel.AccomodationType,
            };

            _hotelRepository.Add(hotelEntity);

            try
            {
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw new Exception("Otel kaydı sırasında bir sorunla karşılaşıldı.");
            }

            foreach (var featureId in hotel.FeatureIds)
            {
                var hotelFeature = new HotelFeatureEntity()
                {
                    HotelId = hotelEntity.Id,
                    FeatureId = featureId,
                };

                _hotelFeatureRepository.Add(hotelFeature);
            }

            try
            {
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransaction();

            }
            catch (Exception)
            {
                await _unitOfWork.RollBackTransaction();
                throw new Exception("Otel özellikleri eklenirken bir hatayla katşılaşıldı, süreç başa sarıldı.");
            }

            return new ServiceMessage()
            {
                IsSucceed = true,
            };
        }

        public async Task<ServiceMessage> AdjustHotelStars(int id, int changeTo)
        {
            var hotel = _hotelRepository.GetById(id);

            if (hotel is null)
            {
                return new ServiceMessage 
                { 
                    IsSucceed = false,
                    Message = "Bu id ile eşleşen otel bulunamadı."
                };
            }
            hotel.Stars = changeTo;

            _hotelRepository.Update(hotel);

            try
            {
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw new Exception("Yıldız sayısı değiştirilirken bir hata oluştu.");
            }

            return new ServiceMessage()
            {
                IsSucceed = true
            };
        }

        public async Task<ServiceMessage> DeleteHotel(int id)
        {
            var hotel = _hotelRepository.GetById(id);

            if (hotel == null)
            {
                return new ServiceMessage()
                {
                    IsSucceed = false,
                    Message = "Silinmek istenen otel bulunamadı."
                };
            }

            _hotelRepository.Delete(id);

            try
            {
                await _unitOfWork.SaveChangesAsync();

            }
            catch (Exception)
            {

                throw new Exception("Silme işlemi sırasında bir hata oldu.");
            }

            return new ServiceMessage()
            {
                IsSucceed = true
            };
        }

        public async Task<HotelDto> GetHotel(int id)
        {
            var hotel = await _hotelRepository.GetAll(x => x.Id == id)
                .Select(x => new HotelDto()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Stars = x.Stars,
                    Location = x.Location,
                    AccomodationType = x.AccomodationType,
                    Features = x.HotelFeatures.Select(x => new HotelFeatureDto
                    {
                        Id = x.Id,
                        Title = x.Feature.Title
                    }).ToList()
                }).FirstOrDefaultAsync();

            return hotel;
        }

        public async Task<List<HotelDto>> GetHotels()
        {
            var hotels = await _hotelRepository.GetAll()
               .Select(x => new HotelDto()
               {
                   Id = x.Id,
                   Name = x.Name,
                   Stars = x.Stars,
                   Location = x.Location,
                   AccomodationType = x.AccomodationType,
                   Features = x.HotelFeatures.Select(x => new HotelFeatureDto
                   {
                       Id = x.Id,
                       Title = x.Feature.Title
                   }).ToList()
               }).ToListAsync();

            return hotels;
        }

        public async Task<ServiceMessage> UpdateHotel(UpdateHotelDto hotel)
        {
            var hotelEntity = _hotelRepository.GetById(hotel.Id);

            if (hotelEntity is null)
            {
                return new ServiceMessage()
                {
                    IsSucceed = false,
                    Message = "Otel bulunamadı."
                };
            }

            await _unitOfWork.BeginTransaction();

            hotelEntity.Name = hotel.Name;
            hotelEntity.Stars = hotel.Stars;
            hotelEntity.Location    = hotel.Location;
            hotelEntity.AccomodationType = hotel.AccomodationType;

            _hotelRepository.Update(hotelEntity);

            try
            {
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception)
            {
                await _unitOfWork.RollBackTransaction();

                throw new Exception("Otel bilgileri güncellebirken bir hata ile karşılaşıldı.");
            }

            var hotelFeatures = _hotelFeatureRepository.GetAll(x => x.HotelId == x.HotelId).ToList();
           
            foreach (var hotelFeature in hotelFeatures)
            {
                _hotelFeatureRepository.Delete(hotelFeature, false); // HARD DELETE
            }

            foreach (var featureId in hotel.FeatureIds)
            {
                var hotelFeature = new HotelFeatureEntity()
                {
                    HotelId = hotelEntity.Id,
                    FeatureId = featureId
                };

                _hotelFeatureRepository.Add(hotelFeature);
            }

            try
            {
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransaction();
            }
            catch (Exception)
            {
                await _unitOfWork.RollBackTransaction();
                throw new Exception("Otel bilgileri güncellenirken bir hata oluştu. İşlemleri geriye alınıyor.");
            }

            return new ServiceMessage()
            {
                IsSucceed = true,
            };
        }
    }
}
