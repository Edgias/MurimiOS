using Murimi.API.Interfaces;
using Murimi.API.Models.Requests;
using Murimi.API.Models.Responses;
using Murimi.ApplicationCore;
using Murimi.ApplicationCore.Entities;

namespace Murimi.API.ObjectMappers
{
    public class LocationMapper : IMapper<Location, LocationRequest, LocationResponse>
    {
        public Location Map(LocationRequest request)
        {
            Address locationAddress = new(request.Street1, request.Street2, 
                request.City, request.State, request.ZipCode, request.Country);

            Location entity = new(request.Name, request.Latitude, request.Longitude, locationAddress);

            return entity;
        }

        public LocationResponse Map(Location entity)
        {
            LocationResponse response = new()
            {
                Id = entity.Id,
                IsActive = entity.IsActive,
                City = entity.LocationAddress?.City,
                Country = entity.LocationAddress?.Country,
                Latitude = entity.Latitude,
                Longitude = entity.Longitude,
                Name = entity.Name,
                State = entity.LocationAddress?.State,
                Street1 = entity.LocationAddress?.Street1,
                Street2 = entity.LocationAddress?.Street2,
                ZipCode = entity.LocationAddress?.ZipCode
            };

            return response;
        }

        public void Map(Location entity, LocationRequest request)
        {
            Address locationAddress = new(request.Street1, request.Street2,
                request.City, request.State, request.ZipCode, request.Country);

            entity.UpdateDetails(request.Name, request.Latitude, request.Longitude, locationAddress);
        }
    }
}
