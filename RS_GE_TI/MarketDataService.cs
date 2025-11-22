using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace RS_GE_TI
{
  public static class MarketDataService
  {
    public class ItemData
    {
      public int Id { get; set; }
      public string Name { get; set; }
      public List<int> Prices { get; set; } = new List<int>();
    }

    private static readonly HttpClient client = new HttpClient();

    // Phase 1: Get volatility winners
    public static async Task<List<ItemData>> GetVolatilityWinners(double thresholdPercent)
    {
      var winners = new List<ItemData>();

      // 1. Fetch Top 100 most traded items (list=0, scale=180)
      var topItems = await FetchTop100Items("https://secure.runescape.com/m=itemdb_rs/top100?list=0&scale=180");

      //counter
      int total = topItems.Count;
      int processed = 0;

      foreach (var item in topItems)
      {


        // fetch + filter

        // 2. Fetch 6-month price history
        var prices = await FetchItemGraph(item.Id);
        if (prices.Count == 0) continue;


        int max30 = int.MinValue;
        int min30 = int.MaxValue;

        // Make sure we have at least 30 prices
        int limit = Math.Min(30, prices.Count);

        for (int i = prices.Count - 30; i < prices.Count; i++)
        {
          int price = prices[i];

          if (price > max30)
            max30 = price;

          if (price < min30)
            min30 = price;
        }
        double diffPercent = (double)(max30 - min30) / max30 * 100;
        processed++;
        Console.WriteLine($"[{processed}/{total}] Processing {item.Name}...{diffPercent:F2}");
        // 3. Apply volatility filter
        if (diffPercent >= thresholdPercent)
        {
          winners.Add(new ItemData
          {
            Id = item.Id,
            Name = item.Name,
            Prices = prices
          });
        }
      }

      return winners;
    }

    // Helper: Fetch Top 100 items (IDs + names)
    private static async Task<List<ItemData>> FetchTop100Items(string url)
    {
      var items = new List<ItemData>();
      string html = await client.GetStringAsync(url);

      var doc = new HtmlAgilityPack.HtmlDocument();
      doc.LoadHtml(html);

      var rows = doc.DocumentNode.SelectNodes("//table//tr");
      if (rows == null) return items;

      foreach (var tr in rows.Skip(1)) // skip header
      {
        var cells = tr.SelectNodes("td");
        if (cells == null || cells.Count < 1) continue;

        string name = cells[0].InnerText.Trim();

        // Extract itemId from <a href="...obj=1234">
        var linkNode = cells[0].SelectSingleNode(".//a");
        int id = 0;
        if (linkNode != null)
        {
          var href = linkNode.GetAttributeValue("href", "");
          var parts = href.Split("obj=");
          if (parts.Length > 1) int.TryParse(parts[1], out id);
        }

        if (id > 0)
        {
          items.Add(new ItemData { Id = id, Name = name });
        }
      }

      return items;
    }

    // Helper: Fetch item graph JSON (daily prices)
    private static async Task<List<int>> FetchItemGraph(int itemId)
    {
      var prices = new List<int>();
      string url = $"https://secure.runescape.com/m=itemdb_rs/api/graph/{itemId}.json";

      try
      {
        string json = await client.GetStringAsync(url);
        dynamic data = JsonConvert.DeserializeObject(json);

        if (data?.daily != null)
        {
          foreach (var kvp in data.daily)
          {
            int price = (int)kvp.Value;
            prices.Add(price);
          }
        }
      }
      catch
      {
        // ignore errors for items without graph data
      }

      return prices;
    }

    public static void RunIndicators()
    {
      var winners = LoadWinnersFromJson("winners.json");

      foreach (var item in winners)
      {
        var ma20 = MovingAverage(item.Prices, 20);
        var ma50 = MovingAverage(item.Prices, 50);
        var ma120 = MovingAverage(item.Prices, 120);

        Console.WriteLine($"{item.Name,-25} MA20={ma20:F2} MA50={ma50:F2} MA120={ma120:F2}");
      }
    }

    public static double MovingAverage(List<int> prices, int window)
    {
      if (prices.Count < window) return 0;
      return prices.Skip(prices.Count - window).Average();
    }

    // Save winners to JSON file
    public static void SaveWinnersToJson(List<ItemData> winners, string filePath)
    {
      var json = JsonConvert.SerializeObject(winners, Formatting.Indented);
      File.WriteAllText(filePath, json);
    }

    // Load winners from JSON file
    public static List<ItemData> LoadWinnersFromJson(string filePath)
    {
      if (!File.Exists(filePath)) return new List<ItemData>();

      var json = File.ReadAllText(filePath);
      return JsonConvert.DeserializeObject<List<ItemData>>(json) ?? new List<ItemData>();
    }

    public static void SaveWinnersToCsvExpanded(List<ItemData> winners, string filePath)
    {
      using (var writer = new StreamWriter(filePath))
      {
        // Write header row: Id, Name, then Day1, Day2, ..., DayN
        int maxDays = winners.Max(w => w.Prices.Count);
        writer.Write("Id,Name");
        for (int d = 1; d <= maxDays; d++)
        {
          writer.Write($",Day{d}");
        }
        writer.WriteLine();

        // Write each item row
        foreach (var item in winners)
        {
          writer.Write($"{item.Id},{item.Name}");

          foreach (var price in item.Prices)
          {
            writer.Write($",{price}");
          }

          // Pad with blanks if this item has fewer days than maxDays
          int missing = maxDays - item.Prices.Count;
          for (int i = 0; i < missing; i++)
          {
            writer.Write(",");
          }

          writer.WriteLine();
        }
      }
    }


  }
}
