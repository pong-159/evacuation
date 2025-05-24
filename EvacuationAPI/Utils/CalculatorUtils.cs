using EvacuationAPI.Models;

namespace EvacuationAPI.Utils;

public static class CalculatorUtils
{

    public static double ConvertKphToMps(double kph)
    {
        return kph * 0.277778;
    }

    public static double GetDistance(LocationCoordinate location1, LocationCoordinate location2)
    {
        // Haversine formula
        return Haversine(location1.Latitude, location2.Latitude, location1.Longitude, location2.Longitude);
    }
    
    public static double GetDistance(Vehicles vehicle, EvacuationZones evacuationZones)
    {
        // return 100;
        // Haversine formula
        return Haversine(vehicle.Latitude, evacuationZones.Latitude, vehicle.Longitude, evacuationZones.Longitude);
    }
    
    public static double GetDistance(double lat1, double lat2, double lon1, double lon2)
    {
        // return 100;
        // Haversine formula
        return Haversine(lat1, lat2, lon1, lon2);
    }
    
    //meter
    private static double Haversine(double lat1, double lat2, double lon1, double lon2)
    {
        const double r = 6378100; // meters
            
        var sdlat = Math.Sin((lat2 - lat1) / 2);
        var sdlon = Math.Sin((lon2 - lon1) / 2);
        var q = sdlat * sdlat + Math.Cos(lat1) * Math.Cos(lat2) * sdlon * sdlon;
        var d = 2 * r * Math.Asin(Math.Sqrt(q));

        return d;
    }
}