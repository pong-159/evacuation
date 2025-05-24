namespace EvacuationAPI.Models;

public class LocationCoordinate

{
    public LocationCoordinate()
    {
    }

    public LocationCoordinate(double latitude, double longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }

    public LocationCoordinate(LocationCoordinate coordinate)
    {
        Latitude = coordinate.Latitude;
        Longitude = coordinate.Longitude;
    }

    public double Latitude { get; set; }
    public double Longitude { get; set; }
}