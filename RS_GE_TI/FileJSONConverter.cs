using Newtonsoft.Json;

namespace RS_GE_TI
{
  public static class FileJSONConverter
  {
    // Save winners to JSON file
    public static void SaveWinnersToJson(List<DailyMarketData.ItemData> winners, string filePath)
    {
      var json = JsonConvert.SerializeObject(winners, Formatting.Indented);
      File.WriteAllText(filePath, json);
    }

    // Load winners from JSON file
    public static List<DailyMarketData.ItemData> LoadWinnersFromJson(string filePath)
    {
      if (!File.Exists(filePath)) return new List<DailyMarketData.ItemData>();

      var json = File.ReadAllText(filePath);
      return JsonConvert.DeserializeObject<List<DailyMarketData.ItemData>>(json) ?? new List<DailyMarketData.ItemData>();
    }
  }
}