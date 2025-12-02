// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Net.Http;
// using System.Threading.Tasks;
// using System.IO;
// using Newtonsoft.Json;

// namespace RS_GE_TI
// {
//   public static class MarketDataService
//   {
//     private static readonly HttpClient client = new HttpClient();

//     // Phase 1: Get volatility winners (Every 6 months)
//     public static async Task<List<ItemData>> GetVolatilityWinners(double thresholdPercent)
//     {
//       var winners = new List<ItemData>();

//       // 1. Fetch Top 100 most traded items (list=0, scale=180)
//       var topItems = await FetchTop100Items("https://secure.runescape.com/m=itemdb_rs/top100?list=0&scale=180");

//       //counter
//       int total = topItems.Count;
//       int processed = 0;

//       foreach (var item in topItems)
//       {
//         // fetch + filter

//         // 2. Fetch 6-month price history
//         var prices = await FetchItemGraph(item.Id);
//         if (Prices.Count == 0) continue;


//         int max30 = int.MinValue;
//         int min30 = int.MaxValue;

//         // Make sure we have at least 30 prices
//         int limit = Math.Min(30, prices.Count);

//         for (int i = prices.Count - 30; i < prices.Count; i++)
//         {
//           int price = prices[i];

//           if (price > max30)
//             max30 = price;

//           if (price < min30)
//             min30 = price;
//         }
//         double diffPercent = (double)(max30 - min30) / max30 * 100;
//         processed++;
//         Console.WriteLine($"[{processed}/{total}] Processing {item.Name}...{diffPercent:F2}");

//         // 3. Apply volatility filter
//         if (diffPercent >= thresholdPercent)
//         {
//           winners.Add(new ItemData
//           {
//             Id = item.Id,
//             Name = item.Name,
//             Prices = prices
//           });
//         }
//       }

//       return winners;
//     }

    

//     //Helper for Phase 2: Step 2 - Daily log of indicators
//     public static void RunIndicators(int sMAI, int lMAI)
//     {
//       var winners = LoadWinnersFromJson("winners.json");

//       foreach (var item in winners)
//       {
//         var sMA = MovingAverage(item.Prices, sMAI);
//         var lMA = MovingAverage(item.Prices, lMAI);

//         // Use the latest price (most recent day)
//         int price = item.Prices.LastOrDefault();

//         string signal;
//         if (price > lMA && price > sMA)
//           signal = "BUY";
//         else if (price > lMA && price < sMA)
//           signal = "WATCH";
//         else if (price < sMA)
//           signal = "SELL";
//         else
//           signal = "HOLD";

//         // Console.ForegroundColor = signal switch
//         // {
//         //   "BUY" => ConsoleColor.Green,
//         //   "SELL" => ConsoleColor.Red,
//         //   "WATCH" => ConsoleColor.Yellow,
//         //   _ => ConsoleColor.Gray
//         // };
//         // Console.WriteLine($"{item.Name,-25} SMA ({Program.sMAThreshold})={sMA:F2} " +
//         //                     $"LMA ({Program.lMAThreshold}) ={lMA:F2} Price={price} Signal={signal}");
//         // Console.ResetColor();

//       }
//     }

//     // Helper: Fetch item graph JSON (daily prices)
//     private static async Task<List<(DateTime Date, int Price)>> FetchItemGraph(int itemId)
//     {
//       var history = new List<(DateTime, int)>();
//       string url = $"https://secure.runescape.com/m=itemdb_rs/api/graph/{itemId}.json";

//       try
//       {
//         string json = await client.GetStringAsync(url);
//         dynamic data = JsonConvert.DeserializeObject(json);

//         if (data?.daily != null)
//         {
//           foreach (var kvp in data.daily)
//           {
//             long timestampMs = long.Parse(kvp.Key);
//             DateTime date = DateTimeOffset.FromUnixTimeMilliseconds(timestampMs).UtcDateTime;
//             int price = (int)kvp.Value;

//             history.Add((date, price));
//           }
//         }
//       }
//       catch
//       {
//         // ignore errors
//       }
//       return history;
//     }

//     // Helper: Fetch Top 100 items (IDs + names)
//     private static async Task<List<ItemData>> FetchTop100Items(string url)
//     {
//       var items = new List<ItemData>();
//       string html = await client.GetStringAsync(url);

//       var doc = new HtmlAgilityPack.HtmlDocument();
//       doc.LoadHtml(html);

//       var rows = doc.DocumentNode.SelectNodes("//table//tr");
//       if (rows == null) return items;

//       foreach (var tr in rows.Skip(1)) // skip header
//       {
//         var cells = tr.SelectNodes("td");
//         if (cells == null || cells.Count < 1) continue;

//         string name = cells[0].InnerText.Trim();

//         // Extract itemId from <a href="...obj=1234">
//         var linkNode = cells[0].SelectSingleNode(".//a");
//         int id = 0;
//         if (linkNode != null)
//         {
//           var href = linkNode.GetAttributeValue("href", "");
//           var parts = href.Split("obj=");
//           if (parts.Length > 1) int.TryParse(parts[1], out id);
//         }

//         if (id > 0)
//         {
//           items.Add(new ItemData { Id = id, Name = name });
//         }
//       }
//       return items;
//     }

//     public static void BacktestIndicatorsToCsv(
//                                                 string filePath,
//                                                 string outputCsv,
//                                                 int shortMA = 20,
//                                                 int longMA = 120,
//                                                 double stopLossPct = -5.0,   // stop-loss threshold (%)
//                                                 double takeProfitPct = 10.0  // take-profit threshold (%)
//                                             )
//     {
//       var winners = LoadWinnersFromJson(filePath);

//       using (var writer = new StreamWriter(outputCsv))
//       {
//         // Header row
//         writer.WriteLine("Item,Day,Signal,Price,EntryPrice,EntryDay,PctChange");

//         foreach (var item in winners)
//         {
//           var prices = item.Prices;
//           int start = Math.Max(shortMA, longMA);
//           if (prices.Count < start) continue;

//           bool inTrade = false;
//           int entryPrice = 0, entryDay = 0;

//           for (int day = start; day < prices.Count; day++)
//           {
//             var window = prices.Take(day + 1).ToList();
//             double sMA = MovingAverage(window, shortMA);
//             double lMA = MovingAverage(window, longMA);
//             int price = prices[day];

//             // Previous day MAs for slope check
//             double prevSMA = MovingAverage(window.Take(window.Count - 1).ToList(), shortMA);
//             double prevLMA = MovingAverage(window.Take(window.Count - 1).ToList(), longMA);

//             // Signal logic with slope + crossover
//             string signal = "HOLD";
//             if (!inTrade)
//             {
//               // BUY only if shortMA crosses above longMA AND slope is rising
//               if (prevSMA <= prevLMA && sMA > lMA && sMA > prevSMA)
//                 signal = "BUY";
//             }
//             else
//             {
//               // SELL if shortMA crosses below longMA AND slope is falling
//               if (prevSMA >= prevLMA && sMA < lMA && sMA < prevSMA)
//                 signal = "SELL";
//             }

//             // Trade entry
//             if (signal == "BUY" && !inTrade)
//             {
//               inTrade = true;
//               entryPrice = price;
//               entryDay = day;
//               writer.WriteLine($"{item.Name},{day},BUY,{price},{entryPrice},{entryDay},");
//             }
//             // Trade exit
//             else if (inTrade)
//             {
//               double pctChange = ((double)(price - entryPrice) / entryPrice) * 100.0;

//               // Stop-loss / take-profit check
//               if (pctChange <= stopLossPct || pctChange >= takeProfitPct)
//               {
//                 inTrade = false;
//                 writer.WriteLine($"{item.Name},{day},EXIT,{price},{entryPrice},{entryDay},{pctChange:F2}");
//               }
//               else if (signal == "SELL")
//               {
//                 inTrade = false;
//                 writer.WriteLine($"{item.Name},{day},SELL,{price},{entryPrice},{entryDay},{pctChange:F2}");
//               }
//             }
//           }
//         }
//       }

//       Console.WriteLine($"Backtest results saved to {outputCsv}");
//     }
//   }
// }
