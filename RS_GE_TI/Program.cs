using RS_GE_TI;

namespace RS_GE_TI
{
    class Program
    {
        public static char itemName;
        public static int itemId;
        public static int sMAThreshold = 7;
        public static int lMAThreshold = 120;
        public static double volatilityThreshold = .15;

        public List<Item>? itemsList;

        static async Task Main()
        {
            //await Menus.MainMenu();

            // Research technical indicator success rate
            //MarketDataService.BacktestIndicators("winners.json");

            // // Define the MA combos you want to test
            // int[] shortOptions = { 7, 14, 20 };
            // int[] longOptions = { 80, 120, 150 };

            // // Call SweepMACombos with stop-loss and take-profit thresholds
            // MarketDataService.SweepMACombos(
            //     "winners.json",
            //     shortOptions,
            //     longOptions,
            //     stopLossPct: -2.5,   // exit if trade drops 5%
            //     takeProfitPct: 5.0  // exit if trade gains 10%
            // );

            // Phase 1: Get volatility winners (Every 6 months)
            // await MarketDataService.GetVolatilityWinners(volatilityThreshold);

            // Phase 2: Step 1 - Daily price scrub
            // await DailyMarketData.RefreshWinnersPrices("winners.json");

            // Phase 2: Step 2 - Buy/Sell/Watch Indicators
            DailyMarketData.RunIndicators(sMAThreshold, lMAThreshold);

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

        //Phase 2: Step 2 - Daily log of buy indicators
        public static void ReportBuyIndicators()
        {

        }

        //Phase 2: Step 2 - Daily log of sell indicators
        public static void ReportSellIndicators()
        {

        }

        //Phase 2: Step 2 - Daily log of watch indicators
        public static void ReportWatchIndicators()
        {

        }
    }
}



