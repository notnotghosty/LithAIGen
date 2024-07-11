using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

class Program
{
    static void Main(string[] args)
    {
        try
        {
            var items = LoadItems("items.txt");
            if (items == null || items.Count == 0)
            {
                throw new Exception("No items were loaded.");
            }

            var data = LoadData("data.json");

            var dailyItems = GenerateRandomItems(items, 6);
            var featuredItems = GenerateRandomItems(items, 2);

            var itemShopConfig = new ItemShopConfig
            {
                Daily1 = CreateItemEntry(dailyItems, 0),
                Daily2 = CreateItemEntry(dailyItems, 1),
                Daily3 = CreateItemEntry(dailyItems, 2),
                Daily4 = CreateItemEntry(dailyItems, 3),
                Daily5 = CreateItemEntry(dailyItems, 4),
                Daily6 = CreateItemEntry(dailyItems, 5),
                Featured1 = CreateItemEntry(featuredItems, 0),
                Featured2 = CreateItemEntry(featuredItems, 1)
            };

            ExportItemShopConfig(itemShopConfig, "catalog_config.json");

            Console.WriteLine("Item shop configuration has been exported successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }

        Console.WriteLine("Press Enter to close the program.");
        Console.ReadLine();
    }

    static List<Cosmetic> LoadItems(string fileName)
    {
        if (!File.Exists(fileName))
        {
            throw new FileNotFoundException($"File '{fileName}' not found.");
        }

        var items = new List<Cosmetic>();
        foreach (var line in File.ReadLines(fileName))
        {
            var parts = line.Split(new char[] { ':', ',' });
            if (parts.Length != 3)
            {
                throw new FormatException($"Line '{line}' is not in the correct format.");
            }

            if (!int.TryParse(parts[2], out int price))
            {
                throw new FormatException($"Price '{parts[2]}' is not a valid integer.");
            }

            items.Add(new Cosmetic(parts[0] + ":" + parts[1], price));
        }

        return items;
    }

    static List<Cosmetic> GenerateRandomItems(List<Cosmetic> items, int count)
    {
        var random = new Random();
        var selectedItems = new List<Cosmetic>();

        for (int i = 0; i < count; i++)
        {
            var index = random.Next(items.Count);
            selectedItems.Add(items[index]);
            items.RemoveAt(index); // Ensure no duplicates
        }

        return selectedItems;
    }

    static ItemEntry CreateItemEntry(List<Cosmetic> items, int index)
    {
        if (index < items.Count)
        {
            var item = items[index];
            return new ItemEntry
            {
                ItemGrants = new List<string> { item.Id },
                Price = item.Price
            };
        }
        return new ItemEntry
        {
            ItemGrants = new List<string> { "" },
            Price = 0
        };
    }

    static void ExportItemShopConfig(ItemShopConfig config, string fileName)
    {
        var json = JsonConvert.SerializeObject(config, Newtonsoft.Json.Formatting.Indented);
        File.WriteAllText(fileName, json);
    }

    static AIData LoadData(string fileName)
    {
        if (!File.Exists(fileName))
        {
            throw new FileNotFoundException($"Data file '{fileName}' not found.");
        }

        var dataJson = File.ReadAllText(fileName);
        return JsonConvert.DeserializeObject<AIData>(dataJson)
               ?? throw new Exception("Failed to load AI data.");
    }
}

class Cosmetic
{
    public string Id { get; }
    public int Price { get; }

    public Cosmetic(string id, int price)
    {
        Id = id;
        Price = price;
    }
}

class ItemShopConfig
{
    public string Comment => "BR Item Shop Config";
    public ItemEntry Daily1 { get; set; }
    public ItemEntry Daily2 { get; set; }
    public ItemEntry Daily3 { get; set; }
    public ItemEntry Daily4 { get; set; }
    public ItemEntry Daily5 { get; set; }
    public ItemEntry Daily6 { get; set; }
    public ItemEntry Featured1 { get; set; }
    public ItemEntry Featured2 { get; set; }
}

class ItemEntry
{
    public List<string> ItemGrants { get; set; }
    public int Price { get; set; }
}

class AIData
{
    public Dictionary<string, double> ItemWeights { get; set; }
    public Dictionary<string, double> FeedbackScores { get; set; }
    public List<ItemShop> ShopHistory { get; set; }
    public List<ItemShop> GoodShopsHistory { get; set; }
    public Dictionary<(string, string), int> ItemRelationships { get; set; }
}

class ItemShop
{
    public List<Cosmetic> DailyItems { get; set; }
    public List<Cosmetic> FeaturedItems { get; set; }
}
