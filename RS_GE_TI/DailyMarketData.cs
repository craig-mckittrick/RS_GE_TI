using Newtonsoft.Json;

namespace RS_GE_TI
{
  public static class DailyMarketData
  {

    private static readonly HttpClient client = new HttpClient();

    public class PricePoint
    {
      public DateTime Date { get; set; }
      public int Price { get; set; }
    }
    public class ItemData
    {
      // Properties
      public int Id { get; set; }
      public string Name { get; set; }

      public List<PricePoint> DailyHistory { get; set; } = new List<PricePoint>();
    }

    // Phase 2: Step 1 - Daily price scrub
    public static async Task RefreshWinnersPrices(string filePath)
    {
      var winners = FileJSONConverter.LoadWinnersFromJson(filePath);
      if (winners.Count == 0)
      {
        Console.WriteLine("No winners found in JSON.");
        return;
      }

      int total = winners.Count;
      int processed = 0;

      foreach (var item in winners)
      {
        var updatedItem = await FetchItemGraph(item.Id);
        if (updatedItem.DailyHistory.Count == 0)
        {
          Console.WriteLine($"[{processed + 1}/{total}] Skipped {item.Name} — no price data.");
          continue;
        }

        item.DailyHistory = updatedItem.DailyHistory;
        processed++;
        Console.WriteLine($"[{processed}/{total}] Updated {item.Name} with {item.DailyHistory.Count} prices.");
      }

      FileJSONConverter.SaveWinnersToJson(winners, filePath);
      Console.WriteLine($"Update complete.");
    }

    // Helper for phase 2 step 1
    private static async Task<ItemData> FetchItemGraph(int itemId)
    {
      var itemData = new ItemData { Id = itemId };
      string url = $"https://secure.runescape.com/m=itemdb_rs/api/graph/{itemId}.json";

      try
      {
        string json = await client.GetStringAsync(url);
        dynamic data = JsonConvert.DeserializeObject(json);

        if (data?.daily != null)
        {
          foreach (var kvp in data.daily)
          {
            string keyString = kvp.Name ?? kvp.Key?.ToString(); // depending on type
            if (long.TryParse(keyString, out long timeStamp))
            {
              DateTime date = DateTimeOffset.FromUnixTimeMilliseconds(timeStamp).UtcDateTime;
              int price = (int)kvp.Value;
              itemData.DailyHistory.Add(new PricePoint { Date = date, Price = price });
            }
          }
        }
      }
      catch
      {
        // ignore errors
      }
      return itemData;
    }

    //Helper for Phase 2: Step 2 - Daily log of indicators
    public static void RunIndicators(int sMAI, int lMAI)
    {
      var winners = FileJSONConverter.LoadWinnersFromJson("winners.json");

      foreach (var item in winners)
      {
        var prices = item.DailyHistory.Select(p => p.Price).ToList();
        var sMA = MovingAverage(prices, sMAI);
        var lMA = MovingAverage(prices, lMAI);

        // Use the latest price (most recent day)
        int price = prices.LastOrDefault();

        string signal;
        if (price > lMA && price > sMA)
          signal = "BUY";
        else if (price > lMA && price < sMA)
          signal = "WATCH";
        else if (price < sMA)
          signal = "SELL";
        else
          signal = "HOLD";

        Console.ForegroundColor = signal switch
        {
          "BUY" => ConsoleColor.Green,
          "SELL" => ConsoleColor.Red,
          "WATCH" => ConsoleColor.Yellow,
          _ => ConsoleColor.Gray
        };
        Console.WriteLine($"{item.Name,-25} SMA ({Program.sMAThreshold})={sMA:F2} " +
                            $"LMA ({Program.lMAThreshold}) ={lMA:F2} Price={price} Signal={signal}");
        Console.ResetColor();

      }
    }

    // helper phase 2 step 2
    public static double MovingAverage(List<int> prices, int window)
    {
      if (prices.Count < window) return 0;
      return prices.Skip(prices.Count - window).Average();
    }
  }
}
