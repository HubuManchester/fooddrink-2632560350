using FlavorVault.Models;

namespace FlavorVault.Services;

/// <summary>
/// Seed data initialization service
/// Version-controlled via seed_version, only re-populates when version does not match
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
    /// Initialize seed data, skip if seed_version already matches
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
                Name = "Mapo Tofu",
                Region = "Sichuan",
                Rarity = "Common",
                ImagePath = "seed_mapo_tofu.jpg",
                StarRating = 5,
                PrimaryTaste = "Spicy",
                AromaTag = "Herbal",
                TextureTag = "Tender",
                Description = "Mapo Tofu is a traditional famous dish from Sichuan province, belonging to Sichuan cuisine. The main ingredients are tofu, minced beef (sometimes minced pork), chili, and Sichuan peppercorn. The numbing sensation comes from Sichuan peppercorn, and the heat from chili. This dish highlights the 'mala' (numbing and spicy) characteristic of Sichuan cuisine with a unique flavor and smooth texture. Created by Chen Mapo in the first year of Tongzhi in the Qing Dynasty (1862) in Chengdu.",
                Ingredients = "[\"Tofu\",\"Minced beef\",\"Sichuan peppercorn powder\",\"Chili powder\",\"Doubanjiang\",\"Scallion, ginger, garlic\"]",
                PriceRange = "10~30",
                CollectStatus = "Collected",
                Latitude = 30.5728,
                Longitude = 104.0668,
                LocationName = "Chengdu Chen Mapo Tofu Flagship Store",
                DiscoverDate = "2025-03-15",
                NoteText = "Authentic Mapo Tofu, the numbing sensation of Sichuan peppercorn and the heat of chili perfectly blended, tofu is silky and flavorful",
                CollectionName = "Spicy Lovers",
                IsShowcase = true,
                CreatedAt = "2025-03-15 12:30:00",
                UpdatedAt = "2025-03-15 12:30:00"
            },
            new()
            {
                CatalogNumber = "FV-0002",
                Name = "Twice-Cooked Pork",
                Region = "Sichuan",
                Rarity = "Recommended",
                ImagePath = "seed_huiguo_rou.jpg",
                StarRating = 5,
                PrimaryTaste = "Salty",
                AromaTag = "SoySauce",
                TextureTag = "Firm",
                Description = "Twice-Cooked Pork is a traditional Han Chinese dish, belonging to Sichuan cuisine, one of the Eight Great Cuisines of China. The name 'twice-cooked' means cooking it again. As a traditional Sichuan dish, it holds a very important position in Sichuan cuisine and is often chosen as the first dish in Sichuan cuisine certification exams. Twice-Cooked Pork has always been considered the king of Sichuan cuisine, the very embodiment of Sichuan flavors.",
                Ingredients = "[\"Pork belly\",\"Green garlic sprouts\",\"Doubanjiang\",\"Sweet bean paste\",\"Fermented black beans\",\"Green pepper\"]",
                PriceRange = "30~60",
                CollectStatus = "Collected",
                Latitude = 30.6571,
                Longitude = 104.0657,
                LocationName = "Chengdu Shu Jiuxiang",
                DiscoverDate = "2025-04-02",
                NoteText = "The perfect example of rich but not greasy, robust soy sauce aroma, a rice killer",
                CollectionName = null,
                IsShowcase = false,
                CreatedAt = "2025-04-02 18:45:00",
                UpdatedAt = "2025-04-02 18:45:00"
            },
            new()
            {
                CatalogNumber = "FV-0003",
                Name = "Shrimp Dumplings",
                Region = "Cantonese",
                Rarity = "Common",
                ImagePath = "seed_xiajiao.jpg",
                StarRating = 4,
                PrimaryTaste = "Umami",
                AromaTag = "None",
                TextureTag = "Chewy",
                Description = "Shrimp Dumplings are a famous traditional snack from Guangdong, belonging to Cantonese cuisine. They originated in the early 20th century in Wufeng Township, Guangzhou, featuring a wheat starch wrapper enclosing one or two shrimp as the main filling. Made primarily with fresh shrimp, after steaming the skin is as thin as paper, crystal clear, with savory juices. A classic dim sum item in Cantonese morning tea.",
                Ingredients = "[\"Fresh shrimp\",\"Wheat starch\",\"Lard\",\"Bamboo shoot shreds\",\"White pepper\"]",
                PriceRange = "10~30",
                CollectStatus = "Collected",
                Latitude = 23.1291,
                Longitude = 113.2644,
                LocationName = "Guangzhou Tao Tao Ju",
                DiscoverDate = "2025-02-20",
                NoteText = "Skin as thin as paper, shrimp is chewy and fresh, a must-order for dim sum",
                CollectionName = "Street Legends",
                IsShowcase = false,
                CreatedAt = "2025-02-20 09:15:00",
                UpdatedAt = "2025-02-20 09:15:00"
            },
            new()
            {
                CatalogNumber = "FV-0004",
                Name = "Roast Goose",
                Region = "Cantonese",
                Rarity = "Recommended",
                ImagePath = "seed_shao_e.jpg",
                StarRating = 5,
                PrimaryTaste = "Complex",
                AromaTag = "Smoky",
                TextureTag = "Crispy",
                Description = "Roast Goose is a traditional Cantonese BBQ meat dish, part of Cantonese cuisine. Originating from roast duck, seasonings such as soy sauce, salt, sugar, and five-spice powder are stuffed into the goose cavity, then sewn shut, inflated to separate skin from meat, coated with malt sugar water and dried, and finally roasted at high heat. The skin is crispy and the meat tender, rich but not greasy, a premium dish for Cantonese banquets.",
                Ingredients = "[\"Black goose\",\"Five-spice powder\",\"Sand ginger powder\",\"Malt sugar\",\"Star anise\",\"Cinnamon\"]",
                PriceRange = "60~100",
                CollectStatus = "WantToTry",
                Latitude = 22.3193,
                Longitude = 114.1694,
                LocationName = "Hong Kong Sham Tseng Roast Goose",
                DiscoverDate = "2025-05-10",
                NoteText = "Always wanted to try authentic Sham Tseng roast goose, heard the skin is crispy and meat is tender and delicious",
                CollectionName = null,
                IsShowcase = false,
                CreatedAt = "2025-05-10 19:30:00",
                UpdatedAt = "2025-05-10 19:30:00"
            },
            new()
            {
                CatalogNumber = "FV-0005",
                Name = "Xiaolongbao",
                Region = "Jiangnan",
                Rarity = "Common",
                ImagePath = "seed_xiaolongbao.jpg",
                StarRating = 5,
                PrimaryTaste = "Umami",
                AromaTag = "None",
                TextureTag = "Tender",
                Description = "Xiaolongbao is a traditional Chinese steamed bun, most famous in the Jiangnan region. The Shanghai Nanxiang Xiaolongbao is the most well-known, dating back to the tenth year of Tongzhi in the Qing Dynasty (1871). With thin skin, generous filling, and plenty of soup, it uses refined white flour for the skin and pork with meat jelly for the filling. Usually eight per basket, small and delicate, with savory broth. When eating, first bite a small hole at the bottom, sip the broth, then enjoy the whole dumpling.",
                Ingredients = "[\"Pork shoulder\",\"Meat jelly\",\"All-purpose flour\",\"Scallion-ginger juice\",\"Cooking wine\",\"Sesame oil\"]",
                PriceRange = "~10",
                CollectStatus = "Collected",
                Latitude = 31.2304,
                Longitude = 121.4737,
                LocationName = "Shanghai Nanxiang Mantou Shop",
                DiscoverDate = "2025-01-18",
                NoteText = "Authentic Nanxiang Xiaolongbao, a gentle bite releases the savory broth, paired with vinegar and ginger it is perfection",
                CollectionName = "Street Legends",
                IsShowcase = false,
                CreatedAt = "2025-01-18 11:00:00",
                UpdatedAt = "2025-01-18 11:00:00"
            },
            new()
            {
                CatalogNumber = "FV-0006",
                Name = "Dongpo Pork",
                Region = "Jiangnan",
                Rarity = "Limited",
                ImagePath = "seed_dongpo_rou.jpg",
                StarRating = 5,
                PrimaryTaste = "Sweet",
                AromaTag = "SoySauce",
                TextureTag = "Tender",
                Description = "Dongpo Pork is a famous Hangzhou dish, belonging to Zhejiang cuisine. Legend has it that it was created by the Northern Song poet Su Dongpo. Made with pork belly slow-braised with wine, soy sauce, and other seasonings over low heat. The meat is tender and melts in your mouth without being greasy, with a bright red color like agate. Dongpo Pork emphasizes slow heat, minimal water, and plenty of wine. The finished dish is tender yet holds its shape, fragrant and glutinous without being cloying.",
                Ingredients = "[\"Pork belly\",\"Shaoxing yellow wine\",\"Soy sauce\",\"Rock sugar\",\"Scallion and ginger\",\"Star anise\"]",
                PriceRange = "30~60",
                CollectStatus = "Collected",
                Latitude = 30.2590,
                Longitude = 120.1551,
                LocationName = "Hangzhou Lou Wai Lou",
                DiscoverDate = "2025-03-28",
                NoteText = "Tender and flavorful, rich but not greasy, the aroma of yellow wine permeates every fiber of the meat",
                CollectionName = "Luxury Moments",
                IsShowcase = true,
                CreatedAt = "2025-03-28 13:20:00",
                UpdatedAt = "2025-03-28 13:20:00"
            },
            new()
            {
                CatalogNumber = "FV-0007",
                Name = "Zhajiangmian",
                Region = "Northern",
                Rarity = "Common",
                ImagePath = "seed_lanzhou_lamian.jpg",
                StarRating = 4,
                PrimaryTaste = "Salty",
                AromaTag = "SoySauce",
                TextureTag = "Firm",
                Description = "Zhajiangmian is a traditional Chinese specialty noodle dish, recognized as one of the 'Top Ten Chinese Noodles'. It originated in Beijing and belongs to Beijing cuisine. The soul of Zhajiangmian lies in the fried sauce, based on yellow soybean paste and sweet bean paste, with diced meat slowly stir-fried until the sauce is rich and aromatic. Paired with shredded cucumber, bean sprouts, shredded radish and other toppings, every strand of noodle is coated with savory sauce when mixed.",
                Ingredients = "[\"Hand-rolled noodles\",\"Diced pork belly\",\"Yellow soybean paste\",\"Sweet bean paste\",\"Shredded cucumber\",\"Bean sprouts\",\"Shredded radish\"]",
                PriceRange = "~10",
                CollectStatus = "Collected",
                Latitude = 39.9042,
                Longitude = 116.4074,
                LocationName = "Beijing Old Noodle Shop",
                DiscoverDate = "2025-04-10",
                NoteText = "Authentic Beijing flavor, rich sauce aroma, firm and chewy noodles",
                CollectionName = "Noodle Universe",
                IsShowcase = false,
                CreatedAt = "2025-04-10 12:00:00",
                UpdatedAt = "2025-04-10 12:00:00"
            },
            new()
            {
                CatalogNumber = "FV-0008",
                Name = "Pita with Lamb Soup",
                Region = "Northwest",
                Rarity = "Recommended",
                ImagePath = "seed_yangrou_paomo.jpg",
                StarRating = 5,
                PrimaryTaste = "Umami",
                AromaTag = "Herbal",
                TextureTag = "Rich",
                Description = "Pita with Lamb Soup (Yangrou Paomo) is a traditional delicacy from Xi'an, Shaanxi, hailed as 'the best bowl in the world.' The baked flatbread is torn into small pieces by hand and added to lamb bone broth that has been simmered for hours, served with glass noodles, wood ear mushrooms, and more. The broth is rich, the meat is tender, the bread is smooth and chewy, with deep and complex flavors. The traditional way to eat is to nibble from the edge without stirring.",
                Ingredients = "[\"Lamb\",\"Baked flatbread\",\"Glass noodles\",\"Wood ear mushrooms\",\"Daylily buds\",\"Scallion and ginger\",\"Spices\"]",
                PriceRange = "10~30",
                CollectStatus = "Collected",
                Latitude = 34.2658,
                Longitude = 108.9541,
                LocationName = "Xi'an Lao Sun Jia Paomo",
                DiscoverDate = "2025-05-01",
                NoteText = "The process of tearing the bread is itself a pleasure, the broth is savory and rich, the bread absorbs the essence of the lamb",
                CollectionName = null,
                IsShowcase = false,
                CreatedAt = "2025-05-01 07:30:00",
                UpdatedAt = "2025-05-01 07:30:00"
            },
            new()
            {
                CatalogNumber = "FV-0009",
                Name = "Lanzhou Beef Noodles",
                Region = "Northwest",
                Rarity = "Common",
                ImagePath = "seed_yangrou_paomo.jpg",
                StarRating = 4,
                PrimaryTaste = "Umami",
                AromaTag = "None",
                TextureTag = "Firm",
                Description = "Lanzhou Beef Noodles, also known as Lanzhou Clear Broth Beef Noodles, is a traditional delicacy from Lanzhou, Gansu Province. It is famous for its unique flavor of 'clear broth, tender meat, fine noodles' and the 'five colors': clear broth, white radish, red chili oil, green garlic sprouts, and yellow noodles. The hand-pulled noodles are firm and smooth, and the beef broth is savory and rich.",
                Ingredients = "[\"High-gluten flour\",\"Beef\",\"Beef bones\",\"White radish\",\"Chili oil\",\"Garlic sprouts\",\"Cilantro\"]",
                PriceRange = "~10",
                CollectStatus = "Collected",
                Latitude = 36.0611,
                Longitude = 103.8343,
                LocationName = "Lanzhou Ma Zilu Beef Noodles",
                DiscoverDate = "2025-02-08",
                NoteText = "Five colors in one bowl - clear broth, white radish, red chili, green herbs, yellow noodles. The noodle-pulling master's skill is awe-inspiring",
                CollectionName = "Noodle Universe",
                IsShowcase = false,
                CreatedAt = "2025-02-08 08:00:00",
                UpdatedAt = "2025-02-08 08:00:00"
            },
            new()
            {
                CatalogNumber = "FV-0010",
                Name = "Tempura",
                Region = "Japan",
                Rarity = "Recommended",
                ImagePath = "seed_shuipenyangrou.jpg",
                StarRating = 4,
                PrimaryTaste = "Umami",
                AromaTag = "None",
                TextureTag = "Crispy",
                Description = "Tempura is one of the most representative deep-fried dishes in Japanese cuisine. Originating from Portugal, it was introduced to Japan via missionaries in the 16th century. Seafood and vegetables are coated with a thin batter and quickly fried in high-temperature oil. The finished product is crispy on the outside and tender on the inside, preserving the original umami of the ingredients. Served with tentsuyu dipping sauce or matcha salt, it offers rich layers of texture.",
                Ingredients = "[\"Prawns\",\"Pumpkin\",\"Sweet potato\",\"Eggplant\",\"Cake flour\",\"Egg\",\"Ice water\"]",
                PriceRange = "30~60",
                CollectStatus = "Collected",
                Latitude = 35.6762,
                Longitude = 139.6503,
                LocationName = "Tokyo Ginza Tempura Heritage Shop",
                DiscoverDate = "2025-04-15",
                NoteText = "Batter as thin as cicada wings, the umami of the ingredients perfectly locked in, wonderful when dipped in tentsuyu",
                CollectionName = null,
                IsShowcase = true,
                CreatedAt = "2025-04-15 19:00:00",
                UpdatedAt = "2025-04-15 19:00:00"
            },
            new()
            {
                CatalogNumber = "FV-0011",
                Name = "Unagi Rice Bowl",
                Region = "Japan",
                Rarity = "Limited",
                ImagePath = "seed_tianfuluo.jpg",
                StarRating = 5,
                PrimaryTaste = "Complex",
                AromaTag = "Smoky",
                TextureTag = "Tender",
                Description = "Unagi Rice Bowl (Unaju/Unadon) is a traditional premium Japanese dish. Charcoal-grilled eel is laid over steaming white rice and drizzled with a special sweet soy sauce. The eel is slowly charcoal-grilled until the outside is slightly charred and the inside is soft and tender, with a rich and sweet-savory sauce. In Japan, there is a custom of eating eel for nourishment on the Day of the Ox during the doyo period.",
                Ingredients = "[\"Japanese eel\",\"Soy sauce\",\"Mirin\",\"Sake\",\"Sugar\",\"Sansho pepper powder\",\"Rice\"]",
                PriceRange = "60~100",
                CollectStatus = "Collected",
                Latitude = 35.0116,
                Longitude = 135.7681,
                LocationName = "Kyoto Pontocho Eel Restaurant",
                DiscoverDate = "2025-04-18",
                NoteText = "The charcoal aroma and sweet-savory sauce blend perfectly, every bite is a delight",
                CollectionName = "Luxury Moments",
                IsShowcase = false,
                CreatedAt = "2025-04-18 20:30:00",
                UpdatedAt = "2025-04-18 20:30:00"
            },
            new()
            {
                CatalogNumber = "FV-0012",
                Name = "Korean Fried Chicken",
                Region = "Korea",
                Rarity = "Common",
                ImagePath = "seed_dongyingong.jpg",
                StarRating = 4,
                PrimaryTaste = "Salty",
                AromaTag = "Garlic",
                TextureTag = "Crispy",
                Description = "Korean Fried Chicken is one of the most popular national dishes in South Korea. Unlike American fried chicken, Korean fried chicken is typically double-fried for an extra crispy exterior, then coated with various flavored sauces. Classic flavors include original, sweet and spicy, garlic soy sauce, and honey butter. Paired with beer, it is known as 'chimaek' (chicken + maekju), a unique Korean dining culture.",
                Ingredients = "[\"Chicken\",\"Minced garlic\",\"Soy sauce\",\"Chili sauce\",\"Honey\",\"Starch\",\"Flour\"]",
                PriceRange = "10~30",
                CollectStatus = "Tried",
                Latitude = 37.5665,
                Longitude = 126.9780,
                LocationName = "Seoul Hongdae Fried Chicken Street",
                DiscoverDate = "2025-03-05",
                NoteText = "Double frying makes the skin super crispy, sweet and spicy sauce is my favorite, absolutely amazing with beer",
                CollectionName = "Street Legends",
                IsShowcase = false,
                CreatedAt = "2025-03-05 21:00:00",
                UpdatedAt = "2025-03-05 21:00:00"
            },
            new()
            {
                CatalogNumber = "FV-0013",
                Name = "Tom Yum Soup",
                Region = "SoutheastAsia",
                Rarity = "Recommended",
                ImagePath = "seed_dongyingong.jpg",
                StarRating = 5,
                PrimaryTaste = "Sour",
                AromaTag = "Herbal",
                TextureTag = "Refreshing",
                Description = "Tom Yum Goong is Thailand's national soup. 'Tom Yum' means sour and spicy shrimp soup. Made with fresh shrimp as the main ingredient, simmered with lemongrass, galangal, kaffir lime leaves, chili, and other spices. The soup is bright orange-red, sour, spicy, savory, and aromatic with rich layers. The coconut milk version is even more creamy and smooth. One of Thailand's most representative dishes.",
                Ingredients = "[\"Prawns\",\"Lemongrass\",\"Galangal\",\"Kaffir lime leaves\",\"Chili\",\"Lime juice\",\"Fish sauce\",\"Coconut milk\"]",
                PriceRange = "30~60",
                CollectStatus = "Collected",
                Latitude = 13.7563,
                Longitude = 100.5018,
                LocationName = "Bangkok Street Stall",
                DiscoverDate = "2025-04-22",
                NoteText = "Perfect balance of sour, spicy, and savory, the aroma of lemongrass and kaffir lime leaves is irresistible",
                CollectionName = "Spicy Lovers",
                IsShowcase = true,
                CreatedAt = "2025-04-22 13:00:00",
                UpdatedAt = "2025-04-22 13:00:00"
            },
            new()
            {
                CatalogNumber = "FV-0014",
                Name = "Vietnamese Pho",
                Region = "SoutheastAsia",
                Rarity = "Common",
                ImagePath = "seed_manyu_fan.jpg",
                StarRating = 4,
                PrimaryTaste = "Umami",
                AromaTag = "Herbal",
                TextureTag = "Refreshing",
                Description = "Vietnamese Pho is the national dish of Vietnam, a traditional dish made with rice noodles served in beef bone broth. The broth is simmered for hours with beef bones and various spices (star anise, cinnamon, cloves, black cardamom, etc.), resulting in a clear and savory base. When served, fresh Thai basil, lime, chili, and bean sprouts are added, making it refreshing yet rich in flavor.",
                Ingredients = "[\"Beef\",\"Rice noodles\",\"Beef bones\",\"Star anise\",\"Cinnamon\",\"Thai basil\",\"Lime\",\"Bean sprouts\"]",
                PriceRange = "10~30",
                CollectStatus = "Collected",
                Latitude = 21.0285,
                Longitude = 105.8542,
                LocationName = "Hanoi Old Quarter Pho Shop",
                DiscoverDate = "2025-03-20",
                NoteText = "Clear and savory broth, paired with fresh herbs, every bite is full of layers",
                CollectionName = "Noodle Universe",
                IsShowcase = false,
                CreatedAt = "2025-03-20 08:30:00",
                UpdatedAt = "2025-03-20 08:30:00"
            },
            new()
            {
                CatalogNumber = "FV-0015",
                Name = "Beef Wellington",
                Region = "Western",
                Rarity = "Premium",
                ImagePath = "seed_huiguo_rou.jpg",
                StarRating = 5,
                PrimaryTaste = "Complex",
                AromaTag = "Creamy",
                TextureTag = "Crispy",
                Description = "Beef Wellington is a classic premium British dish. At its core is a whole filet mignon steak, wrapped in mushroom duxelles and Parma ham, with the outermost layer covered by golden crispy puff pastry. After baking, it is crispy on the outside and tender on the inside, with distinct layers when cut, offering a rich texture. This dish demands exceptional culinary skill and is a classic in Western cuisine.",
                Ingredients = "[\"Filet mignon\",\"Puff pastry\",\"Mushrooms\",\"Parma ham\",\"Dijon mustard\",\"Egg\",\"Butter\"]",
                PriceRange = "100+",
                CollectStatus = "Collected",
                Latitude = 51.5074,
                Longitude = -0.1278,
                LocationName = "London The Grill",
                DiscoverDate = "2025-05-15",
                NoteText = "Golden crispy puff pastry, tender and juicy steak, mushroom duxelles adds layers - a collectible dining experience",
                CollectionName = "Luxury Moments",
                IsShowcase = true,
                CreatedAt = "2025-05-15 20:00:00",
                UpdatedAt = "2025-05-15 20:00:00"
            },
            new()
            {
                CatalogNumber = "FV-0016",
                Name = "Hot and Sour Vermicelli",
                Region = "Sichuan",
                Rarity = "Common",
                ImagePath = "seed_yangrou_paomo.jpg",
                StarRating = 4,
                PrimaryTaste = "Spicy",
                AromaTag = "Garlic",
                TextureTag = "Firm",
                Description = "Hot and Sour Vermicelli is a traditional famous snack from Sichuan, Chongqing, and surrounding areas. Made with sweet potato vermicelli as the main ingredient, seasoned with vinegar, chili oil, Sichuan peppercorn, crushed peanuts, cilantro, and more. It is appetizing with its sour and spicy kick, and the vermicelli is firm and bouncy, making it one of the most popular street snacks. Authentic Hot and Sour Vermicelli emphasizes the perfect balance of 'numbing, spicy, savory, aromatic, and sour' flavors.",
                Ingredients = "[\"Sweet potato vermicelli\",\"Chili oil\",\"Vinegar\",\"Sichuan peppercorn powder\",\"Crushed peanuts\",\"Cilantro\",\"Garlic water\",\"Soy sauce\"]",
                PriceRange = "~10",
                CollectStatus = "Collected",
                Latitude = 29.5630,
                Longitude = 106.5516,
                LocationName = "Chongqing Jiefangbei Hot and Sour Vermicelli Stall",
                DiscoverDate = "2025-02-25",
                NoteText = "Street stall Hot and Sour Vermicelli is the most authentic, satisfyingly sour and spicy, the vermicelli is bouncy and chewy",
                CollectionName = "Spicy Lovers",
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
                Name = "Spicy Lovers",
                Theme = "Spicy Challenge",
                Description = "Challenge various spicy delicacies",
                ColorTag = "Red",
                SortOrder = 0,
                CreatedAt = "2025-01-01 10:00:00",
                UpdatedAt = "2025-01-01 10:00:00"
            },
            new()
            {
                Name = "Noodle Universe",
                Theme = "Noodles From Around",
                Description = "Collect noodles from all regions",
                ColorTag = "Yellow",
                SortOrder = 1,
                CreatedAt = "2025-01-01 10:05:00",
                UpdatedAt = "2025-01-01 10:05:00"
            },
            new()
            {
                Name = "Street Legends",
                Theme = "Street Food",
                Description = "The most down-to-earth flavors",
                ColorTag = "Green",
                SortOrder = 2,
                CreatedAt = "2025-01-01 10:10:00",
                UpdatedAt = "2025-01-01 10:10:00"
            },
            new()
            {
                Name = "Luxury Moments",
                Theme = "Premium Experience",
                Description = "Treasured delicacies for special occasions",
                ColorTag = "Purple",
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
                Name = "Kuanzhai Alley",
                Category = "FoodStreet",
                Address = "Qingyang District, Chengdu",
                Latitude = 30.6697,
                Longitude = 104.0555,
                Region = "Sichuan",
                Feature = "Sichuan snacks hub",
                StarRating = 5,
                CreatedAt = "2025-01-01 10:00:00"
            },
            new()
            {
                Name = "Shangxiajiu Pedestrian Street",
                Category = "Shopping",
                Address = "Liwan District, Guangzhou",
                Latitude = 23.1181,
                Longitude = 113.2442,
                Region = "Cantonese",
                Feature = "Cantonese dim sum street",
                StarRating = 4,
                CreatedAt = "2025-01-01 10:05:00"
            },
            new()
            {
                Name = "City God Temple",
                Category = "Landmark",
                Address = "Huangpu District, Shanghai",
                Latitude = 31.2275,
                Longitude = 121.4920,
                Region = "Jiangnan",
                Feature = "Birthplace of Nanxiang Xiaolongbao",
                StarRating = 5,
                CreatedAt = "2025-01-01 10:10:00"
            },
            new()
            {
                Name = "Guijie (Ghost Street)",
                Category = "FoodStreet",
                Address = "Dongcheng District, Beijing",
                Latitude = 39.9403,
                Longitude = 116.4274,
                Region = "Northern",
                Feature = "Late-night food paradise",
                StarRating = 4,
                CreatedAt = "2025-01-01 10:15:00"
            },
            new()
            {
                Name = "Muslim Quarter (Huimin Street)",
                Category = "Heritage",
                Address = "Beilin District, Xi'an",
                Latitude = 34.2583,
                Longitude = 108.9425,
                Region = "Northwest",
                Feature = "Northwest flavors street",
                StarRating = 5,
                CreatedAt = "2025-01-01 10:20:00"
            },
            new()
            {
                Name = "Tsukiji Market",
                Category = "Landmark",
                Address = "Chuo Ward, Tokyo",
                Latitude = 35.6654,
                Longitude = 139.7707,
                Region = "Japan",
                Feature = "Seafood tempura and sushi",
                StarRating = 5,
                CreatedAt = "2025-01-01 10:25:00"
            },
            new()
            {
                Name = "Myeongdong Food Street",
                Category = "NightMarket",
                Address = "Jung District, Seoul",
                Latitude = 37.5636,
                Longitude = 126.9834,
                Region = "Korea",
                Feature = "Korean fried chicken and BBQ",
                StarRating = 4,
                CreatedAt = "2025-01-01 10:30:00"
            },
            new()
            {
                Name = "Khao San Road",
                Category = "NightMarket",
                Address = "Banglapphao, Bangkok",
                Latitude = 13.7589,
                Longitude = 100.4974,
                Region = "SoutheastAsia",
                Feature = "Thai street food",
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
