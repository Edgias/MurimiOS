namespace Edgias.MurimiOS.Domain.Entities;

public class Location(string name, decimal? latitude, decimal? longitude,
        Address locationAddress) : BaseEntity, IAggregateRoot
{
    public string Name { get; private set; } = name;

    public decimal? Latitude { get; private set; } = latitude;

    public decimal? Longitude { get; private set; } = longitude;

    public Address LocationAddress { get; private set; } = locationAddress;

    public void Update(string name, decimal? latitude, decimal? longitude,
        Address locationAddress)
    {
        Name = name;
        Latitude = latitude;
        Longitude = longitude;
        LocationAddress = locationAddress;
    }

    public void UpdateLatitudeAndLongitude(decimal latitude, decimal longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }
}


