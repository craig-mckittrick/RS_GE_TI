using System.Text.Json;
using HtmlAgilityPack;

namespace RS_GE_TI
{
  class API_Caller
  {
    public static async Task<List<Item>> Item_call_API(char userLetter)
    {
      //first ask for a category
      int option = Menus.CategoryMenu();
      var itemLibrary = new Dictionary<int, string>();

      //TODO loop multiple page numbers as required
      int pageNumber = 1;
      using HttpClient client = new HttpClient();

      string url = $"https://secure.runescape.com/m=itemdb_rs/api/catalogue/items.json?category={option}&alpha={userLetter}&page={pageNumber}";

      // Cancellation token to stop the loading loop
      using var cts = new CancellationTokenSource();

      // Start the loading loop in a background task
      var loadingTask = Task.Run(async () =>
      {
        while (!cts.Token.IsCancellationRequested)
        {

          Console.Write("Loading..\r"); // overwrite the same line
          await Task.Delay(500);        // wait half a second
        }
      });

      string json = await client.GetStringAsync(url);
      // Stop the loading loop
      cts.Cancel();
      await loadingTask; // ensure loop task finishes
      Console.WriteLine($"Done with page 1.");

      //get number of items % by 12
      using JsonDocument doc = JsonDocument.Parse(json);
      int total = doc.RootElement.GetProperty("total").GetInt32();
      int totalPages = (int)Math.Ceiling(total / 12.0);
      var itemsList = new List<Item>();

      // Append first page
      var itemsElement = doc.RootElement.GetProperty("items");
      var items = JsonSerializer.Deserialize<List<Item>>(itemsElement.GetRawText());
      itemsList.AddRange(items!);

      // Step 2: Loop remaining pages
      int page = 2;
      for (page = 2; page <= totalPages; page++)
      {
        string pageUrl = $"https://secure.runescape.com/m=itemdb_rs/api/catalogue/items.json?category={option}&alpha={userLetter}&page={page}";

        // Cancellation token to stop the loading loop
        var cts1 = new CancellationTokenSource();

        // Start the loading loop in a background task
        loadingTask = Task.Run(async () =>
       {
         while (!cts1.Token.IsCancellationRequested)
         {

           Console.Write("Loading..\r"); // overwrite the same line
           await Task.Delay(500);        // wait half a second
         }
       });

        string pageJson = await client.GetStringAsync(pageUrl);

        // Stop the loading loop
        cts1.Cancel();
        await loadingTask; // ensure loop task finishes
        Console.WriteLine($"Done with page {page} of {totalPages}.");
        using JsonDocument pageDoc = JsonDocument.Parse(pageJson);
        var pageItemsElement = pageDoc.RootElement.GetProperty("items");
        var pageItems = JsonSerializer.Deserialize<List<Item>>(pageItemsElement.GetRawText());
        itemsList.AddRange(pageItems!);
      }

      // Step 3: Return combined list
      return itemsList;
    }

    public static async Task PrintTop100Trades()
    {
      string url = "https://secure.runescape.com/m=itemdb_rs/top100?list=0&scale=180";
      int columns = 3;
      int colWidth = 35;
      using var client = new HttpClient();
      string html = await client.GetStringAsync(url);

      var doc = new HtmlDocument();
      doc.LoadHtml(html);

      // The items are inside table rows <tr> with <td> cells
      var rows = doc.DocumentNode.SelectNodes("//table//tr");

      if (rows == null)
      {
        Console.WriteLine("No trades found.");
        return;
      }

      Console.WriteLine("Top 100 Grand Exchange Trades:\n");

      var items = rows
          .Skip(1) // skip header row
          .Select(tr =>
          {
            var cells = tr.SelectNodes("td");
            if (cells == null || cells.Count < 2) return null;
            string name = cells[0].InnerText.Trim();
            string value = cells[1].InnerText.Trim();
            return $"{name}";
          })
          .Where(x => x != null)
          .ToList();

      for (int i = 0; i < items.Count; i++)
      {
        string entry = $"{i + 1,2}. {items[i]}";
        Console.Write(entry.PadRight(colWidth));

        if ((i + 1) % columns == 0)
          Console.WriteLine();
      }

      if (items.Count % columns != 0)
        Console.WriteLine();
    }
  }
}