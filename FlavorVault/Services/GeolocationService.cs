using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.ApplicationModel;
using FlavorVault.Models;

namespace FlavorVault.Services;

/// <summary>
/// Geolocation service
/// Provides location acquisition, simulated nearby place data, distance calculation, and compass direction conversion
/// </summary>
public class GeolocationService
{
    #region Simulated place data (about 20 locations, distributed across major cities)

    private static readonly List<(string Name, string Address, double Lat, double Lon, string Category, string Region, string Feature, int StarRating)> MockPlaces =
    [
        // === Sichuan ===
        ("Kuanzhai Alley", "Qingyang District, Chengdu", 30.6697, 104.0559, "FoodStreet", "Sichuan", "Sichuan snacks hub: San Da Pao, Long Chaoshou, Bobo Chicken", 5),
        ("Hongyadong", "Yuzhong District, Chongqing", 29.5628, 106.5828, "Landmark", "Sichuan", "Chongqing hotpot, Xiaomian, Hot and sour noodles", 5),
        ("Jinli Ancient Street", "Wuhou District, Chengdu", 30.6466, 104.0484, "FoodStreet", "Sichuan", "Zhang Fei Beef, Tangyou Guozi, Dan Dan Noodles", 4),

        // === Cantonese ===
        ("Shangxiajiu Pedestrian Street", "Liwan District, Guangzhou", 23.1186, 113.2433, "Shopping", "Cantonese", "Cantonese dim sum street: Rice rolls, Shrimp dumplings, Double-skin milk", 4),
        ("Temple Street Night Market", "Yau Tsim Mong District, Hong Kong", 22.3069, 114.1700, "NightMarket", "Cantonese", "Hong Kong claypot rice, Fish balls, Pan-fried three treasures", 4),

        // === Jiangnan ===
        ("City God Temple", "Huangpu District, Shanghai", 31.2275, 121.4920, "Landmark", "Jiangnan", "Birthplace of Nanxiang Xiaolongbao, Crab shell cake, Shengjian", 5),
        ("Hefang Street", "Shangcheng District, Hangzhou", 30.2463, 120.1700, "FoodStreet", "Jiangnan", "West Lake Vinegar Fish, Dongpo Pork, Longjing Shrimp", 4),
        ("Confucius Temple Qinhuai River", "Qinhuai District, Nanjing", 32.0225, 118.7875, "Landmark", "Jiangnan", "Duck blood vermicelli soup, Salted duck, Rice cakes", 4),

        // === Northern ===
        ("Guijie (Ghost Street)", "Dongcheng District, Beijing", 39.9413, 116.4265, "FoodStreet", "Northern", "Late-night food paradise: Spicy crayfish, Grilled fish", 5),
        ("Nanluoguxiang", "Dongcheng District, Beijing", 39.9359, 116.4031, "FoodStreet", "Northern", "Beijing hutong snacks: Zhajiangmian, Douzhi", 4),
        ("Wangfujing Snack Street", "Dongcheng District, Beijing", 39.9137, 116.4103, "FoodStreet", "Northern", "Traditional Beijing snacks: Tanghulu, Baodu", 4),

        // === Northwest ===
        ("Huimin Street (Muslim Quarter)", "Beilin District, Xi'an", 34.2610, 108.9430, "Heritage", "Northwest", "Northwest flavors street: Pita with lamb soup, Roujiamo", 5),
        ("Zhengning Road Night Market", "Chengguan District, Lanzhou", 36.0580, 103.8340, "NightMarket", "Northwest", "Lanzhou beef noodles, Milk egg fermented rice wine", 4),

        // === Japan ===
        ("Tsukiji Market", "Chuo Ward, Tokyo", 35.6654, 139.7707, "Landmark", "Japan", "Seafood tempura, Sushi, Tamagoyaki", 5),
        ("Dotonbori", "Chuo Ward, Osaka", 34.6686, 135.5010, "FoodStreet", "Japan", "Okonomiyaki, Takoyaki, Ramen", 5),

        // === Korea ===
        ("Myeongdong Food Street", "Jung District, Seoul", 37.5636, 126.9834, "NightMarket", "Korea", "Korean fried chicken, BBQ, Spicy rice cakes", 4),
        ("Gwangjang Market", "Jongno District, Seoul", 37.5704, 126.9917, "Heritage", "Korea", "Mung bean pancakes, Gimbap, Yukhoe", 5),

        // === SoutheastAsia ===
        ("Khao San Road", "Banglapphao, Bangkok", 13.7584, 100.4975, "NightMarket", "SoutheastAsia", "Thai street food: Tom Yum, Mango sticky rice", 4),
        ("Bui Vien Street", "District 1, Ho Chi Minh City", 10.7690, 106.6938, "FoodStreet", "SoutheastAsia", "Vietnamese pho, Banh mi, Spring rolls", 4),

        // === Western ===
        ("Boqueria Market", "La Rambla, Barcelona", 41.3816, 2.1719, "Heritage", "Western", "Spanish Tapas, Seafood paella, Iberian ham", 5),
    ];

    #endregion

    /// <summary>
    /// Get the current device location
    /// </summary>
    /// <returns>Current location, or null on failure</returns>
    public async Task<Location?> GetCurrentLocationAsync()
    {
        try
        {
            var status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            if (status != PermissionStatus.Granted)
            {
                return null;
            }

            var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
            var location = await Geolocation.Default.GetLocationAsync(request);
            return location;
        }
        catch (FeatureNotSupportedException)
        {
            return null;
        }
        catch (PermissionException)
        {
            return null;
        }
        catch (Exception)
        {
            return null;
        }
    }

    /// <summary>
    /// Get nearby places (automatically acquires current location)
    /// </summary>
    public async Task<List<NearbyPlace>> GetNearbyPlacesAsync(double radiusKm = 500)
    {
        try
        {
            var location = await GetCurrentLocationAsync();
            if (location == null)
            {
                location = new Location(39.9042, 116.4074);
            }
            return GetNearbyPlaces(location, radiusKm);
        }
        catch (Exception)
        {
            return GetNearbyPlaces(new Location(39.9042, 116.4074), radiusKm);
        }
    }

    /// <summary>
    /// Get nearby places (based on hardcoded mock data), sorted by distance
    /// </summary>
    /// <param name="center">Center point coordinates</param>
    /// <param name="radiusKm">Search radius in kilometers, default 50km</param>
    /// <returns>List of nearby places sorted by distance</returns>
    public List<NearbyPlace> GetNearbyPlaces(Location center, double radiusKm = 50)
    {
        try
        {
            var centerLoc = new Location(center.Latitude, center.Longitude);

            var results = new List<NearbyPlace>();

            foreach (var place in MockPlaces)
            {
                var placeLoc = new Location(place.Lat, place.Lon);
                var distance = CalculateDistance(center, placeLoc);

                if (distance <= radiusKm)
                {
                    results.Add(new NearbyPlace
                    {
                        Name = place.Name,
                        Address = place.Address,
                        Distance = Math.Round(distance, 1),
                        Type = GetCategoryShortName(place.Category),
                        Location = placeLoc,
                        Category = place.Category,
                        Region = place.Region,
                        Feature = place.Feature,
                        StarRating = place.StarRating
                    });
                }
            }

            // Sort by distance ascending
            return results.OrderBy(p => p.Distance).ToList();
        }
        catch (Exception)
        {
            return [];
        }
    }

    /// <summary>
    /// Calculate distance between two points (kilometers) using the Haversine formula
    /// </summary>
    /// <param name="a">Point A</param>
    /// <param name="b">Point B</param>
    /// <returns>Distance in kilometers</returns>
    public double CalculateDistance(Location a, Location b)
    {
        try
        {
            // Use built-in MAUI distance calculation
            return Location.CalculateDistance(a, b, DistanceUnits.Kilometers);
        }
        catch (Exception)
        {
            // Fallback to manual Haversine calculation
            return HaversineDistance(a.Latitude, a.Longitude, b.Latitude, b.Longitude);
        }
    }

    /// <summary>
    /// Convert an angle to a compass direction text
    /// </summary>
    /// <param name="heading">Magnetic north heading angle (0-360 degrees)</param>
    /// <returns>Compass direction text: N/NE/E/SE/S/SW/W/NW</returns>
    public string GetCompassDirection(double heading)
    {
        try
        {
            // Ensure the angle is within 0-360 range
            heading = ((heading % 360) + 360) % 360;

            // Each direction spans 45 degrees, with 22.5 degrees as the boundary
            // N: 337.5 ~ 22.5
            // NE: 22.5 ~ 67.5
            // E: 67.5 ~ 112.5
            // SE: 112.5 ~ 157.5
            // S: 157.5 ~ 202.5
            // SW: 202.5 ~ 247.5
            // W: 247.5 ~ 292.5
            // NW: 292.5 ~ 337.5

            if (heading >= 337.5 || heading < 22.5)
                return "N";
            if (heading >= 22.5 && heading < 67.5)
                return "NE";
            if (heading >= 67.5 && heading < 112.5)
                return "E";
            if (heading >= 112.5 && heading < 157.5)
                return "SE";
            if (heading >= 157.5 && heading < 202.5)
                return "S";
            if (heading >= 202.5 && heading < 247.5)
                return "SW";
            if (heading >= 247.5 && heading < 292.5)
                return "W";
            if (heading >= 292.5 && heading < 337.5)
                return "NW";

            return "N";
        }
        catch (Exception)
        {
            return "N";
        }
    }

    /// <summary>
    /// Get category short name (for place card icon text)
    /// </summary>
    private static string GetCategoryShortName(string category)
    {
        return category switch
        {
            "FoodStreet" => "F",
            "Heritage" => "H",
            "NightMarket" => "N",
            "Shopping" => "S",
            "Landmark" => "L",
            _ => "F"
        };
    }

    /// <summary>
    /// Haversine formula to calculate distance between two points
    /// </summary>
    private static double HaversineDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double EarthRadiusKm = 6371.0;

        var dLat = DegreesToRadians(lat2 - lat1);
        var dLon = DegreesToRadians(lon2 - lon1);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return EarthRadiusKm * c;
    }

    private static double DegreesToRadians(double degrees)
    {
        return degrees * Math.PI / 180.0;
    }
}
