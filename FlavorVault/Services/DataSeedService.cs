using FlavorVault.Models;

namespace FlavorVault.Services;

/// <summary>
/// Data seed service, populates sample data on first launch
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
    /// Check and perform initial data seeding
    /// </summary>
    public async Task SeedIfNeededAsync()
    {
        await _dbService.Init();

        // Check if data already exists
        var count = await _foodEntryRepo.GetCountAsync();
        if (count > 0) return;

        System.Diagnostics.Debug.WriteLine("[DataSeedService] Starting sample data seeding...");

        await SeedCollectionsAsync();
        await SeedFoodEntriesAsync();
        await SeedWishItemsAsync();
        await SeedPlaceMarksAsync();
        await SeedUserProfileAsync();

        System.Diagnostics.Debug.WriteLine("[DataSeedService] Sample data seeding completed!");
    }

    /// <summary>
    /// Seed collections
    /// </summary>
    private async Task SeedCollectionsAsync()
    {
        var collections = new[]
        {
            new Collection { Name = "Street Snacks", Description = "Classic street food from around the world", ColorTag = "Red", Theme = "Spicy" },
            new Collection { Name = "Dessert Collection", Description = "Sweet taste memories", ColorTag = "Purple", Theme = "Sweet" },
            new Collection { Name = "Home Cooking", Description = "Warm home-style flavors", ColorTag = "Green", Theme = "Home" },
            new Collection { Name = "Exotic Flavors", Description = "Explore world cuisine", ColorTag = "Blue", Theme = "Travel" },
        };

        foreach (var c in collections)
            await _collectionRepo.SaveAsync(c);
    }

    /// <summary>
    /// Seed food guide entries (40+ items)
    /// </summary>
    private async Task SeedFoodEntriesAsync()
    {
        var now = DateTime.Now;
        var entries = new[]
        {
            // ===== Sichuan =====
            new FoodEntry { CatalogNumber = "FV-0001", Name = "Mapo Tofu", Region = "Sichuan", Rarity = "Common",
                ImagePath = "seed_suanlafen.jpg",
                StarRating = 4, PrimaryTaste = "Spicy", AromaTag = "SoySauce", TextureTag = "Tender",
                Ingredients = "Tofu, Minced beef, Doubanjiang, Sichuan peppercorn, Chili powder", PriceRange = "10~30",
                Description = "Classic Sichuan dish, numbing and spicy, smooth and flavorful tofu", CollectStatus = "Collected",
                IsShowcase = true, DiscoverDate = now.AddDays(-30).ToString("yyyy-MM-dd"),
                LocationName = "Chengdu, Kuanzhai Alley", CreatedAt = now.AddDays(-30).ToString("yyyy-MM-dd HH:mm:ss") },

            new FoodEntry { CatalogNumber = "FV-0002", Name = "Chongqing Xiaomian", Region = "Sichuan", Rarity = "Common",
                ImagePath = "seed_yangrou_paomo.jpg",
                StarRating = 5, PrimaryTaste = "Spicy", AromaTag = "Garlic", TextureTag = "Firm",
                Ingredients = "Alkaline noodles, Chili oil, Sichuan peppercorn, Crushed peanuts, Pickled mustard tuber", PriceRange = "10~30",
                Description = "The soul of Chongqing breakfast, a bowl of red oil noodles to start the day", CollectStatus = "Collected",
                IsShowcase = true, DiscoverDate = now.AddDays(-28).ToString("yyyy-MM-dd"),
                LocationName = "Chongqing, Jiefangbei", CreatedAt = now.AddDays(-28).ToString("yyyy-MM-dd HH:mm:ss") },

            new FoodEntry { CatalogNumber = "FV-0003", Name = "Sichuan Boiled Fish", Region = "Sichuan", Rarity = "Recommended",
                ImagePath = "seed_huilington_niupai.jpg",
                StarRating = 5, PrimaryTaste = "Spicy", AromaTag = "SoySauce", TextureTag = "Silky",
                Ingredients = "Grass carp, Bean sprouts, Dried chili, Sichuan peppercorn, Doubanjiang", PriceRange = "60~100",
                Description = "Tender fish fillets soaked in red chili oil, numbing and spicy", CollectStatus = "Collected",
                IsShowcase = false, DiscoverDate = now.AddDays(-25).ToString("yyyy-MM-dd"),
                LocationName = "Chengdu, Chunxi Road", CreatedAt = now.AddDays(-25).ToString("yyyy-MM-dd HH:mm:ss") },

            new FoodEntry { CatalogNumber = "FV-0004", Name = "Bobo Chicken", Region = "Sichuan", Rarity = "Common",
                ImagePath = "seed_xiajiao.jpg",
                StarRating = 4, PrimaryTaste = "Spicy", AromaTag = "SoySauce", TextureTag = "Chewy",
                Ingredients = "Chicken, Lotus root slices, Potato, Bamboo skewers, Red chili oil", PriceRange = "10~30",
                Description = "Cold skewers marinated in red chili oil, savory and flavorful", CollectStatus = "Collected",
                IsShowcase = false, DiscoverDate = now.AddDays(-20).ToString("yyyy-MM-dd"),
                LocationName = "Chengdu, Jinli", CreatedAt = now.AddDays(-20).ToString("yyyy-MM-dd HH:mm:ss") },

            new FoodEntry { CatalogNumber = "FV-0005", Name = "Sliced Beef with Chili Sauce", Region = "Sichuan", Rarity = "Recommended",
                ImagePath = "seed_dongyingong.jpg",
                StarRating = 4, PrimaryTaste = "Spicy", AromaTag = "SoySauce", TextureTag = "Firm",
                Ingredients = "Beef, Beef offal, Sichuan peppercorn, Chili oil, Sesame seeds", PriceRange = "30~60",
                Description = "Famous cold dish soaked in red chili oil, numbing and spicy", CollectStatus = "Collected",
                IsShowcase = false, DiscoverDate = now.AddDays(-18).ToString("yyyy-MM-dd"),
                LocationName = "Chengdu, Zongfu Road", CreatedAt = now.AddDays(-18).ToString("yyyy-MM-dd HH:mm:ss") },

            // ===== Cantonese =====
            new FoodEntry { CatalogNumber = "FV-0006", Name = "Shrimp Dumplings", Region = "Cantonese", Rarity = "Common",
                ImagePath = "seed_xiajiao.jpg",
                StarRating = 5, PrimaryTaste = "Umami", AromaTag = "None", TextureTag = "Tender",
                Ingredients = "Fresh shrimp, Wheat starch, Lard, Bamboo shoots", PriceRange = "30~60",
                Description = "Crystal-clear shrimp dumpling, each bite bursts with freshness", CollectStatus = "Collected",
                IsShowcase = true, DiscoverDate = now.AddDays(-26).ToString("yyyy-MM-dd"),
                LocationName = "Guangzhou, Shangxiajiu", CreatedAt = now.AddDays(-26).ToString("yyyy-MM-dd HH:mm:ss") },

            new FoodEntry { CatalogNumber = "FV-0007", Name = "Char Siu", Region = "Cantonese", Rarity = "Recommended",
                ImagePath = "seed_huiguo_rou.jpg",
                StarRating = 5, PrimaryTaste = "Sweet", AromaTag = "SoySauce", TextureTag = "Tender",
                Ingredients = "Pork belly, Honey glaze, Char Siu sauce, Rose wine", PriceRange = "30~60",
                Description = "Honey-glazed BBQ pork, aromatic and perfectly balanced sweet and savory", CollectStatus = "Collected",
                IsShowcase = false, DiscoverDate = now.AddDays(-24).ToString("yyyy-MM-dd"),
                LocationName = "Hong Kong, Sham Shui Po", CreatedAt = now.AddDays(-24).ToString("yyyy-MM-dd HH:mm:ss") },

            new FoodEntry { CatalogNumber = "FV-0008", Name = "Rice Noodle Rolls", Region = "Cantonese", Rarity = "Common",
                ImagePath = "seed_xiajiao.jpg",
                StarRating = 4, PrimaryTaste = "Umami", AromaTag = "SoySauce", TextureTag = "Silky",
                Ingredients = "Rice batter, Egg, Shrimp, Soy sauce, Scallions", PriceRange = "~10",
                Description = "Silky rice noodle rolls with special soy sauce, a breakfast favorite", CollectStatus = "Collected",
                IsShowcase = false, DiscoverDate = now.AddDays(-22).ToString("yyyy-MM-dd"),
                LocationName = "Guangzhou, Xiguan", CreatedAt = now.AddDays(-22).ToString("yyyy-MM-dd HH:mm:ss") },

            new FoodEntry { CatalogNumber = "FV-0009", Name = "Pineapple Bun", Region = "Cantonese", Rarity = "Common",
                ImagePath = "seed_fotiaoqiang.jpg",
                StarRating = 4, PrimaryTaste = "Sweet", AromaTag = "Creamy", TextureTag = "Crispy",
                Ingredients = "Flour, Butter, Sugar, Egg, Pineapple crust topping", PriceRange = "~10",
                Description = "Crispy pineapple crust topping on soft bread, with a slice of butter inside", CollectStatus = "WantToTry",
                IsShowcase = false, DiscoverDate = now.AddDays(-15).ToString("yyyy-MM-dd"),
                LocationName = "Hong Kong, Mong Kok", CreatedAt = now.AddDays(-15).ToString("yyyy-MM-dd HH:mm:ss") },

            // ===== Jiangnan =====
            new FoodEntry { CatalogNumber = "FV-0010", Name = "Xiaolongbao", Region = "Jiangnan", Rarity = "Common",
                ImagePath = "seed_xiaolongbao.jpg",
                StarRating = 5, PrimaryTaste = "Umami", AromaTag = "None", TextureTag = "Tender",
                Ingredients = "Pork filling, Flour, Ginger juice, Crab roe, Chicken broth jelly", PriceRange = "30~60",
                Description = "Thin-skinned Nanxiang Xiaolongbao bursting with savory broth, dipped in vinegar", CollectStatus = "Collected",
                IsShowcase = true, DiscoverDate = now.AddDays(-27).ToString("yyyy-MM-dd"),
                LocationName = "Shanghai, City God Temple", CreatedAt = now.AddDays(-27).ToString("yyyy-MM-dd HH:mm:ss") },

            new FoodEntry { CatalogNumber = "FV-0011", Name = "Dongpo Pork", Region = "Jiangnan", Rarity = "Recommended",
                ImagePath = "seed_huiguo_rou.jpg",
                StarRating = 5, PrimaryTaste = "Sweet", AromaTag = "SoySauce", TextureTag = "Tender",
                Ingredients = "Pork belly, Shaoxing wine, Soy sauce, Rock sugar, Scallion and ginger", PriceRange = "60~100",
                Description = "Tender and flavorful, rich but not greasy, melts in your mouth", CollectStatus = "Collected",
                IsShowcase = false, DiscoverDate = now.AddDays(-23).ToString("yyyy-MM-dd"),
                LocationName = "Hangzhou, Lou Wai Lou", CreatedAt = now.AddDays(-23).ToString("yyyy-MM-dd HH:mm:ss") },

            new FoodEntry { CatalogNumber = "FV-0012", Name = "Osmanthus Cake", Region = "Jiangnan", Rarity = "Common",
                ImagePath = "seed_kxaifuchenbing.jpg",
                StarRating = 3, PrimaryTaste = "Sweet", AromaTag = "Floral", TextureTag = "Tender",
                Ingredients = "Glutinous rice flour, Osmanthus flowers, Sugar, Honey", PriceRange = "~10",
                Description = "Sweet osmanthus aroma with soft and tender texture", CollectStatus = "Collected",
                IsShowcase = false, DiscoverDate = now.AddDays(-19).ToString("yyyy-MM-dd"),
                LocationName = "Suzhou, Guanqian Street", CreatedAt = now.AddDays(-19).ToString("yyyy-MM-dd HH:mm:ss") },

            new FoodEntry { CatalogNumber = "FV-0013", Name = "Shengjian Bao", Region = "Jiangnan", Rarity = "Common",
                ImagePath = "seed_xiajiao.jpg",
                StarRating = 4, PrimaryTaste = "Umami", AromaTag = "SoySauce", TextureTag = "Crispy",
                Ingredients = "Pork filling, Flour, Sesame seeds, Scallions, Meat jelly", PriceRange = "10~30",
                Description = "Golden crispy bottom, bite into it and the savory juices burst out", CollectStatus = "Collected",
                IsShowcase = false, DiscoverDate = now.AddDays(-17).ToString("yyyy-MM-dd"),
                LocationName = "Shanghai, Xiao Yang Shengjian", CreatedAt = now.AddDays(-17).ToString("yyyy-MM-dd HH:mm:ss") },

            // ===== Northern =====
            new FoodEntry { CatalogNumber = "FV-0014", Name = "Peking Duck", Region = "Northern", Rarity = "Recommended",
                ImagePath = "seed_shao_e.jpg",
                StarRating = 5, PrimaryTaste = "Salty", AromaTag = "Fruity", TextureTag = "Crispy",
                Ingredients = "Force-fed duck, Sweet bean sauce, Scallions, Cucumber, Thin pancakes", PriceRange = "100+",
                Description = "Date-red crispy duck skin, dipped in sweet bean sauce and wrapped in a pancake", CollectStatus = "Collected",
                IsShowcase = true, DiscoverDate = now.AddDays(-29).ToString("yyyy-MM-dd"),
                LocationName = "Beijing, Quanjude", CreatedAt = now.AddDays(-29).ToString("yyyy-MM-dd HH:mm:ss") },

            new FoodEntry { CatalogNumber = "FV-0015", Name = "Zhajiangmian", Region = "Northern", Rarity = "Common",
                ImagePath = "seed_yangrou_paomo.jpg",
                StarRating = 4, PrimaryTaste = "Salty", AromaTag = "SoySauce", TextureTag = "Firm",
                Ingredients = "Hand-pulled noodles, Yellow soybean paste, Diced pork, Shredded cucumber, Bean sprouts", PriceRange = "10~30",
                Description = "Old Beijing flavor, rich fried sauce paired with smooth noodles", CollectStatus = "Collected",
                IsShowcase = false, DiscoverDate = now.AddDays(-21).ToString("yyyy-MM-dd"),
                LocationName = "Beijing, Gulou East Street", CreatedAt = now.AddDays(-21).ToString("yyyy-MM-dd HH:mm:ss") },

            new FoodEntry { CatalogNumber = "FV-0016", Name = "Donkey Meat Sandwich", Region = "Northern", Rarity = "Recommended",
                ImagePath = "seed_budabu.jpg",
                StarRating = 4, PrimaryTaste = "Umami", AromaTag = "None", TextureTag = "Crispy",
                Ingredients = "Donkey meat, Baked bread, Green pepper, Cilantro", PriceRange = "10~30",
                Description = "Crispy on the outside, tender on the inside - a Hebei classic", CollectStatus = "WantToTry",
                IsShowcase = false, DiscoverDate = now.AddDays(-14).ToString("yyyy-MM-dd"),
                LocationName = "Baoding, Ancient Lotus Pond", CreatedAt = now.AddDays(-14).ToString("yyyy-MM-dd HH:mm:ss") },

            new FoodEntry { CatalogNumber = "FV-0017", Name = "Sweet and Sour Pork", Region = "Northern", Rarity = "Common",
                ImagePath = "seed_huiguo_rou.jpg",
                StarRating = 4, PrimaryTaste = "Sour", AromaTag = "Fruity", TextureTag = "Crispy",
                Ingredients = "Pork tenderloin, Starch, Sugar, White vinegar, Tomato sauce", PriceRange = "30~60",
                Description = "A Northeast classic, crispy outside and tender inside, sweet and sour", CollectStatus = "Collected",
                IsShowcase = false, DiscoverDate = now.AddDays(-12).ToString("yyyy-MM-dd"),
                LocationName = "Harbin, Central Street", CreatedAt = now.AddDays(-12).ToString("yyyy-MM-dd HH:mm:ss") },

            // ===== Northwest =====
            new FoodEntry { CatalogNumber = "FV-0018", Name = "Lanzhou Beef Noodles", Region = "Northwest", Rarity = "Common",
                ImagePath = "seed_yangrou_paomo.jpg",
                StarRating = 5, PrimaryTaste = "Umami", AromaTag = "Herbal", TextureTag = "Firm",
                Ingredients = "High-gluten flour, Beef, Radish, Chili oil, Cilantro", PriceRange = "~10",
                Description = "Five colors in one bowl: clear broth, white radish, red chili, green herbs, yellow noodles", CollectStatus = "Collected",
                IsShowcase = true, DiscoverDate = now.AddDays(-25).ToString("yyyy-MM-dd"),
                LocationName = "Lanzhou, Zhengning Road", CreatedAt = now.AddDays(-25).ToString("yyyy-MM-dd HH:mm:ss") },

            new FoodEntry { CatalogNumber = "FV-0019", Name = "Pita with Lamb Soup", Region = "Northwest", Rarity = "Recommended",
                ImagePath = "seed_huilington_niupai.jpg",
                StarRating = 4, PrimaryTaste = "Umami", AromaTag = "Herbal", TextureTag = "Rich",
                Ingredients = "Lamb, Baked flatbread, Glass noodles, Wood ear mushrooms, Scallion and ginger", PriceRange = "30~60",
                Description = "Rich broth soaked into torn flatbread pieces, warming and satisfying", CollectStatus = "Collected",
                IsShowcase = false, DiscoverDate = now.AddDays(-16).ToString("yyyy-MM-dd"),
                LocationName = "Xi'an, Muslim Quarter", CreatedAt = now.AddDays(-16).ToString("yyyy-MM-dd HH:mm:ss") },

            new FoodEntry { CatalogNumber = "FV-0020", Name = "Roujiamo", Region = "Northwest", Rarity = "Common",
                ImagePath = "seed_budabu.jpg",
                StarRating = 4, PrimaryTaste = "Salty", AromaTag = "SoySauce", TextureTag = "Crispy",
                Ingredients = "Baiji flatbread, Braised pork, Green pepper, Cilantro", PriceRange = "10~30",
                Description = "Crispy flatbread stuffed with slow-braised tender pork", CollectStatus = "Collected",
                IsShowcase = false, DiscoverDate = now.AddDays(-13).ToString("yyyy-MM-dd"),
                LocationName = "Xi'an, Yongxingfang", CreatedAt = now.AddDays(-13).ToString("yyyy-MM-dd HH:mm:ss") },

            // ===== Japan =====
            new FoodEntry { CatalogNumber = "FV-0021", Name = "Ramen", Region = "Japan", Rarity = "Common",
                ImagePath = "seed_lanzhou_lamian.jpg",
                StarRating = 5, PrimaryTaste = "Umami", AromaTag = "Garlic", TextureTag = "Firm",
                Ingredients = "Noodles, Pork bone broth, Chashu pork, Soft-boiled egg, Nori", PriceRange = "30~60",
                Description = "Rich pork bone broth paired with chewy noodles", CollectStatus = "Collected",
                IsShowcase = true, DiscoverDate = now.AddDays(-22).ToString("yyyy-MM-dd"),
                LocationName = "Tokyo, Shinjuku", CreatedAt = now.AddDays(-22).ToString("yyyy-MM-dd HH:mm:ss") },

            new FoodEntry { CatalogNumber = "FV-0022", Name = "Sushi", Region = "Japan", Rarity = "Recommended",
                ImagePath = "seed_kaishiliaoli.jpg",
                StarRating = 5, PrimaryTaste = "Umami", AromaTag = "None", TextureTag = "Tender",
                Ingredients = "Sushi rice, Salmon, Tuna, Nori, Wasabi", PriceRange = "100+",
                Description = "Fresh sashimi on vinegared rice, simple yet exquisite", CollectStatus = "Collected",
                IsShowcase = false, DiscoverDate = now.AddDays(-10).ToString("yyyy-MM-dd"),
                LocationName = "Tokyo, Tsukiji Market", CreatedAt = now.AddDays(-10).ToString("yyyy-MM-dd HH:mm:ss") },

            new FoodEntry { CatalogNumber = "FV-0023", Name = "Tempura", Region = "Japan", Rarity = "Common",
                ImagePath = "seed_yangrou_paomo.jpg",
                StarRating = 4, PrimaryTaste = "Umami", AromaTag = "None", TextureTag = "Crispy",
                Ingredients = "Shrimp, Vegetables, Flour, Egg, Frying oil", PriceRange = "60~100",
                Description = "Light and crispy batter wrapped around fresh ingredients", CollectStatus = "Collected",
                IsShowcase = false, DiscoverDate = now.AddDays(-8).ToString("yyyy-MM-dd"),
                LocationName = "Kyoto, Gion", CreatedAt = now.AddDays(-8).ToString("yyyy-MM-dd HH:mm:ss") },

            new FoodEntry { CatalogNumber = "FV-0024", Name = "Matcha Desserts", Region = "Japan", Rarity = "Recommended",
                ImagePath = "seed_kxaifuchenbing.jpg",
                StarRating = 4, PrimaryTaste = "Sweet", AromaTag = "Herbal", TextureTag = "Silky",
                Ingredients = "Matcha powder, Milk, Red bean, Mochi, Shiratamako", PriceRange = "30~60",
                Description = "Rich Uji matcha paired with delicate desserts", CollectStatus = "WantToTry",
                IsShowcase = false, DiscoverDate = now.AddDays(-5).ToString("yyyy-MM-dd"),
                LocationName = "Kyoto, Uji", CreatedAt = now.AddDays(-5).ToString("yyyy-MM-dd HH:mm:ss") },

            // ===== Korea =====
            new FoodEntry { CatalogNumber = "FV-0025", Name = "Korean BBQ", Region = "Korea", Rarity = "Common",
                ImagePath = "seed_hanguo_zhaji.jpg",
                StarRating = 4, PrimaryTaste = "Salty", AromaTag = "Smoky", TextureTag = "Chewy",
                Ingredients = "Pork belly, Lettuce, Garlic cloves, Gochujang, Doenjang", PriceRange = "60~100",
                Description = "Charcoal-grilled pork belly wrapped in fresh lettuce leaves", CollectStatus = "Collected",
                IsShowcase = false, DiscoverDate = now.AddDays(-19).ToString("yyyy-MM-dd"),
                LocationName = "Seoul, Myeongdong", CreatedAt = now.AddDays(-19).ToString("yyyy-MM-dd HH:mm:ss") },

            new FoodEntry { CatalogNumber = "FV-0026", Name = "Bibimbap", Region = "Korea", Rarity = "Common",
                ImagePath = "seed_huilington_niupai.jpg",
                StarRating = 4, PrimaryTaste = "Complex", AromaTag = "SoySauce", TextureTag = "Crispy",
                Ingredients = "Rice, Gochujang, Egg, Various vegetables, Sesame oil", PriceRange = "30~60",
                Description = "Sizzling stone pot with crispy rice crust paired with spicy sauce", CollectStatus = "Collected",
                IsShowcase = false, DiscoverDate = now.AddDays(-11).ToString("yyyy-MM-dd"),
                LocationName = "Seoul, Hongdae", CreatedAt = now.AddDays(-11).ToString("yyyy-MM-dd HH:mm:ss") },

            new FoodEntry { CatalogNumber = "FV-0027", Name = "Korean Fried Chicken", Region = "Korea", Rarity = "Common",
                ImagePath = "seed_jiaohuaji.jpg",
                StarRating = 5, PrimaryTaste = "Salty", AromaTag = "Garlic", TextureTag = "Crispy",
                Ingredients = "Chicken, Flour, Korean chili sauce, Honey, Minced garlic", PriceRange = "30~60",
                Description = "Crispy outer skin coated in sweet and spicy sauce, perfect with beer", CollectStatus = "Collected",
                IsShowcase = false, DiscoverDate = now.AddDays(-7).ToString("yyyy-MM-dd"),
                LocationName = "Seoul, Gangnam", CreatedAt = now.AddDays(-7).ToString("yyyy-MM-dd HH:mm:ss") },

            // ===== SoutheastAsia =====
            new FoodEntry { CatalogNumber = "FV-0028", Name = "Tom Yum Soup", Region = "SoutheastAsia", Rarity = "Recommended",
                ImagePath = "seed_dongyingong.jpg",
                StarRating = 4, PrimaryTaste = "Sour", AromaTag = "Herbal", TextureTag = "Refreshing",
                Ingredients = "Shrimp, Lemongrass, Galangal, Kaffir lime leaves, Chili", PriceRange = "30~60",
                Description = "A Thai classic that is sour, spicy, and aromatic - tropical flavors in a bowl", CollectStatus = "Collected",
                IsShowcase = false, DiscoverDate = now.AddDays(-15).ToString("yyyy-MM-dd"),
                LocationName = "Bangkok, Khao San Road", CreatedAt = now.AddDays(-15).ToString("yyyy-MM-dd HH:mm:ss") },

            new FoodEntry { CatalogNumber = "FV-0029", Name = "Vietnamese Pho", Region = "SoutheastAsia", Rarity = "Common",
                ImagePath = "seed_yangrou_paomo.jpg",
                StarRating = 4, PrimaryTaste = "Umami", AromaTag = "Herbal", TextureTag = "Silky",
                Ingredients = "Rice noodles, Beef, Bean sprouts, Thai basil, Fish sauce", PriceRange = "10~30",
                Description = "Light and savory beef pho, served with a plate of fresh herbs", CollectStatus = "Collected",
                IsShowcase = false, DiscoverDate = now.AddDays(-9).ToString("yyyy-MM-dd"),
                LocationName = "Ho Chi Minh City, Ben Thanh Market", CreatedAt = now.AddDays(-9).ToString("yyyy-MM-dd HH:mm:ss") },

            new FoodEntry { CatalogNumber = "FV-0030", Name = "Singapore Chili Crab", Region = "SoutheastAsia", Rarity = "Limited",
                ImagePath = "seed_shao_e.jpg",
                StarRating = 5, PrimaryTaste = "Complex", AromaTag = "Fruity", TextureTag = "Rich",
                Ingredients = "Crab, Tomato sauce, Chili sauce, Egg, Scallion, ginger, garlic", PriceRange = "100+",
                Description = "Thick chili sauce coating sweet and succulent crab meat", CollectStatus = "WantToTry",
                IsShowcase = false, DiscoverDate = now.AddDays(-4).ToString("yyyy-MM-dd"),
                LocationName = "Singapore, Newton Food Centre", CreatedAt = now.AddDays(-4).ToString("yyyy-MM-dd HH:mm:ss") },

            // ===== Western =====
            new FoodEntry { CatalogNumber = "FV-0031", Name = "Italian Pizza", Region = "Western", Rarity = "Common",
                ImagePath = "seed_budabu.jpg",
                StarRating = 4, PrimaryTaste = "Salty", AromaTag = "Fruity", TextureTag = "Crispy",
                Ingredients = "Dough, Tomato sauce, Mozzarella cheese, Basil, Olive oil", PriceRange = "30~60",
                Description = "Wood-fired thin-crust pizza, crispy edges and soft center", CollectStatus = "Collected",
                IsShowcase = false, DiscoverDate = now.AddDays(-18).ToString("yyyy-MM-dd"),
                LocationName = "Rome, Trastevere", CreatedAt = now.AddDays(-18).ToString("yyyy-MM-dd HH:mm:ss") },

            new FoodEntry { CatalogNumber = "FV-0032", Name = "French Croissant", Region = "Western", Rarity = "Common",
                ImagePath = "seed_fotiaoqiang.jpg",
                StarRating = 5, PrimaryTaste = "Salty", AromaTag = "Creamy", TextureTag = "Crispy",
                Ingredients = "Flour, Butter, Yeast, Milk, Salt", PriceRange = "10~30",
                Description = "Layered flaky pastry with a wonderful butter aroma", CollectStatus = "Collected",
                IsShowcase = false, DiscoverDate = now.AddDays(-6).ToString("yyyy-MM-dd"),
                LocationName = "Paris, Montmartre", CreatedAt = now.AddDays(-6).ToString("yyyy-MM-dd HH:mm:ss") },

            new FoodEntry { CatalogNumber = "FV-0033", Name = "Hamburger", Region = "Western", Rarity = "Common",
                ImagePath = "seed_yuenan_hefen.jpg",
                StarRating = 4, PrimaryTaste = "Salty", AromaTag = "Smoky", TextureTag = "Tender",
                Ingredients = "Beef patty, Cheese, Lettuce, Tomato, Bun", PriceRange = "30~60",
                Description = "Juicy beef patty topped with melted cheese", CollectStatus = "Collected",
                IsShowcase = false, DiscoverDate = now.AddDays(-3).ToString("yyyy-MM-dd"),
                LocationName = "New York, Brooklyn", CreatedAt = now.AddDays(-3).ToString("yyyy-MM-dd HH:mm:ss") },

            new FoodEntry { CatalogNumber = "FV-0034", Name = "Tiramisu", Region = "Western", Rarity = "Recommended",
                ImagePath = "seed_kxaifuchenbing.jpg",
                StarRating = 5, PrimaryTaste = "Sweet", AromaTag = "Creamy", TextureTag = "Silky",
                Ingredients = "Mascarpone cheese, Ladyfingers, Espresso, Cocoa powder, Eggs", PriceRange = "30~60",
                Description = "Layers of sweetness, the perfect blend of coffee and cheese", CollectStatus = "Collected",
                IsShowcase = false, DiscoverDate = now.AddDays(-2).ToString("yyyy-MM-dd"),
                LocationName = "Venice, St. Mark's Square", CreatedAt = now.AddDays(-2).ToString("yyyy-MM-dd HH:mm:ss") },

            // Additional curated entries (supplementing regions)
            new FoodEntry { CatalogNumber = "FV-0035", Name = "Mao Xue Wang", Region = "Sichuan", Rarity = "Recommended",
                StarRating = 4, PrimaryTaste = "Spicy", AromaTag = "Garlic", TextureTag = "Chewy",
                Ingredients = "Duck blood curd, Beef tripe, Luncheon meat, Bean sprouts, Hotpot base", PriceRange = "60~100",
                Description = "Bubbling red broth packed with ingredients, numbing and spicy", CollectStatus = "Collected",
                DiscoverDate = now.AddDays(-11).ToString("yyyy-MM-dd"),
                LocationName = "Chongqing, Ciqikou", CreatedAt = now.AddDays(-11).ToString("yyyy-MM-dd HH:mm:ss") },

            new FoodEntry { CatalogNumber = "FV-0036", Name = "Sweet and Sour Spareribs", Region = "Jiangnan", Rarity = "Common",
                StarRating = 4, PrimaryTaste = "Sour", AromaTag = "Fruity", TextureTag = "Crispy",
                Ingredients = "Pork ribs, Sugar, Vinegar, Tomato sauce, Scallion and ginger", PriceRange = "30~60",
                Description = "Glossy red color, perfectly balanced sweet and sour, crispy outside and tender inside", CollectStatus = "Collected",
                DiscoverDate = now.AddDays(-8).ToString("yyyy-MM-dd"),
                LocationName = "Wuxi, Nanchang Street", CreatedAt = now.AddDays(-8).ToString("yyyy-MM-dd HH:mm:ss") },

            new FoodEntry { CatalogNumber = "FV-0037", Name = "Claypot Rice", Region = "Cantonese", Rarity = "Recommended",
                StarRating = 5, PrimaryTaste = "Salty", AromaTag = "SoySauce", TextureTag = "Crispy",
                Ingredients = "Long-grain rice, Chinese sausage, Soy sauce, Egg, Greens", PriceRange = "30~60",
                Description = "Crispy rice crust at the bottom, rich cured meat flavors, one pot satisfies all", CollectStatus = "Collected",
                DiscoverDate = now.AddDays(-6).ToString("yyyy-MM-dd"),
                LocationName = "Guangzhou, Beijing Road", CreatedAt = now.AddDays(-6).ToString("yyyy-MM-dd HH:mm:ss") },

            new FoodEntry { CatalogNumber = "FV-0038", Name = "Army Stew", Region = "Korea", Rarity = "Common",
                StarRating = 3, PrimaryTaste = "Spicy", AromaTag = "Smoky", TextureTag = "Rich",
                Ingredients = "Kimchi, Luncheon meat, Instant noodles, Cheese, Rice cakes", PriceRange = "60~100",
                Description = "Steaming hot army stew, loaded with ingredients", CollectStatus = "Collected",
                DiscoverDate = now.AddDays(-4).ToString("yyyy-MM-dd"),
                LocationName = "Seoul, Myeongdong", CreatedAt = now.AddDays(-4).ToString("yyyy-MM-dd HH:mm:ss") },

            new FoodEntry { CatalogNumber = "FV-0039", Name = "Japanese Curry Rice", Region = "Japan", Rarity = "Common",
                StarRating = 4, PrimaryTaste = "Salty", AromaTag = "Fruity", TextureTag = "Rich",
                Ingredients = "Curry roux, Potato, Carrot, Onion, Pork", PriceRange = "30~60",
                Description = "Thick Japanese curry over white rice, warm and comforting", CollectStatus = "Collected",
                DiscoverDate = now.AddDays(-3).ToString("yyyy-MM-dd"),
                LocationName = "Osaka, Dotonbori", CreatedAt = now.AddDays(-3).ToString("yyyy-MM-dd HH:mm:ss") },

            new FoodEntry { CatalogNumber = "FV-0040", Name = "Lamb Skewers", Region = "Northwest", Rarity = "Common",
                StarRating = 4, PrimaryTaste = "Salty", AromaTag = "Smoky", TextureTag = "Chewy",
                Ingredients = "Lamb, Cumin, Chili powder, Salt, Onion", PriceRange = "~10",
                Description = "Charcoal-grilled lamb skewers sprinkled with cumin and chili powder", CollectStatus = "Collected",
                DiscoverDate = now.AddDays(-1).ToString("yyyy-MM-dd"),
                LocationName = "Urumqi, Grand Bazaar", CreatedAt = now.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss") },

            new FoodEntry { CatalogNumber = "FV-0041", Name = "Luosifen (Snail Rice Noodles)", Region = "Cantonese", Rarity = "Limited",
                StarRating = 4, PrimaryTaste = "Spicy", AromaTag = "Herbal", TextureTag = "Firm",
                Ingredients = "Rice noodles, Fermented bamboo shoots, Fried tofu skin, Peanuts, Snail broth", PriceRange = "10~30",
                Description = "Pungent aroma but addictive taste, the more you eat the more you crave", CollectStatus = "Collected",
                IsShowcase = false, DiscoverDate = now.ToString("yyyy-MM-dd"),
                LocationName = "Liuzhou, Wuxing Street", CreatedAt = now.ToString("yyyy-MM-dd HH:mm:ss") },

            new FoodEntry { CatalogNumber = "FV-0042", Name = "Macarons", Region = "Western", Rarity = "Limited",
                ImagePath = "seed_kxaifuchenbing.jpg",
                StarRating = 4, PrimaryTaste = "Sweet", AromaTag = "Creamy", TextureTag = "Crispy",
                Ingredients = "Almond flour, Powdered sugar, Egg whites, Food coloring, Cream", PriceRange = "30~60",
                Description = "Colorful French macarons with a crispy shell and soft center", CollectStatus = "WantToTry",
                DiscoverDate = now.ToString("yyyy-MM-dd"),
                LocationName = "Paris, Champs-Elysees", CreatedAt = now.ToString("yyyy-MM-dd HH:mm:ss") },

            new FoodEntry { CatalogNumber = "FV-0043", Name = "Jianbing Guozi", Region = "Northern", Rarity = "Common",
                StarRating = 4, PrimaryTaste = "Salty", AromaTag = "SoySauce", TextureTag = "Crispy",
                Ingredients = "Mung bean flour, Egg, Crispy cracker, Scallions, Sweet bean sauce", PriceRange = "~10",
                Description = "A Tianjin breakfast staple, one Jianbing to kickstart the day", CollectStatus = "Collected",
                DiscoverDate = now.AddDays(-7).ToString("yyyy-MM-dd"),
                LocationName = "Tianjin, Ancient Culture Street", CreatedAt = now.AddDays(-7).ToString("yyyy-MM-dd HH:mm:ss") },

            new FoodEntry { CatalogNumber = "FV-0044", Name = "Mango Sticky Rice", Region = "SoutheastAsia", Rarity = "Common",
                StarRating = 4, PrimaryTaste = "Sweet", AromaTag = "Creamy", TextureTag = "Tender",
                Ingredients = "Sticky rice, Mango, Coconut cream, Sesame seeds, Sugar", PriceRange = "10~30",
                Description = "Sweet mango paired with coconut-flavored sticky rice, a tropical delight", CollectStatus = "Collected",
                DiscoverDate = now.AddDays(-2).ToString("yyyy-MM-dd"),
                LocationName = "Chiang Mai, Old City", CreatedAt = now.AddDays(-2).ToString("yyyy-MM-dd HH:mm:ss") },

            new FoodEntry { CatalogNumber = "FV-0045", Name = "Yuxiang Shredded Pork", Region = "Sichuan", Rarity = "Premium",
                StarRating = 5, PrimaryTaste = "Complex", AromaTag = "Garlic", TextureTag = "Tender",
                Ingredients = "Pork tenderloin, Wood ear mushrooms, Bamboo shoot strips, Pickled chili, Scallion, ginger, garlic", PriceRange = "30~60",
                Description = "No fish in this 'fish-flavored' dish, yet it's the most classic Sichuan taste", CollectStatus = "Collected",
                IsShowcase = true, DiscoverDate = now.AddDays(-1).ToString("yyyy-MM-dd"),
                LocationName = "Chengdu, People's Park", CreatedAt = now.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss") },
        };

        foreach (var entry in entries)
            await _foodEntryRepo.InsertAsync(entry);
    }

    /// <summary>
    /// Seed wish list items
    /// </summary>
    private async Task SeedWishItemsAsync()
    {
        var items = new WishItem[0];

        foreach (var item in items)
            await _wishItemRepo.InsertAsync(item);
    }

    /// <summary>
    /// Seed place marks
    /// </summary>
    private async Task SeedPlaceMarksAsync()
    {
        var marks = new[]
        {
            new PlaceMark { Name = "Kuanzhai Alley Snack Street", Latitude = 30.6697, Longitude = 104.0559, Region = "Sichuan", CreatedAt = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd HH:mm:ss") },
            new PlaceMark { Name = "City God Temple Food Plaza", Latitude = 31.2275, Longitude = 121.4920, Region = "Jiangnan", CreatedAt = DateTime.Now.AddDays(-27).ToString("yyyy-MM-dd HH:mm:ss") },
            new PlaceMark { Name = "Muslim Quarter (Huimin Street)", Latitude = 34.2610, Longitude = 108.9430, Region = "Northwest", CreatedAt = DateTime.Now.AddDays(-16).ToString("yyyy-MM-dd HH:mm:ss") },
        };

        foreach (var mark in marks)
            await _placeMarkRepo.SaveAsync(mark);
    }

    /// <summary>
    /// Seed user profile settings
    /// </summary>
    private async Task SeedUserProfileAsync()
    {
        await _userProfileRepo.SetAsync("userName", "Food Explorer");
        await _userProfileRepo.SetAsync("seeded", "true");
    }
}
