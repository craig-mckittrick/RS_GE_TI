
private static void ClassBasics()
{
    Item a = new Item();
    Item b = new Item();

    a.Name = "Maple logs";
    a.Id = 1517;
    a.Current.Price = 319;
    a.Members = "false";

    b.Name = "Dragon hatchet";
    b.Id = 6739;
    b.Current.Price = 36.8m;
    b.Members = "true";

    System.Console.WriteLine($"My item: {a}");
    System.Console.WriteLine($"My item: {b}");

    System.Console.WriteLine($"My item: Name: {a.Name}, item number: {a.Id}, price: {a.Current.Price}, is Member's: {a.Members}");
    System.Console.WriteLine($"My item: Name: {b.Name}, item number: {b.Id}, price: {b.Current.Price}, is Member's: {b.Members}");

    System.Console.WriteLine($"My item equals your item? {a.Equals(b)}");
}
# Steps to update "winners.json"
    // Phase 1: run the heavy fetch
    var winners = await StepOne.GetVolatilityWinners(15.0);

    // Explicitly save to file
    StepOne.SaveWinnersToJson(winners, "winners.json");

    Console.WriteLine("Winners saved to winners.json");

    // Phase 2: run indicators
    StepOne.RunIndicators();

# Logic for indicator
    if (price > longMA && price > shortMA)
        signal = "BUY";
    else if (price > longMA && price < shortMA)
        signal = "WATCH"; // setup, not confirmed
    else if (price < shortMA)
        signal = "SELL";
    else
        signal = "HOLD";

    // Daily fetch of prices for winners.json list of items
    public static async Task<List<ItemData>> FetchLatestPrices()
    {
      var prices = await FetchItemGraph(item.Id);
      return;
    }