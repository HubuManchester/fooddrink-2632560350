using FlavorVault.Models;

namespace FlavorVault.Services;

/// <summary>
/// 种子数据初始化服务
/// 通过 seed_version 控制版本，仅在版本不匹配时重新填充
/// </summary>
public class SeedDataService
{
    private const string SeedVersionKey = "seed_version";
    private const string CurrentSeedVersion = "1";

    private readonly FoodEntryRepository _foodEntryRepo;
    private readonly WishItemRepository _wishItemRepo;
    private readonly CollectionRepository _collectionRepo;
    private readonly PlaceMarkRepository _placeMarkRepo;
    private readonly UserProfileRepository _userProfileRepo;

    public SeedDataService(
        FoodEntryRepository foodEntryRepo,
        WishItemRepository wishItemRepo,
        CollectionRepository collectionRepo,
        PlaceMarkRepository placeMarkRepo,
        UserProfileRepository userProfileRepo)
    {
        _foodEntryRepo = foodEntryRepo;
        _wishItemRepo = wishItemRepo;
        _collectionRepo = collectionRepo;
        _placeMarkRepo = placeMarkRepo;
        _userProfileRepo = userProfileRepo;
    }

    /// <summary>
    /// 初始化种子数据，若 seed_version 已匹配则跳过
    /// </summary>
    public async Task InitializeAsync()
    {
        try
        {
            var currentVersion = await _userProfileRepo.GetAsync(SeedVersionKey);
            if (currentVersion == CurrentSeedVersion)
            {
                System.Diagnostics.Debug.WriteLine("[SeedDataService] Seed version matched, skipping.");
                return;
            }

            System.Diagnostics.Debug.WriteLine("[SeedDataService] Seeding data...");

            await SeedFoodEntriesAsync();
            await SeedWishItemsAsync();
            await SeedCollectionsAsync();
            await SeedPlaceMarksAsync();
            await SeedUserPreferencesAsync();

            await _userProfileRepo.SetAsync(SeedVersionKey, CurrentSeedVersion);

            System.Diagnostics.Debug.WriteLine("[SeedDataService] Seeding completed.");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[SeedDataService] InitializeAsync error: {ex.Message}");
        }
    }

    private async Task SeedFoodEntriesAsync()
    {
        var entries = new List<FoodEntry>
        {
            new()
            {
                CatalogNumber = "FV-0001",
                Name = "麻婆豆腐",
                Region = "川渝",
                Rarity = "日常",
                ImagePath = "seed_mapo_tofu.jpg",
                StarRating = 5,
                PrimaryTaste = "辣",
                AromaTag = "花椒香",
                TextureTag = "软糯",
                Description = "麻婆豆腐是四川省传统名菜之一，属于川菜系。主要原料为豆腐、牛肉末（也有用猪肉末的）、辣椒和花椒等。麻来自花椒，辣来自辣椒，这道菜突出了川菜\"麻辣\"的特点，口味独特，口感顺滑。由陈麻婆于清朝同治元年（1862年）在成都首创。",
                Ingredients = "[\"豆腐\",\"牛肉末\",\"花椒粉\",\"辣椒面\",\"豆瓣酱\",\"葱姜蒜\"]",
                PriceRange = "10~30",
                CollectStatus = "已收藏",
                Latitude = 30.5728,
                Longitude = 104.0668,
                LocationName = "成都陈麻婆豆腐旗舰店",
                DiscoverDate = "2025-03-15",
                NoteText = "正宗的麻婆豆腐，花椒的麻和辣椒的辣完美融合，豆腐嫩滑入味",
                CollectionName = "无辣不欢",
                IsShowcase = true,
                CreatedAt = "2025-03-15 12:30:00",
                UpdatedAt = "2025-03-15 12:30:00"
            },
            new()
            {
                CatalogNumber = "FV-0002",
                Name = "回锅肉",
                Region = "川渝",
                Rarity = "推荐",
                ImagePath = "seed_huiguo_rou.jpg",
                StarRating = 5,
                PrimaryTaste = "咸",
                AromaTag = "酱香",
                TextureTag = "劲道",
                Description = "回锅肉是汉族传统菜肴，属中国八大菜系川菜中的一种。所谓回锅，就是再次烹调的意思。回锅肉作为一道传统川菜，在川菜中的地位是非常重要的，川菜考级经常用回锅肉作为首选菜肴。回锅肉一直被认为是川菜之首，是川菜的化身。",
                Ingredients = "[\"五花肉\",\"青蒜苗\",\"豆瓣酱\",\"甜面酱\",\"豆豉\",\"青椒\"]",
                PriceRange = "30~60",
                CollectStatus = "已收藏",
                Latitude = 30.6571,
                Longitude = 104.0657,
                LocationName = "成都蜀九香",
                DiscoverDate = "2025-04-02",
                NoteText = "肥而不腻的完美代表，酱香浓郁，下饭神器",
                CollectionName = null,
                IsShowcase = false,
                CreatedAt = "2025-04-02 18:45:00",
                UpdatedAt = "2025-04-02 18:45:00"
            },
            new()
            {
                CatalogNumber = "FV-0003",
                Name = "虾饺",
                Region = "粤港",
                Rarity = "日常",
                ImagePath = "seed_xiajiao.jpg",
                StarRating = 4,
                PrimaryTaste = "鲜",
                AromaTag = "无",
                TextureTag = "弹牙",
                Description = "虾饺是广东地区著名的传统小吃，属粤菜系。虾饺始创于20世纪初的广州五凤乡，以一层澄面皮包着一至两只虾为主馅。以鲜虾为主料，蒸熟后皮薄如纸，晶莹剔透，肉汁鲜美。是广式早茶中的经典点心。",
                Ingredients = "[\"鲜虾仁\",\"澄面\",\"猪油\",\"竹笋丝\",\"胡椒粉\"]",
                PriceRange = "10~30",
                CollectStatus = "已收藏",
                Latitude = 23.1291,
                Longitude = 113.2644,
                LocationName = "广州陶陶居",
                DiscoverDate = "2025-02-20",
                NoteText = "皮薄如纸，虾仁弹牙鲜美，早茶必点",
                CollectionName = "街头传奇",
                IsShowcase = false,
                CreatedAt = "2025-02-20 09:15:00",
                UpdatedAt = "2025-02-20 09:15:00"
            },
            new()
            {
                CatalogNumber = "FV-0004",
                Name = "烧鹅",
                Region = "粤港",
                Rarity = "推荐",
                ImagePath = "seed_shao_e.jpg",
                StarRating = 5,
                PrimaryTaste = "复合",
                AromaTag = "烟熏",
                TextureTag = "酥脆",
                Description = "烧鹅是广东传统的烧烤肉食，粤菜系。烧鹅源于烧鸭，将酱油、盐、糖、五香粉等腌料填入鹅肚，缝合后吹气使皮肉分离，再涂上麦芽糖水晾干，最后以高温炉火烤制而成。皮脆肉嫩，肥而不腻，是广东人宴客的上等菜式。",
                Ingredients = "[\"黑鬃鹅\",\"五香粉\",\"沙姜粉\",\"麦芽糖\",\"八角\",\"桂皮\"]",
                PriceRange = "60~100",
                CollectStatus = "想尝试",
                Latitude = 22.3193,
                Longitude = 114.1694,
                LocationName = "香港深井烧鹅",
                DiscoverDate = "2025-05-10",
                NoteText = "一直想尝试正宗深井烧鹅，听说皮脆肉嫩非常美味",
                CollectionName = null,
                IsShowcase = false,
                CreatedAt = "2025-05-10 19:30:00",
                UpdatedAt = "2025-05-10 19:30:00"
            },
            new()
            {
                CatalogNumber = "FV-0005",
                Name = "小笼包",
                Region = "江南",
                Rarity = "日常",
                ImagePath = "seed_xiaolongbao.jpg",
                StarRating = 5,
                PrimaryTaste = "鲜",
                AromaTag = "无",
                TextureTag = "软糯",
                Description = "小笼包是中国传统的面点小吃，以江南地区最为著名。上海南翔小笼最为知名，始于清代同治十年（1871年）。皮薄馅大汁多，以精白面粉发酵作皮，以猪肉加皮冻调馅。一笼通常八个，小巧玲珑，汤汁鲜美。食用时先在底部咬一小口，吸出汤汁后再整个享用。",
                Ingredients = "[\"猪夹心肉\",\"皮冻\",\"中筋面粉\",\"葱姜汁\",\"料酒\",\"香油\"]",
                PriceRange = "~10",
                CollectStatus = "已收藏",
                Latitude = 31.2304,
                Longitude = 121.4737,
                LocationName = "上海南翔馒头店",
                DiscoverDate = "2025-01-18",
                NoteText = "正宗南翔小笼，轻轻一咬汤汁四溢，蘸上醋和姜丝堪称完美",
                CollectionName = "街头传奇",
                IsShowcase = false,
                CreatedAt = "2025-01-18 11:00:00",
                UpdatedAt = "2025-01-18 11:00:00"
            },
            new()
            {
                CatalogNumber = "FV-0006",
                Name = "东坡肉",
                Region = "江南",
                Rarity = "限定",
                ImagePath = "seed_dongpo_rou.jpg",
                StarRating = 5,
                PrimaryTaste = "甜",
                AromaTag = "酱香",
                TextureTag = "软糯",
                Description = "东坡肉是杭州名菜，属浙菜系。相传为北宋诗人苏东坡所创。用猪五花肉加酒、酱油等调料小火慢焖而成。肉质酥烂而不腻，色泽红亮如玛瑙。东坡肉讲究慢火、少水、多酒，成品酥烂而形不碎，香糯而不腻口，是杭州的传统名菜。",
                Ingredients = "[\"五花肉\",\"绍兴黄酒\",\"酱油\",\"冰糖\",\"葱姜\",\"八角\"]",
                PriceRange = "30~60",
                CollectStatus = "已收藏",
                Latitude = 30.2590,
                Longitude = 120.1551,
                LocationName = "杭州楼外楼",
                DiscoverDate = "2025-03-28",
                NoteText = "酥烂入味，肥而不腻，黄酒的香气沁入每一寸肉纤维",
                CollectionName = "奢享时刻",
                IsShowcase = true,
                CreatedAt = "2025-03-28 13:20:00",
                UpdatedAt = "2025-03-28 13:20:00"
            },
            new()
            {
                CatalogNumber = "FV-0007",
                Name = "炸酱面",
                Region = "北方",
                Rarity = "日常",
                ImagePath = "seed_lanzhou_lamian.jpg",
                StarRating = 4,
                PrimaryTaste = "咸",
                AromaTag = "酱香",
                TextureTag = "劲道",
                Description = "炸酱面是中国传统特色面食，被誉为\"中国十大面条\"之一。最初起源于北京，属北京菜系。炸酱面的灵魂在于炸酱，以黄酱和甜面酱为基础，加入肉丁慢慢煸炒，直到酱香浓郁。搭配黄瓜丝、豆芽、萝卜丝等菜码，拌匀后每根面条都裹满酱香。",
                Ingredients = "[\"手擀面条\",\"五花肉丁\",\"黄酱\",\"甜面酱\",\"黄瓜丝\",\"豆芽\",\"萝卜丝\"]",
                PriceRange = "~10",
                CollectStatus = "已收藏",
                Latitude = 39.9042,
                Longitude = 116.4074,
                LocationName = "北京老字号面馆",
                DiscoverDate = "2025-04-10",
                NoteText = "地道的北京味儿，酱香浓郁，面条筋道",
                CollectionName = "面面俱到",
                IsShowcase = false,
                CreatedAt = "2025-04-10 12:00:00",
                UpdatedAt = "2025-04-10 12:00:00"
            },
            new()
            {
                CatalogNumber = "FV-0008",
                Name = "羊肉泡馍",
                Region = "西北",
                Rarity = "推荐",
                ImagePath = "seed_yangrou_paomo.jpg",
                StarRating = 5,
                PrimaryTaste = "鲜",
                AromaTag = "草本",
                TextureTag = "浓郁",
                Description = "羊肉泡馍是陕西西安的传统美食，被誉为\"天下第一碗\"。将烤制的坨坨馍用手掰成碎块，加入熬制数小时的羊肉高汤中，配以粉丝、木耳等。汤浓肉烂，馍筋光滑，料重味醇。吃法讲究蚕食，不能搅动，从边上一点一点吃。",
                Ingredients = "[\"羊肉\",\"坨坨馍\",\"粉丝\",\"木耳\",\"黄花菜\",\"葱姜\",\"香料\"]",
                PriceRange = "10~30",
                CollectStatus = "已收藏",
                Latitude = 34.2658,
                Longitude = 108.9541,
                LocationName = "西安老孙家泡馍",
                DiscoverDate = "2025-05-01",
                NoteText = "掰馍的过程本身就是一种享受，汤鲜味浓，馍吸满了羊肉精华",
                CollectionName = null,
                IsShowcase = false,
                CreatedAt = "2025-05-01 07:30:00",
                UpdatedAt = "2025-05-01 07:30:00"
            },
            new()
            {
                CatalogNumber = "FV-0009",
                Name = "兰州拉面",
                Region = "西北",
                Rarity = "日常",
                ImagePath = "seed_yangrou_paomo.jpg",
                StarRating = 4,
                PrimaryTaste = "鲜",
                AromaTag = "无",
                TextureTag = "劲道",
                Description = "兰州牛肉拉面，又称兰州清汤牛肉面，是甘肃省兰州市的传统美食。以\"汤镜者清，肉烂者香，面细者精\"的独特风味和\"一清二白三红四绿五黄\"（汤清、萝卜白、辣油红、蒜苗绿、面条黄）著称。手工拉制的面条筋道爽滑，牛肉汤鲜美醇厚。",
                Ingredients = "[\"高筋面粉\",\"牛肉\",\"牛骨\",\"白萝卜\",\"辣椒油\",\"蒜苗\",\"香菜\"]",
                PriceRange = "~10",
                CollectStatus = "已收藏",
                Latitude = 36.0611,
                Longitude = 103.8343,
                LocationName = "兰州马子禄牛肉面",
                DiscoverDate = "2025-02-08",
                NoteText = "一清二白三红四绿五黄，拉面师傅的手艺令人叹服",
                CollectionName = "面面俱到",
                IsShowcase = false,
                CreatedAt = "2025-02-08 08:00:00",
                UpdatedAt = "2025-02-08 08:00:00"
            },
            new()
            {
                CatalogNumber = "FV-0010",
                Name = "天妇罗",
                Region = "日本",
                Rarity = "推荐",
                ImagePath = "seed_shuipenyangrou.jpg",
                StarRating = 4,
                PrimaryTaste = "鲜",
                AromaTag = "无",
                TextureTag = "酥脆",
                Description = "天妇罗是日本料理中最具代表性的油炸食品之一。源自葡萄牙，在16世纪经由传教士传入日本。将海鲜和蔬菜裹上薄薄的衣糊，放入高温油中快速炸制。成品外酥里嫩，保留食材原有鲜味。蘸上天汁或抹茶盐食用，口感层次丰富。",
                Ingredients = "[\"大虾\",\"南瓜\",\"红薯\",\"茄子\",\"低筋面粉\",\"鸡蛋\",\"冰水\"]",
                PriceRange = "30~60",
                CollectStatus = "已收藏",
                Latitude = 35.6762,
                Longitude = 139.6503,
                LocationName = "东京银座天妇罗老铺",
                DiscoverDate = "2025-04-15",
                NoteText = "外衣薄如蝉翼，食材的鲜味被完美锁住，蘸天汁食用绝妙",
                CollectionName = null,
                IsShowcase = true,
                CreatedAt = "2025-04-15 19:00:00",
                UpdatedAt = "2025-04-15 19:00:00"
            },
            new()
            {
                CatalogNumber = "FV-0011",
                Name = "鳗鱼饭",
                Region = "日本",
                Rarity = "限定",
                ImagePath = "seed_tianfuluo.jpg",
                StarRating = 5,
                PrimaryTaste = "复合",
                AromaTag = "烟熏",
                TextureTag = "软糯",
                Description = "鳗鱼饭（鳗重/鳗丼）是日本传统的高级料理。将蒲烧鳗鱼铺在热腾腾的白饭上，淋上特制的甜酱油汁。鳗鱼经过炭火慢烤，外层微焦，内里软嫩，酱汁浓郁香甜。在日本，每年土用丑日有吃鳗鱼进补的习俗。",
                Ingredients = "[\"日本鳗\",\"酱油\",\"味醂\",\"清酒\",\"砂糖\",\"山椒粉\",\"米饭\"]",
                PriceRange = "60~100",
                CollectStatus = "已收藏",
                Latitude = 35.0116,
                Longitude = 135.7681,
                LocationName = "京都先斗町鳗鱼料理",
                DiscoverDate = "2025-04-18",
                NoteText = "炭烤的香气和甜咸交织的酱汁完美融合，每一口都是享受",
                CollectionName = "奢享时刻",
                IsShowcase = false,
                CreatedAt = "2025-04-18 20:30:00",
                UpdatedAt = "2025-04-18 20:30:00"
            },
            new()
            {
                CatalogNumber = "FV-0012",
                Name = "炸鸡",
                Region = "韩国",
                Rarity = "日常",
                ImagePath = "seed_dongyingong.jpg",
                StarRating = 4,
                PrimaryTaste = "咸",
                AromaTag = "蒜香",
                TextureTag = "酥脆",
                Description = "韩国炸鸡（韩语：한국식 치킨）是韩国最受欢迎的国民美食之一。与美式炸鸡不同，韩式炸鸡通常经过两次炸制，外皮格外酥脆，再裹上各种口味的酱汁。经典口味包括原味、甜辣酱、蒜香酱油、蜂蜜黄油等。搭配啤酒被称为\"炸鸡啤酒\"（치맥），是韩国独特的饮食文化。",
                Ingredients = "[\"鸡肉\",\"蒜末\",\"酱油\",\"辣椒酱\",\"蜂蜜\",\"淀粉\",\"面粉\"]",
                PriceRange = "10~30",
                CollectStatus = "已尝试",
                Latitude = 37.5665,
                Longitude = 126.9780,
                LocationName = "首尔弘大炸鸡街",
                DiscoverDate = "2025-03-05",
                NoteText = "双重炸制让外皮超酥脆，甜辣酱口味是我的最爱，配啤酒一绝",
                CollectionName = "街头传奇",
                IsShowcase = false,
                CreatedAt = "2025-03-05 21:00:00",
                UpdatedAt = "2025-03-05 21:00:00"
            },
            new()
            {
                CatalogNumber = "FV-0013",
                Name = "冬阴功",
                Region = "东南亚",
                Rarity = "推荐",
                ImagePath = "seed_dongyingong.jpg",
                StarRating = 5,
                PrimaryTaste = "酸",
                AromaTag = "草本",
                TextureTag = "清爽",
                Description = "冬阴功汤（Tom Yum Goong）是泰国的国汤，\"冬阴\"意为酸辣虾汤。以鲜虾为主料，加入香茅、南姜、青柠叶、辣椒等香料熬制。汤色橙红透亮，酸辣鲜香，层次丰富。椰奶版本的冬阴功更加浓郁顺滑。是泰国最具代表性的美食之一。",
                Ingredients = "[\"大虾\",\"香茅\",\"南姜\",\"青柠叶\",\"辣椒\",\"青柠汁\",\"鱼露\",\"椰奶\"]",
                PriceRange = "30~60",
                CollectStatus = "已收藏",
                Latitude = 13.7563,
                Longitude = 100.5018,
                LocationName = "曼谷路边摊",
                DiscoverDate = "2025-04-22",
                NoteText = "酸辣鲜香的完美平衡，香茅和青柠叶的香气让人欲罢不能",
                CollectionName = "无辣不欢",
                IsShowcase = true,
                CreatedAt = "2025-04-22 13:00:00",
                UpdatedAt = "2025-04-22 13:00:00"
            },
            new()
            {
                CatalogNumber = "FV-0014",
                Name = "越南河粉",
                Region = "东南亚",
                Rarity = "日常",
                ImagePath = "seed_manyu_fan.jpg",
                StarRating = 4,
                PrimaryTaste = "鲜",
                AromaTag = "草本",
                TextureTag = "清爽",
                Description = "越南河粉（Phở）是越南的国菜，是一种以大米制成的河粉配上牛骨高汤的传统美食。汤底由牛骨与多种香料（八角、桂皮、丁香、草果等）长时间熬制而成，清澈鲜美。食用时加入新鲜九层塔、青柠、辣椒和豆芽，清爽而滋味丰富。",
                Ingredients = "[\"牛肉\",\"大米河粉\",\"牛骨\",\"八角\",\"桂皮\",\"九层塔\",\"青柠\",\"豆芽\"]",
                PriceRange = "10~30",
                CollectStatus = "已收藏",
                Latitude = 21.0285,
                Longitude = 105.8542,
                LocationName = "河内老城区河粉店",
                DiscoverDate = "2025-03-20",
                NoteText = "清澈鲜美的汤底，搭配新鲜香草，每一口都充满层次感",
                CollectionName = "面面俱到",
                IsShowcase = false,
                CreatedAt = "2025-03-20 08:30:00",
                UpdatedAt = "2025-03-20 08:30:00"
            },
            new()
            {
                CatalogNumber = "FV-0015",
                Name = "惠灵顿牛排",
                Region = "欧美",
                Rarity = "珍藏",
                ImagePath = "seed_huiguo_rou.jpg",
                StarRating = 5,
                PrimaryTaste = "复合",
                AromaTag = "奶香",
                TextureTag = "酥脆",
                Description = "惠灵顿牛排（Beef Wellington）是一道经典的英式高级料理。以整块菲力牛排为核心，包裹蘑菇酱（Duxelles）和帕尔马火腿，最外层覆盖一层金黄酥脆的酥皮。烤制后外酥内嫩，切开时层次分明，口感丰富。这道菜对厨师的技艺要求极高，是西餐中的经典之作。",
                Ingredients = "[\"菲力牛排\",\"酥皮\",\"蘑菇\",\"帕尔马火腿\",\"黄芥末\",\"鸡蛋\",\"黄油\"]",
                PriceRange = "100+",
                CollectStatus = "已收藏",
                Latitude = 51.5074,
                Longitude = -0.1278,
                LocationName = "伦敦The Grill",
                DiscoverDate = "2025-05-15",
                NoteText = "酥皮金黄酥脆，牛排嫩滑多汁，蘑菇酱丰富了层次感，值得珍藏的美食体验",
                CollectionName = "奢享时刻",
                IsShowcase = true,
                CreatedAt = "2025-05-15 20:00:00",
                UpdatedAt = "2025-05-15 20:00:00"
            },
            new()
            {
                CatalogNumber = "FV-0016",
                Name = "酸辣粉",
                Region = "川渝",
                Rarity = "日常",
                ImagePath = "seed_yangrou_paomo.jpg",
                StarRating = 4,
                PrimaryTaste = "辣",
                AromaTag = "蒜香",
                TextureTag = "劲道",
                Description = "酸辣粉是四川、重庆等地的传统名小吃。以红薯粉条为主料，加入醋、辣椒油、花椒、花生碎、香菜等调味。酸辣开胃，粉条劲道弹滑，是街头巷尾最受欢迎的小吃之一。正宗的酸辣粉讲究\"麻、辣、鲜、香、酸\"五味俱全。",
                Ingredients = "[\"红薯粉条\",\"辣椒油\",\"醋\",\"花椒粉\",\"花生碎\",\"香菜\",\"蒜水\",\"酱油\"]",
                PriceRange = "~10",
                CollectStatus = "已收藏",
                Latitude = 29.5630,
                Longitude = 106.5516,
                LocationName = "重庆解放碑酸辣粉摊",
                DiscoverDate = "2025-02-25",
                NoteText = "路边摊的酸辣粉最正宗，酸辣过瘾，粉条Q弹有嚼劲",
                CollectionName = "无辣不欢",
                IsShowcase = false,
                CreatedAt = "2025-02-25 15:30:00",
                UpdatedAt = "2025-02-25 15:30:00"
            }
        };

        foreach (var entry in entries)
        {
            await _foodEntryRepo.SaveAsync(entry);
        }
    }

    private async Task SeedWishItemsAsync()
    {
        var items = new List<WishItem>();

        foreach (var item in items)
        {
            await _wishItemRepo.SaveAsync(item);
        }
    }

    private async Task SeedCollectionsAsync()
    {
        var collections = new List<Collection>
        {
            new()
            {
                Name = "无辣不欢",
                Theme = "辣味挑战",
                Description = "挑战各种辣味美食",
                ColorTag = "红",
                SortOrder = 0,
                CreatedAt = "2025-01-01 10:00:00",
                UpdatedAt = "2025-01-01 10:00:00"
            },
            new()
            {
                Name = "面面俱到",
                Theme = "各地面食",
                Description = "收集全国各地的面食",
                ColorTag = "黄",
                SortOrder = 1,
                CreatedAt = "2025-01-01 10:05:00",
                UpdatedAt = "2025-01-01 10:05:00"
            },
            new()
            {
                Name = "街头传奇",
                Theme = "街边小吃",
                Description = "最接地气的味道",
                ColorTag = "绿",
                SortOrder = 2,
                CreatedAt = "2025-01-01 10:10:00",
                UpdatedAt = "2025-01-01 10:10:00"
            },
            new()
            {
                Name = "奢享时刻",
                Theme = "高端体验",
                Description = "特殊场合的珍藏美食",
                ColorTag = "紫",
                SortOrder = 3,
                CreatedAt = "2025-01-01 10:15:00",
                UpdatedAt = "2025-01-01 10:15:00"
            }
        };

        foreach (var collection in collections)
        {
            await _collectionRepo.SaveAsync(collection);
        }
    }

    private async Task SeedPlaceMarksAsync()
    {
        var marks = new List<PlaceMark>
        {
            new()
            {
                Name = "宽窄巷子",
                Category = "美食街",
                Address = "成都市青羊区",
                Latitude = 30.6697,
                Longitude = 104.0555,
                Region = "川渝",
                Feature = "川渝小吃汇聚",
                StarRating = 5,
                CreatedAt = "2025-01-01 10:00:00"
            },
            new()
            {
                Name = "上下九步行街",
                Category = "商圈",
                Address = "广州市荔湾区",
                Latitude = 23.1181,
                Longitude = 113.2442,
                Region = "粤港",
                Feature = "粤式茶点一条街",
                StarRating = 4,
                CreatedAt = "2025-01-01 10:05:00"
            },
            new()
            {
                Name = "城隍庙",
                Category = "景点周边",
                Address = "上海市黄浦区",
                Latitude = 31.2275,
                Longitude = 121.4920,
                Region = "江南",
                Feature = "南翔小笼发源地",
                StarRating = 5,
                CreatedAt = "2025-01-01 10:10:00"
            },
            new()
            {
                Name = "簋街",
                Category = "美食街",
                Address = "北京市东城区",
                Latitude = 39.9403,
                Longitude = 116.4274,
                Region = "北方",
                Feature = "夜宵圣地",
                StarRating = 4,
                CreatedAt = "2025-01-01 10:15:00"
            },
            new()
            {
                Name = "回民街",
                Category = "老字号",
                Address = "西安市碑林区",
                Latitude = 34.2583,
                Longitude = 108.9425,
                Region = "西北",
                Feature = "西北风味一条街",
                StarRating = 5,
                CreatedAt = "2025-01-01 10:20:00"
            },
            new()
            {
                Name = "筑地市场",
                Category = "景点周边",
                Address = "东京都中央区",
                Latitude = 35.6654,
                Longitude = 139.7707,
                Region = "日本",
                Feature = "海鲜天妇罗",
                StarRating = 5,
                CreatedAt = "2025-01-01 10:25:00"
            },
            new()
            {
                Name = "明洞小吃街",
                Category = "夜市",
                Address = "首尔中区",
                Latitude = 37.5636,
                Longitude = 126.9834,
                Region = "韩国",
                Feature = "韩式炸鸡烤肉",
                StarRating = 4,
                CreatedAt = "2025-01-01 10:30:00"
            },
            new()
            {
                Name = "考山路",
                Category = "夜市",
                Address = "曼谷邦腊普",
                Latitude = 13.7589,
                Longitude = 100.4974,
                Region = "东南亚",
                Feature = "泰式街头美食",
                StarRating = 4,
                CreatedAt = "2025-01-01 10:35:00"
            }
        };

        foreach (var mark in marks)
        {
            await _placeMarkRepo.SaveAsync(mark);
        }
    }

    private async Task SeedUserPreferencesAsync()
    {
        await _userProfileRepo.SetThemeAsync("Light");
        await _userProfileRepo.SetFontSizeAsync("Medium");
    }
}
