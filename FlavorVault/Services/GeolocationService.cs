using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.ApplicationModel;
using FlavorVault.Models;

namespace FlavorVault.Services;

/// <summary>
/// 地理位置服务
/// 提供定位获取、模拟附近地点数据、距离计算、指南针方位转换
/// </summary>
public class GeolocationService
{
    #region 模拟地点数据（约20个，分布在中国各大城市）

    private static readonly List<(string Name, string Address, double Lat, double Lon, string Category, string Region, string Feature, int StarRating)> MockPlaces =
    [
        // === 川渝 ===
        ("宽窄巷子", "成都市青羊区宽窄巷子", 30.6697, 104.0559, "美食街", "川渝", "川渝小吃汇聚，三大炮、龙抄手、钵钵鸡", 5),
        ("洪崖洞", "重庆市渝中区嘉陵江滨江路", 29.5628, 106.5828, "景点周边", "川渝", "重庆火锅、小面、酸辣粉", 5),
        ("锦里古街", "成都市武侯区武侯祠大街", 30.6466, 104.0484, "美食街", "川渝", "张飞牛肉、糖油果子、担担面", 4),

        // === 粤港 ===
        ("上下九步行街", "广州市荔湾区上下九路", 23.1186, 113.2433, "商圈", "粤港", "粤式茶点一条街，肠粉、虾饺、双皮奶", 4),
        ("庙街夜市", "香港油尖旺区庙街", 22.3069, 114.1700, "夜市", "粤港", "港式煲仔饭、鱼蛋、煎酿三宝", 4),

        // === 江南 ===
        ("城隍庙", "上海市黄浦区方浜中路", 31.2275, 121.4920, "景点周边", "江南", "南翔小笼发源地，蟹壳黄、生煎", 5),
        ("河坊街", "杭州市上城区河坊街", 30.2463, 120.1700, "美食街", "江南", "西湖醋鱼、东坡肉、龙井虾仁", 4),
        ("夫子庙秦淮河", "南京市秦淮区贡院街", 32.0225, 118.7875, "景点周边", "江南", "鸭血粉丝汤、盐水鸭、糕团", 4),

        // === 北方 ===
        ("簋街", "北京市东城区东直门内大街", 39.9413, 116.4265, "美食街", "北方", "夜宵圣地，麻辣小龙虾、烤鱼", 5),
        ("南锣鼓巷", "北京市东城区南锣鼓巷", 39.9359, 116.4031, "美食街", "北方", "北京胡同小吃，炸酱面、豆汁", 4),
        ("王府井小吃街", "北京市东城区王府井大街", 39.9137, 116.4103, "美食街", "北方", "传统北京小吃，糖葫芦、爆肚", 4),

        // === 西北 ===
        ("回民街", "西安市碑林区西大街", 34.2610, 108.9430, "老字号", "西北", "西北风味一条街，羊肉泡馍、肉夹馍", 5),
        ("正宁路夜市", "兰州市城关区正宁路", 36.0580, 103.8340, "夜市", "西北", "兰州牛肉面、牛奶鸡蛋醪糟", 4),

        // === 日本 ===
        ("筑地市场", "东京都中央区筑地", 35.6654, 139.7707, "景点周边", "日本", "海鲜天妇罗、寿司、玉子烧", 5),
        ("道顿堀", "大阪市中央区道顿堀", 34.6686, 135.5010, "美食街", "日本", "大阪烧、章鱼烧、拉面", 5),

        // === 韩国 ===
        ("明洞小吃街", "首尔特别市中区明洞", 37.5636, 126.9834, "夜市", "韩国", "韩式炸鸡、烤肉、辣炒年糕", 4),
        ("广藏市场", "首尔特别市钟路区礼智洞", 37.5704, 126.9917, "老字号", "韩国", "绿豆煎饼、麻药紫菜饭卷、生拌牛肉", 5),

        // === 东南亚 ===
        ("考山路", "曼谷邦腊普区考山路", 13.7584, 100.4975, "夜市", "东南亚", "泰式街头美食，冬阴功、芒果糯米饭", 4),
        ("越南范五老街", "胡志明市第一郡范五老街", 10.7690, 106.6938, "美食街", "东南亚", "越南河粉、法棍三明治、春卷", 4),

        // === 欧美 ===
        ("波盖利亚市场", "巴塞罗那兰布拉大道", 41.3816, 2.1719, "老字号", "欧美", "西班牙Tapas、海鲜饭、伊比利亚火腿", 5),
    ];

    #endregion

    /// <summary>
    /// 获取当前设备位置
    /// </summary>
    /// <returns>当前位置，失败返回 null</returns>
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
    /// 获取附近地点（自动获取当前位置）
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
    /// 获取附近地点（基于硬编码模拟数据），按距离排序
    /// </summary>
    /// <param name="center">中心点坐标</param>
    /// <param name="radiusKm">搜索半径（公里），默认 50km</param>
    /// <returns>按距离排序的附近地点列表</returns>
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

            // 按距离升序排序
            return results.OrderBy(p => p.Distance).ToList();
        }
        catch (Exception)
        {
            return [];
        }
    }

    /// <summary>
    /// 计算两点间的距离（公里），使用 Haversine 公式
    /// </summary>
    /// <param name="a">点A</param>
    /// <param name="b">点B</param>
    /// <returns>距离（公里）</returns>
    public double CalculateDistance(Location a, Location b)
    {
        try
        {
            // 使用 MAUI 内置的距离计算
            return Location.CalculateDistance(a, b, DistanceUnits.Kilometers);
        }
        catch (Exception)
        {
            // 降级使用 Haversine 手动计算
            return HaversineDistance(a.Latitude, a.Longitude, b.Latitude, b.Longitude);
        }
    }

    /// <summary>
    /// 将角度转换为方位文字
    /// </summary>
    /// <param name="heading">磁北方向角（0-360度）</param>
    /// <returns>方位文字：北/东北/东/东南/南/西南/西/西北</returns>
    public string GetCompassDirection(double heading)
    {
        try
        {
            // 确保角度在 0-360 范围内
            heading = ((heading % 360) + 360) % 360;

            // 每个方位占 45 度，以 22.5 度为分界线
            // 北: 337.5 ~ 22.5
            // 东北: 22.5 ~ 67.5
            // 东: 67.5 ~ 112.5
            // 东南: 112.5 ~ 157.5
            // 南: 157.5 ~ 202.5
            // 西南: 202.5 ~ 247.5
            // 西: 247.5 ~ 292.5
            // 西北: 292.5 ~ 337.5

            if (heading >= 337.5 || heading < 22.5)
                return "北";
            if (heading >= 22.5 && heading < 67.5)
                return "东北";
            if (heading >= 67.5 && heading < 112.5)
                return "东";
            if (heading >= 112.5 && heading < 157.5)
                return "东南";
            if (heading >= 157.5 && heading < 202.5)
                return "南";
            if (heading >= 202.5 && heading < 247.5)
                return "西南";
            if (heading >= 247.5 && heading < 292.5)
                return "西";
            if (heading >= 292.5 && heading < 337.5)
                return "西北";

            return "北";
        }
        catch (Exception)
        {
            return "北";
        }
    }

    /// <summary>
    /// 获取类型简称（用于地点卡片图标文字）
    /// </summary>
    private static string GetCategoryShortName(string category)
    {
        return category switch
        {
            "美食街" => "食",
            "老字号" => "老",
            "夜市" => "夜",
            "商圈" => "商",
            "景点周边" => "景",
            _ => "食"
        };
    }

    /// <summary>
    /// Haversine 公式计算两点间距离
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
