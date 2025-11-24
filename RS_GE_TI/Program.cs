using RS_GE_TI;

namespace RS_GE_TI
{
    class Program
    {
        public static char itemName;
        public static int itemId;

        public List<Item>? itemsList;

        static async Task Main()
        {

            // Phase 2: run indicators
            //StepOne.RunIndicators();
            var winners = MarketDataService.LoadWinnersFromJson("winners.json");
            MarketDataService.SaveWinnersToCsvExpanded(winners, "winners.csv");

            //await Menus.MainMenu();
        }

        public static async Task BrowseByLetter()
        {
            // user input for letter
            Console.WriteLine("Enter the first letter of the item: [type 'esc' to quit]");
            while (true)
            {
                string userFirstLetter = Console.ReadLine() ?? string.Empty;
                if (userFirstLetter == "esc")
                {
                    break;
                }
                else if (char.TryParse(userFirstLetter, out char c) && char.IsLetter(c))
                {
                    Menus.ConfirmMenuChoice($"You entered the letter: '{c}'.");
                    var itemsList = await API_Caller.Item_call_API(c);

                    // output list of items
                    if (itemsList == null || itemsList.Count == 0)
                    {
                        Console.WriteLine("No items found for that letter.");
                        return;
                    }
                    foreach (var item in itemsList)
                    {
                        Console.WriteLine($"{item.Name} - Current Price: {item.Current?.Price:N0}");
                    }
                    return;
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a single letter a-z. [type 'esc' to quit]");
                }
            }
<<<<<<< HEAD

=======
>>>>>>> 8d1b8cc518d50c2761dab9c2e3e15d21f3ea6d8a
        }

        public static decimal ParsePrice(string price)
        {
            if (string.IsNullOrWhiteSpace(price)) return 0;

            price = price.ToLower().Replace("current guide price", "").Trim();

            if (price.EndsWith("k"))
                return decimal.Parse(price[..^1]) * 1000;
            if (price.EndsWith("m"))
                return decimal.Parse(price[..^1]) * 1_000_000;

            return decimal.TryParse(price, out var value) ? value : 0;
        }
    }
}



