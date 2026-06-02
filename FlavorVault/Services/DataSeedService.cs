using FlavorVault.Models;

namespace FlavorVault.Services;

/// <summary>
/// 数据种子服务，首次启动时填充示例数据
/// </summary>
public class DataSeedService
{
    private readonly DatabaseService _dbService;
    private readonly FoodEntryRepository _foodEntryRepo;
    private readonly WishItemRepository _wishItemRepo;
    private readonly CollectionRepository _collectionRepo;
    private readonly PlaceMarkRepository _placeMarkRepo;
    private readonly UserProfileRepository _userProfileRepo;

    public DataSeedService(
        DatabaseService dbService,
        FoodEntryRepository foodEntryRepo,
        WishItemRepository wishItemRepo,
        CollectionRepository collectionRepo,
        PlaceMarkRepository placeMarkRepo,
        UserProfileRepository userProfileRepo)
    {
        _dbService = dbService;
        _foodEntryRepo = foodEntryRepo;
        _wishItemRepo = wishItemRepo;
        _collectionRepo = collectionRepo;
        _placeMarkRepo = placeMarkRepo;
        _userProfileRepo = userProfileRepo;
    }

    /// <summary>
    /// 检查并执行首次数据填充
    /// </summary>
    public async Task SeedIfNeededAsync()
    {
        await _dbService.Init();

        // 检查是否已有数据
        var count = await _foodEntryRepo.GetCountAsync();
        if (count > 0) return;

        System.Diagnostics.Debug.WriteLine("[DataSeedService] 开始填充示例数据...");

        await SeedCollectionsAsync();
        await SeedFoodEntriesAsync();
        await SeedWishItemsAsync();
        await SeedPlaceMarksAsync();
        await SeedUserProfileAsync();

        System.Diagnostics.Debug.WriteLine("[DataSeedService] 示例数据填充完成！");
    }

    /// <summary>
    /// 填充收藏集
    /// </summary>
    private async Task SeedCollectionsAsync()
    {
        var collections = new[]
        {
            new Collection { Name = "街头小吃", Description = "各地经典街头美食", ColorTag = "Red", Theme = "Spicy" },
            new Collection { Name = "甜品合集", Description = "甜蜜的味觉记忆", ColorTag = "Pink", Theme = "Sweet" },
            new Collection { Name = "家常菜", Description = "温暖的家常味道", ColorTag = "Green", Theme = "Home" },
            new Collection { Name = "异国风味", Description = "探索世界美食", ColorTag = "Blue", Theme = "Travel" },
        };

        foreach (var c in collections)
            await _collectionRepo.SaveAsync(c);
    }

    /// <summary>
    /// 填充食物图鉴条目（40+ 条）
    /// </summary>
    private async Task SeedFoodEntriesAsync()
    {
        var now = DateTime.Now;
        var entries = new[]
        {
            // ===== 川渝 =====
            new FoodEntry { CatalogNumber = "FV-0001", Name = "麻婆豆腐", Region = "川渝", Rarity = "日常",
                ImagePath = "seed_suanlafen.jpg",
                StarRating = 4, PrimaryTaste = "辣", AromaTag = "酱香", TextureTag = "软糯",
                Ingredients = "豆腐,牛肉末,豆瓣酱,花椒粉,辣椒面", PriceRange = "10~30",
                Description = "经典川菜，麻辣鲜香，豆腐嫩滑入味", CollectStatus = "已收藏",
                IsShowcase = true, DiscoverDate = now.AddDays(-30).ToString("yyyy-MM-dd"),
                LocationName = "成都·宽窄巷子", CreatedAt = now.AddDays(-30).ToString("yyyy-MM-dd HH:mm:ss") },

            new FoodEntry { CatalogNumber = "FV-0002", Name = "重庆小面", Region = "川渝", Rarity = "日常",
                ImagePath = "seed_yangrou_paomo.jpg",
                StarRating = 5, PrimaryTaste = "辣", AromaTag = "蒜香", TextureTag = "劲道",
                Ingredients = "碱水面,辣椒红油,花椒,花生碎,榨菜", PriceRange = "10~30",
                Description = "重庆人的早餐之魂，一碗红油小面唤醒一天", CollectStatus = "已收藏",
                IsShowcase = true, DiscoverDate = now.AddDays(-28).ToString("yyyy-MM-dd"),
                LocationName = "重庆·解放碑", CreatedAt = now.AddDays(-28).ToString("yyyy-MM-dd HH:mm:ss") },

            new FoodEntry { CatalogNumber = "FV-0003", Name = "水煮鱼", Region = "川渝", Rarity = "推荐",
                ImagePath = "seed_huilington_niupai.jpg",
                StarRating = 5, PrimaryTaste = "辣", AromaTag = "酱香", TextureTag = "丝滑",
                Ingredients = "草鱼,豆芽,干辣椒,花椒,豆瓣酱", PriceRange = "60~100",
                Description = "鲜嫩的鱼片浸在红油中，麻辣鲜香", CollectStatus = "已收藏",
                IsShowcase = false, DiscoverDate = now.AddDays(-25).ToString("yyyy-MM-dd"),
                LocationName = "成都·春熙路", CreatedAt = now.AddDays(-25).ToString("yyyy-MM-dd HH:mm:ss") },

            new FoodEntry { CatalogNumber = "FV-0004", Name = "钵钵鸡", Region = "川渝", Rarity = "日常",
                ImagePath = "seed_xiajiao.jpg",
                StarRating = 4, PrimaryTaste = "辣", AromaTag = "酱香", TextureTag = "弹牙",
                Ingredients = "鸡肉,藕片,土豆,竹签,红油", PriceRange = "10~30",
                Description = "冷串串，红油浸泡，鲜香入味", CollectStatus = "已收藏",
                IsShowcase = false, DiscoverDate = now.AddDays(-20).ToString("yyyy-MM-dd"),
                LocationName = "成都·锦里", CreatedAt = now.AddDays(-20).ToString("yyyy-MM-dd HH:mm:ss") },

            new FoodEntry { CatalogNumber = "FV-0005", Name = "夫妻肺片", Region = "川渝", Rarity = "推荐",
                ImagePath = "seed_dongyingong.jpg",
                StarRating = 4, PrimaryTaste = "辣", AromaTag = "酱香", TextureTag = "劲道",
                Ingredients = "牛肉,牛杂,花椒,辣椒红油,芝麻", PriceRange = "30~60",
                Description = "名扬四海的凉拌菜，红油浸润，麻辣鲜香", CollectStatus = "已收藏",
                IsShowcase = false, DiscoverDate = now.AddDays(-18).ToString("yyyy-MM-dd"),
                LocationName = "成都·总府路", CreatedAt = now.AddDays(-18).ToString("yyyy-MM-dd HH:mm:ss") },

            // ===== 粤港 =====
            new FoodEntry { CatalogNumber = "FV-0006", Name = "虾饺", Region = "粤港", Rarity = "日常",
                ImagePath = "seed_xiajiao.jpg",
                StarRating = 5, PrimaryTaste = "鲜", AromaTag = "无", TextureTag = "软糯",
                Ingredients = "鲜虾,澄粉,猪油,竹笋", PriceRange = "30~60",
                Description = "晶莹剔透的虾饺皇，一口一个鲜", CollectStatus = "已收藏",
                IsShowcase = true, DiscoverDate = now.AddDays(-26).ToString("yyyy-MM-dd"),
                LocationName = "广州·上下九", CreatedAt = now.AddDays(-26).ToString("yyyy-MM-dd HH:mm:ss") },

            new FoodEntry { CatalogNumber = "FV-0007", Name = "叉烧", Region = "粤港", Rarity = "推荐",
                ImagePath = "seed_huiguo_rou.jpg",
                StarRating = 5, PrimaryTaste = "甜", AromaTag = "酱香", TextureTag = "软糯",
                Ingredients = "猪五花,蜜汁,叉烧酱,玫瑰露酒", PriceRange = "30~60",
                Description = "蜜汁叉烧，焦香四溢，甜咸适中", CollectStatus = "已收藏",
                IsShowcase = false, DiscoverDate = now.AddDays(-24).ToString("yyyy-MM-dd"),
                LocationName = "香港·深水埗", CreatedAt = now.AddDays(-24).ToString("yyyy-MM-dd HH:mm:ss") },

            new FoodEntry { CatalogNumber = "FV-0008", Name = "肠粉", Region = "粤港", Rarity = "日常",
                ImagePath = "seed_xiajiao.jpg",
                StarRating = 4, PrimaryTaste = "鲜", AromaTag = "酱香", TextureTag = "丝滑",
                Ingredients = "米浆,鸡蛋,虾仁,酱油,葱花", PriceRange = "~10",
                Description = "滑嫩的肠粉配上特制酱油，早餐首选", CollectStatus = "已收藏",
                IsShowcase = false, DiscoverDate = now.AddDays(-22).ToString("yyyy-MM-dd"),
                LocationName = "广州·西关", CreatedAt = now.AddDays(-22).ToString("yyyy-MM-dd HH:mm:ss") },

            new FoodEntry { CatalogNumber = "FV-0009", Name = "菠萝包", Region = "粤港", Rarity = "日常",
                ImagePath = "seed_fotiaoqiang.jpg",
                StarRating = 4, PrimaryTaste = "甜", AromaTag = "奶香", TextureTag = "酥脆",
                Ingredients = "面粉,黄油,砂糖,鸡蛋,菠萝皮", PriceRange = "~10",
                Description = "酥脆的菠萝皮配上松软的面包，夹上一片黄油", CollectStatus = "想尝试",
                IsShowcase = false, DiscoverDate = now.AddDays(-15).ToString("yyyy-MM-dd"),
                LocationName = "香港·旺角", CreatedAt = now.AddDays(-15).ToString("yyyy-MM-dd HH:mm:ss") },

            // ===== 江南 =====
            new FoodEntry { CatalogNumber = "FV-0010", Name = "小笼包", Region = "江南", Rarity = "日常",
                ImagePath = "seed_xiaolongbao.jpg",
                StarRating = 5, PrimaryTaste = "鲜", AromaTag = "无", TextureTag = "软糯",
                Ingredients = "猪肉馅,面粉,姜汁,蟹黄,鸡汤冻", PriceRange = "30~60",
                Description = "皮薄汁多的南翔小笼，蘸上醋一口爆汁", CollectStatus = "已收藏",
                IsShowcase = true, DiscoverDate = now.AddDays(-27).ToString("yyyy-MM-dd"),
                LocationName = "上海·城隍庙", CreatedAt = now.AddDays(-27).ToString("yyyy-MM-dd HH:mm:ss") },

            new FoodEntry { CatalogNumber = "FV-0011", Name = "东坡肉", Region = "江南", Rarity = "推荐",
                ImagePath = "seed_huiguo_rou.jpg",
                StarRating = 5, PrimaryTaste = "甜", AromaTag = "酱香", TextureTag = "软糯",
                Ingredients = "五花肉,绍兴酒,酱油,冰糖,葱姜", PriceRange = "60~100",
                Description = "酥烂入味，肥而不腻，入口即化", CollectStatus = "已收藏",
                IsShowcase = false, DiscoverDate = now.AddDays(-23).ToString("yyyy-MM-dd"),
                LocationName = "杭州·楼外楼", CreatedAt = now.AddDays(-23).ToString("yyyy-MM-dd HH:mm:ss") },

            new FoodEntry { CatalogNumber = "FV-0012", Name = "桂花糕", Region = "江南", Rarity = "日常",
                ImagePath = "seed_kxaifuchenbing.jpg",
                StarRating = 3, PrimaryTaste = "甜", AromaTag = "花香", TextureTag = "软糯",
                Ingredients = "糯米粉,桂花,白糖,蜂蜜", PriceRange = "~10",
                Description = "清甜的桂花香气，软糯的口感", CollectStatus = "已收藏",
                IsShowcase = false, DiscoverDate = now.AddDays(-19).ToString("yyyy-MM-dd"),
                LocationName = "苏州·观前街", CreatedAt = now.AddDays(-19).ToString("yyyy-MM-dd HH:mm:ss") },

            new FoodEntry { CatalogNumber = "FV-0013", Name = "生煎包", Region = "江南", Rarity = "日常",
                ImagePath = "seed_xiajiao.jpg",
                StarRating = 4, PrimaryTaste = "鲜", AromaTag = "酱香", TextureTag = "酥脆",
                Ingredients = "猪肉馅,面粉,芝麻,葱花,皮冻", PriceRange = "10~30",
                Description = "底部金黄酥脆，一口咬下汤汁四溅", CollectStatus = "已收藏",
                IsShowcase = false, DiscoverDate = now.AddDays(-17).ToString("yyyy-MM-dd"),
                LocationName = "上海·小杨生煎", CreatedAt = now.AddDays(-17).ToString("yyyy-MM-dd HH:mm:ss") },

            // ===== 北方 =====
            new FoodEntry { CatalogNumber = "FV-0014", Name = "北京烤鸭", Region = "北方", Rarity = "推荐",
                ImagePath = "seed_shao_e.jpg",
                StarRating = 5, PrimaryTaste = "咸", AromaTag = "果香", TextureTag = "酥脆",
                Ingredients = "填鸭,甜面酱,大葱,黄瓜,薄饼", PriceRange = "100+",
                Description = "枣红色的鸭皮酥脆，蘸上甜面酱卷饼", CollectStatus = "已收藏",
                IsShowcase = true, DiscoverDate = now.AddDays(-29).ToString("yyyy-MM-dd"),
                LocationName = "北京·全聚德", CreatedAt = now.AddDays(-29).ToString("yyyy-MM-dd HH:mm:ss") },

            new FoodEntry { CatalogNumber = "FV-0015", Name = "炸酱面", Region = "北方", Rarity = "日常",
                ImagePath = "seed_yangrou_paomo.jpg",
                StarRating = 4, PrimaryTaste = "咸", AromaTag = "酱香", TextureTag = "劲道",
                Ingredients = "手擀面,黄酱,肉丁,黄瓜丝,豆芽", PriceRange = "10~30",
                Description = "老北京的味道，浓香的炸酱配上爽滑的面条", CollectStatus = "已收藏",
                IsShowcase = false, DiscoverDate = now.AddDays(-21).ToString("yyyy-MM-dd"),
                LocationName = "北京·鼓楼东大街", CreatedAt = now.AddDays(-21).ToString("yyyy-MM-dd HH:mm:ss") },

            new FoodEntry { CatalogNumber = "FV-0016", Name = "驴肉火烧", Region = "北方", Rarity = "推荐",
                ImagePath = "seed_budabu.jpg",
                StarRating = 4, PrimaryTaste = "鲜", AromaTag = "无", TextureTag = "酥脆",
                Ingredients = "驴肉,火烧皮,青椒,香菜", PriceRange = "10~30",
                Description = "天上龙肉地下驴肉，外酥里嫩", CollectStatus = "想尝试",
                IsShowcase = false, DiscoverDate = now.AddDays(-14).ToString("yyyy-MM-dd"),
                LocationName = "保定·古莲花池", CreatedAt = now.AddDays(-14).ToString("yyyy-MM-dd HH:mm:ss") },

            new FoodEntry { CatalogNumber = "FV-0017", Name = "锅包肉", Region = "北方", Rarity = "日常",
                ImagePath = "seed_huiguo_rou.jpg",
                StarRating = 4, PrimaryTaste = "酸", AromaTag = "果香", TextureTag = "酥脆",
                Ingredients = "里脊肉,淀粉,白糖,白醋,番茄酱", PriceRange = "30~60",
                Description = "东北经典，外酥里嫩，酸甜可口", CollectStatus = "已收藏",
                IsShowcase = false, DiscoverDate = now.AddDays(-12).ToString("yyyy-MM-dd"),
                LocationName = "哈尔滨·中央大街", CreatedAt = now.AddDays(-12).ToString("yyyy-MM-dd HH:mm:ss") },

            // ===== 西北 =====
            new FoodEntry { CatalogNumber = "FV-0018", Name = "兰州牛肉面", Region = "西北", Rarity = "日常",
                ImagePath = "seed_yangrou_paomo.jpg",
                StarRating = 5, PrimaryTaste = "鲜", AromaTag = "草本", TextureTag = "劲道",
                Ingredients = "高筋面粉,牛肉,萝卜,辣椒油,香菜", PriceRange = "~10",
                Description = "一清二白三红四绿五黄，地道兰州味", CollectStatus = "已收藏",
                IsShowcase = true, DiscoverDate = now.AddDays(-25).ToString("yyyy-MM-dd"),
                LocationName = "兰州·正宁路", CreatedAt = now.AddDays(-25).ToString("yyyy-MM-dd HH:mm:ss") },

            new FoodEntry { CatalogNumber = "FV-0019", Name = "羊肉泡馍", Region = "西北", Rarity = "推荐",
                ImagePath = "seed_huilington_niupai.jpg",
                StarRating = 4, PrimaryTaste = "鲜", AromaTag = "草本", TextureTag = "浓郁",
                Ingredients = "羊肉,饦饦馍,粉丝,木耳,葱姜", PriceRange = "30~60",
                Description = "浓汤浸透馍块，暖胃又暖心", CollectStatus = "已收藏",
                IsShowcase = false, DiscoverDate = now.AddDays(-16).ToString("yyyy-MM-dd"),
                LocationName = "西安·回民街", CreatedAt = now.AddDays(-16).ToString("yyyy-MM-dd HH:mm:ss") },

            new FoodEntry { CatalogNumber = "FV-0020", Name = "肉夹馍", Region = "西北", Rarity = "日常",
                ImagePath = "seed_budabu.jpg",
                StarRating = 4, PrimaryTaste = "咸", AromaTag = "酱香", TextureTag = "酥脆",
                Ingredients = "白吉馍,腊汁肉,青椒,香菜", PriceRange = "10~30",
                Description = "外酥里嫩的馍夹上卤好的腊汁肉", CollectStatus = "已收藏",
                IsShowcase = false, DiscoverDate = now.AddDays(-13).ToString("yyyy-MM-dd"),
                LocationName = "西安·永兴坊", CreatedAt = now.AddDays(-13).ToString("yyyy-MM-dd HH:mm:ss") },

            // ===== 日本 =====
            new FoodEntry { CatalogNumber = "FV-0021", Name = "拉面", Region = "日本", Rarity = "日常",
                ImagePath = "seed_lanzhou_lamian.jpg",
                StarRating = 5, PrimaryTaste = "鲜", AromaTag = "蒜香", TextureTag = "劲道",
                Ingredients = "面条,猪骨汤,叉烧,溏心蛋,海苔", PriceRange = "30~60",
                Description = "浓郁的猪骨汤底配上弹牙的面条", CollectStatus = "已收藏",
                IsShowcase = true, DiscoverDate = now.AddDays(-22).ToString("yyyy-MM-dd"),
                LocationName = "东京·新宿", CreatedAt = now.AddDays(-22).ToString("yyyy-MM-dd HH:mm:ss") },

            new FoodEntry { CatalogNumber = "FV-0022", Name = "寿司", Region = "日本", Rarity = "推荐",
                ImagePath = "seed_kaishiliaoli.jpg",
                StarRating = 5, PrimaryTaste = "鲜", AromaTag = "无", TextureTag = "软糯",
                Ingredients = "寿司米,三文鱼,金枪鱼,海苔,芥末", PriceRange = "100+",
                Description = "新鲜的刺身配醋饭，简约而极致", CollectStatus = "已收藏",
                IsShowcase = false, DiscoverDate = now.AddDays(-10).ToString("yyyy-MM-dd"),
                LocationName = "东京·筑地市场", CreatedAt = now.AddDays(-10).ToString("yyyy-MM-dd HH:mm:ss") },

            new FoodEntry { CatalogNumber = "FV-0023", Name = "天妇罗", Region = "日本", Rarity = "日常",
                ImagePath = "seed_yangrou_paomo.jpg",
                StarRating = 4, PrimaryTaste = "鲜", AromaTag = "无", TextureTag = "酥脆",
                Ingredients = "虾,蔬菜,面粉,鸡蛋,炸油", PriceRange = "60~100",
                Description = "轻薄酥脆的面衣包裹鲜嫩的食材", CollectStatus = "已收藏",
                IsShowcase = false, DiscoverDate = now.AddDays(-8).ToString("yyyy-MM-dd"),
                LocationName = "京都·祇园", CreatedAt = now.AddDays(-8).ToString("yyyy-MM-dd HH:mm:ss") },

            new FoodEntry { CatalogNumber = "FV-0024", Name = "抹茶甜品", Region = "日本", Rarity = "推荐",
                ImagePath = "seed_kxaifuchenbing.jpg",
                StarRating = 4, PrimaryTaste = "甜", AromaTag = "草本", TextureTag = "丝滑",
                Ingredients = "抹茶粉,牛奶,红豆,糯米团,白玉粉", PriceRange = "30~60",
                Description = "浓郁的宇治抹茶配上细腻的甜品", CollectStatus = "想尝试",
                IsShowcase = false, DiscoverDate = now.AddDays(-5).ToString("yyyy-MM-dd"),
                LocationName = "京都·宇治", CreatedAt = now.AddDays(-5).ToString("yyyy-MM-dd HH:mm:ss") },

            // ===== 韩国 =====
            new FoodEntry { CatalogNumber = "FV-0025", Name = "韩式烤肉", Region = "韩国", Rarity = "日常",
                ImagePath = "seed_hanguo_zhaji.jpg",
                StarRating = 4, PrimaryTaste = "咸", AromaTag = "烟熏", TextureTag = "弹牙",
                Ingredients = "五花肉,生菜,蒜瓣,辣椒酱,大酱", PriceRange = "60~100",
                Description = "炭火烤制的五花肉配上新鲜生菜包着吃", CollectStatus = "已收藏",
                IsShowcase = false, DiscoverDate = now.AddDays(-19).ToString("yyyy-MM-dd"),
                LocationName = "首尔·明洞", CreatedAt = now.AddDays(-19).ToString("yyyy-MM-dd HH:mm:ss") },

            new FoodEntry { CatalogNumber = "FV-0026", Name = "石锅拌饭", Region = "韩国", Rarity = "日常",
                ImagePath = "seed_huilington_niupai.jpg",
                StarRating = 4, PrimaryTaste = "复合", AromaTag = "酱香", TextureTag = "酥脆",
                Ingredients = "米饭,辣酱,鸡蛋,各种蔬菜,香油", PriceRange = "30~60",
                Description = "滋滋作响的石锅，锅巴酥脆配上辣酱", CollectStatus = "已收藏",
                IsShowcase = false, DiscoverDate = now.AddDays(-11).ToString("yyyy-MM-dd"),
                LocationName = "首尔·弘大", CreatedAt = now.AddDays(-11).ToString("yyyy-MM-dd HH:mm:ss") },

            new FoodEntry { CatalogNumber = "FV-0027", Name = "韩式炸鸡", Region = "韩国", Rarity = "日常",
                ImagePath = "seed_jiaohuaji.jpg",
                StarRating = 5, PrimaryTaste = "咸", AromaTag = "蒜香", TextureTag = "酥脆",
                Ingredients = "鸡肉,面粉,韩式辣酱,蜂蜜,蒜泥", PriceRange = "30~60",
                Description = "酥脆的外皮裹上甜辣酱，配啤酒绝了", CollectStatus = "已收藏",
                IsShowcase = false, DiscoverDate = now.AddDays(-7).ToString("yyyy-MM-dd"),
                LocationName = "首尔·江南", CreatedAt = now.AddDays(-7).ToString("yyyy-MM-dd HH:mm:ss") },

            // ===== 东南亚 =====
            new FoodEntry { CatalogNumber = "FV-0028", Name = "冬阴功汤", Region = "东南亚", Rarity = "推荐",
                ImagePath = "seed_dongyingong.jpg",
                StarRating = 4, PrimaryTaste = "酸", AromaTag = "草本", TextureTag = "清爽",
                Ingredients = "虾,香茅,南姜,柠檬叶,辣椒", PriceRange = "30~60",
                Description = "酸辣鲜香的泰式经典，一碗就能感受到热带风情", CollectStatus = "已收藏",
                IsShowcase = false, DiscoverDate = now.AddDays(-15).ToString("yyyy-MM-dd"),
                LocationName = "曼谷·考山路", CreatedAt = now.AddDays(-15).ToString("yyyy-MM-dd HH:mm:ss") },

            new FoodEntry { CatalogNumber = "FV-0029", Name = "越南河粉", Region = "东南亚", Rarity = "日常",
                ImagePath = "seed_yangrou_paomo.jpg",
                StarRating = 4, PrimaryTaste = "鲜", AromaTag = "草本", TextureTag = "丝滑",
                Ingredients = "河粉,牛肉,豆芽,九层塔,鱼露", PriceRange = "10~30",
                Description = "清爽鲜美的牛肉河粉，配上一碟新鲜香草", CollectStatus = "已收藏",
                IsShowcase = false, DiscoverDate = now.AddDays(-9).ToString("yyyy-MM-dd"),
                LocationName = "胡志明市·滨城市场", CreatedAt = now.AddDays(-9).ToString("yyyy-MM-dd HH:mm:ss") },

            new FoodEntry { CatalogNumber = "FV-0030", Name = "新加坡辣椒蟹", Region = "东南亚", Rarity = "限定",
                ImagePath = "seed_shao_e.jpg",
                StarRating = 5, PrimaryTaste = "复合", AromaTag = "果香", TextureTag = "浓郁",
                Ingredients = "螃蟹,番茄酱,辣椒酱,鸡蛋,葱姜蒜", PriceRange = "100+",
                Description = "浓稠的辣椒酱裹着鲜甜的蟹肉", CollectStatus = "想尝试",
                IsShowcase = false, DiscoverDate = now.AddDays(-4).ToString("yyyy-MM-dd"),
                LocationName = "新加坡·纽顿熟食中心", CreatedAt = now.AddDays(-4).ToString("yyyy-MM-dd HH:mm:ss") },

            // ===== 欧美 =====
            new FoodEntry { CatalogNumber = "FV-0031", Name = "意大利披萨", Region = "欧美", Rarity = "日常",
                ImagePath = "seed_budabu.jpg",
                StarRating = 4, PrimaryTaste = "咸", AromaTag = "果香", TextureTag = "酥脆",
                Ingredients = "面团,番茄酱,马苏里拉芝士,罗勒,橄榄油", PriceRange = "30~60",
                Description = "窑炉烤制的薄底披萨，边缘焦脆中间柔软", CollectStatus = "已收藏",
                IsShowcase = false, DiscoverDate = now.AddDays(-18).ToString("yyyy-MM-dd"),
                LocationName = "罗马·特拉斯提弗列", CreatedAt = now.AddDays(-18).ToString("yyyy-MM-dd HH:mm:ss") },

            new FoodEntry { CatalogNumber = "FV-0032", Name = "法式牛角面包", Region = "欧美", Rarity = "日常",
                ImagePath = "seed_fotiaoqiang.jpg",
                StarRating = 5, PrimaryTaste = "咸", AromaTag = "奶香", TextureTag = "酥脆",
                Ingredients = "面粉,黄油,酵母,牛奶,盐", PriceRange = "10~30",
                Description = "层层酥皮，黄油香气扑鼻", CollectStatus = "已收藏",
                IsShowcase = false, DiscoverDate = now.AddDays(-6).ToString("yyyy-MM-dd"),
                LocationName = "巴黎·蒙马特", CreatedAt = now.AddDays(-6).ToString("yyyy-MM-dd HH:mm:ss") },

            new FoodEntry { CatalogNumber = "FV-0033", Name = "汉堡", Region = "欧美", Rarity = "日常",
                ImagePath = "seed_yuenan_hefen.jpg",
                StarRating = 4, PrimaryTaste = "咸", AromaTag = "烟熏", TextureTag = "软糯",
                Ingredients = "牛肉饼,芝士,生菜,番茄,面包", PriceRange = "30~60",
                Description = "多汁的牛肉饼配上融化的芝士", CollectStatus = "已收藏",
                IsShowcase = false, DiscoverDate = now.AddDays(-3).ToString("yyyy-MM-dd"),
                LocationName = "纽约·布鲁克林", CreatedAt = now.AddDays(-3).ToString("yyyy-MM-dd HH:mm:ss") },

            new FoodEntry { CatalogNumber = "FV-0034", Name = "提拉米苏", Region = "欧美", Rarity = "推荐",
                ImagePath = "seed_kxaifuchenbing.jpg",
                StarRating = 5, PrimaryTaste = "甜", AromaTag = "奶香", TextureTag = "丝滑",
                Ingredients = "马斯卡彭芝士,手指饼干,浓缩咖啡,可可粉,鸡蛋", PriceRange = "30~60",
                Description = "层层叠叠的甜蜜，咖啡与芝士的完美结合", CollectStatus = "已收藏",
                IsShowcase = false, DiscoverDate = now.AddDays(-2).ToString("yyyy-MM-dd"),
                LocationName = "威尼斯·圣马可广场", CreatedAt = now.AddDays(-2).ToString("yyyy-MM-dd HH:mm:ss") },

            // 更多精选条目（补充各地区）
            new FoodEntry { CatalogNumber = "FV-0035", Name = "毛血旺", Region = "川渝", Rarity = "推荐",
                StarRating = 4, PrimaryTaste = "辣", AromaTag = "蒜香", TextureTag = "弹牙",
                Ingredients = "鸭血,毛肚,午餐肉,豆芽,火锅底料", PriceRange = "60~100",
                Description = "红汤翻滚，食材丰富，麻辣鲜香", CollectStatus = "已收藏",
                DiscoverDate = now.AddDays(-11).ToString("yyyy-MM-dd"),
                LocationName = "重庆·磁器口", CreatedAt = now.AddDays(-11).ToString("yyyy-MM-dd HH:mm:ss") },

            new FoodEntry { CatalogNumber = "FV-0036", Name = "糖醋排骨", Region = "江南", Rarity = "日常",
                StarRating = 4, PrimaryTaste = "酸", AromaTag = "果香", TextureTag = "酥脆",
                Ingredients = "猪小排,白糖,醋,番茄酱,葱姜", PriceRange = "30~60",
                Description = "色泽红亮，酸甜适中，外酥里嫩", CollectStatus = "已收藏",
                DiscoverDate = now.AddDays(-8).ToString("yyyy-MM-dd"),
                LocationName = "无锡·南长街", CreatedAt = now.AddDays(-8).ToString("yyyy-MM-dd HH:mm:ss") },

            new FoodEntry { CatalogNumber = "FV-0037", Name = "广式煲仔饭", Region = "粤港", Rarity = "推荐",
                StarRating = 5, PrimaryTaste = "咸", AromaTag = "酱香", TextureTag = "酥脆",
                Ingredients = "丝苗米,腊味,酱油,鸡蛋,青菜", PriceRange = "30~60",
                Description = "锅巴焦香，腊味浓郁，一煲满足", CollectStatus = "已收藏",
                DiscoverDate = now.AddDays(-6).ToString("yyyy-MM-dd"),
                LocationName = "广州·北京路", CreatedAt = now.AddDays(-6).ToString("yyyy-MM-dd HH:mm:ss") },

            new FoodEntry { CatalogNumber = "FV-0038", Name = "韩式部队锅", Region = "韩国", Rarity = "日常",
                StarRating = 3, PrimaryTaste = "辣", AromaTag = "烟熏", TextureTag = "浓郁",
                Ingredients = "泡菜,午餐肉,拉面,芝士,年糕", PriceRange = "60~100",
                Description = "热气腾腾的部队锅，满满的料", CollectStatus = "已收藏",
                DiscoverDate = now.AddDays(-4).ToString("yyyy-MM-dd"),
                LocationName = "首尔·明洞", CreatedAt = now.AddDays(-4).ToString("yyyy-MM-dd HH:mm:ss") },

            new FoodEntry { CatalogNumber = "FV-0039", Name = "日式咖喱饭", Region = "日本", Rarity = "日常",
                StarRating = 4, PrimaryTaste = "咸", AromaTag = "果香", TextureTag = "浓郁",
                Ingredients = "咖喱块,土豆,胡萝卜,洋葱,猪肉", PriceRange = "30~60",
                Description = "浓稠的日式咖喱配上白饭，温暖治愈", CollectStatus = "已收藏",
                DiscoverDate = now.AddDays(-3).ToString("yyyy-MM-dd"),
                LocationName = "大阪·道顿堀", CreatedAt = now.AddDays(-3).ToString("yyyy-MM-dd HH:mm:ss") },

            new FoodEntry { CatalogNumber = "FV-0040", Name = "羊肉串", Region = "西北", Rarity = "日常",
                StarRating = 4, PrimaryTaste = "咸", AromaTag = "烟熏", TextureTag = "弹牙",
                Ingredients = "羊肉,孜然,辣椒面,盐,洋葱", PriceRange = "~10",
                Description = "炭火烤制的羊肉串，撒上孜然辣椒面", CollectStatus = "已收藏",
                DiscoverDate = now.AddDays(-1).ToString("yyyy-MM-dd"),
                LocationName = "乌鲁木齐·大巴扎", CreatedAt = now.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss") },

            new FoodEntry { CatalogNumber = "FV-0041", Name = "螺蛳粉", Region = "粤港", Rarity = "限定",
                StarRating = 4, PrimaryTaste = "辣", AromaTag = "草本", TextureTag = "劲道",
                Ingredients = "米粉,酸笋,腐竹,花生,螺蛳汤", PriceRange = "10~30",
                Description = "闻着臭吃着香，越吃越上瘾", CollectStatus = "已收藏",
                IsShowcase = false, DiscoverDate = now.ToString("yyyy-MM-dd"),
                LocationName = "柳州·五星街", CreatedAt = now.ToString("yyyy-MM-dd HH:mm:ss") },

            new FoodEntry { CatalogNumber = "FV-0042", Name = "马卡龙", Region = "欧美", Rarity = "限定",
                ImagePath = "seed_kxaifuchenbing.jpg",
                StarRating = 4, PrimaryTaste = "甜", AromaTag = "奶香", TextureTag = "酥脆",
                Ingredients = "杏仁粉,糖粉,蛋白,色素,奶油", PriceRange = "30~60",
                Description = "色彩缤纷的法式小圆饼，外壳酥脆内里柔软", CollectStatus = "想尝试",
                DiscoverDate = now.ToString("yyyy-MM-dd"),
                LocationName = "巴黎·香榭丽舍", CreatedAt = now.ToString("yyyy-MM-dd HH:mm:ss") },

            new FoodEntry { CatalogNumber = "FV-0043", Name = "煎饼果子", Region = "北方", Rarity = "日常",
                StarRating = 4, PrimaryTaste = "咸", AromaTag = "酱香", TextureTag = "酥脆",
                Ingredients = "绿豆面,鸡蛋,薄脆,葱花,甜面酱", PriceRange = "~10",
                Description = "天津人的早餐标配，一套煎饼果子开启一天", CollectStatus = "已收藏",
                DiscoverDate = now.AddDays(-7).ToString("yyyy-MM-dd"),
                LocationName = "天津·古文化街", CreatedAt = now.AddDays(-7).ToString("yyyy-MM-dd HH:mm:ss") },

            new FoodEntry { CatalogNumber = "FV-0044", Name = "泰式芒果糯米饭", Region = "东南亚", Rarity = "日常",
                StarRating = 4, PrimaryTaste = "甜", AromaTag = "奶香", TextureTag = "软糯",
                Ingredients = "糯米,芒果,椰浆,芝麻,糖", PriceRange = "10~30",
                Description = "甜蜜的芒果配上椰香糯米饭，热带的甜蜜", CollectStatus = "已收藏",
                DiscoverDate = now.AddDays(-2).ToString("yyyy-MM-dd"),
                LocationName = "清迈·古城", CreatedAt = now.AddDays(-2).ToString("yyyy-MM-dd HH:mm:ss") },

            new FoodEntry { CatalogNumber = "FV-0045", Name = "鱼香肉丝", Region = "川渝", Rarity = "珍藏",
                StarRating = 5, PrimaryTaste = "复合", AromaTag = "蒜香", TextureTag = "软糯",
                Ingredients = "猪里脊,木耳,笋丝,泡椒,葱姜蒜", PriceRange = "30~60",
                Description = "没有鱼的鱼香，却是最经典的川菜味道", CollectStatus = "已收藏",
                IsShowcase = true, DiscoverDate = now.AddDays(-1).ToString("yyyy-MM-dd"),
                LocationName = "成都·人民公园", CreatedAt = now.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss") },
        };

        foreach (var entry in entries)
            await _foodEntryRepo.InsertAsync(entry);
    }

    /// <summary>
    /// 填充心愿清单
    /// </summary>
    private async Task SeedWishItemsAsync()
    {
        var items = new WishItem[0];

        foreach (var item in items)
            await _wishItemRepo.InsertAsync(item);
    }

    /// <summary>
    /// 填充地点标记
    /// </summary>
    private async Task SeedPlaceMarksAsync()
    {
        var marks = new[]
        {
            new PlaceMark { Name = "宽窄巷子小吃街", Latitude = 30.6697, Longitude = 104.0559, Region = "川渝", CreatedAt = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd HH:mm:ss") },
            new PlaceMark { Name = "城隍庙美食广场", Latitude = 31.2275, Longitude = 121.4920, Region = "江南", CreatedAt = DateTime.Now.AddDays(-27).ToString("yyyy-MM-dd HH:mm:ss") },
            new PlaceMark { Name = "回民街", Latitude = 34.2610, Longitude = 108.9430, Region = "西北", CreatedAt = DateTime.Now.AddDays(-16).ToString("yyyy-MM-dd HH:mm:ss") },
        };

        foreach (var mark in marks)
            await _placeMarkRepo.SaveAsync(mark);
    }

    /// <summary>
    /// 填充用户配置
    /// </summary>
    private async Task SeedUserProfileAsync()
    {
        await _userProfileRepo.SetAsync("userName", "美食探险家");
        await _userProfileRepo.SetAsync("seeded", "true");
    }
}
