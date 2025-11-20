using HtmlAgilityPack;
using System.Text.Json;


namespace RS_GE_TI
{
    class Program
    {
        public static char itemName;
        public static int itemId;
        public static string[] menuOptions = new string[] { "Browse items by first letter",
                                                             "Lookup item value",
                                                             "List recommended buys",
                                                             "List recommended sells",
                                                             "Quit" };
        public List<Item>? itemsList;

        static async Task Main()
        {
            await MainMenu();
        }

        public static async Task BrowseByLetter()
        {
            //first ask for a category
            CategoryMenu();

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
                    ConfirmMenuChoice($"You entered the letter: '{c}'.");
                    var itemsList = await Item_call_API(c);

                    // output list of items
                    if (itemsList == null || itemsList.Count == 0)
                    {
                        Console.WriteLine("No items found for that letter.");
                        return;
                    }
                    foreach (var item in itemsList)
                    {
                        Console.WriteLine($"{item.Name} - Current Price: {item.Current?.Price}");
                    }
                    return;
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a single letter a-z. [type 'esc' to quit]");
                }
            }
        }

        public static async Task<List<Item>> Item_call_API(char userLetter)
        {
            var itemLibrary = new Dictionary<int, string>();

            //TODO loop multiple page numbers as required
            int pageNumber = 1;
            using HttpClient client = new HttpClient();

            string url = $"https://secure.runescape.com/m=itemdb_rs/api/catalogue/items.json?category=1&alpha={userLetter}&page={pageNumber}";
            string json = await client.GetStringAsync(url);

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
            for (int page = 2; page <= totalPages; page++)
            {
                string pageUrl = $"https://secure.runescape.com/m=itemdb_rs/api/catalogue/items.json?category=1&alpha={userLetter}&page={page}";
                string pageJson = await client.GetStringAsync(pageUrl);
                using JsonDocument pageDoc = JsonDocument.Parse(pageJson);
                var pageItemsElement = pageDoc.RootElement.GetProperty("items");
                var pageItems = JsonSerializer.Deserialize<List<Item>>(pageItemsElement.GetRawText());
                itemsList.AddRange(pageItems!);
            }

            // Step 3: Return combined list
            return itemsList;
        }

        private static async Task MainMenu()
        {
            Console.WriteLine();
            Console.WriteLine("Welcome to the Runescape Grand Exchange Technical Indicator!");
            int option = PrintMenu();
            switch (option)
            {
                //Browse items by first letter
                case 1:
                    await BrowseByLetter();
                    break;
                // Lookup item value
                case 2:

                    break;
                // List recommended buys
                case 3:

                    break;
                // List recommended sells
                case 4:

                    break;
                // Quit
                case 5:
                    break;
                default:
                    break;
            }
        }

        private static int PrintMenu()
        {

            Console.WriteLine(new String('*', 40));
            Console.WriteLine($"{"* What would you like to do today?",-39}*");
            int i = 1;
            foreach (string menuOption in menuOptions)
            {
                Console.WriteLine($"{$"* {i}. {menuOption}",-39}*");
                i++;
            }
            Console.WriteLine(new String('*', 40));
            while (true)
            {
                string input = Console.ReadLine() ?? string.Empty;

                if (int.TryParse(input, out int output))
                {
                    if (output > i - 1 || output < 1)
                    {
                        Console.WriteLine("Invalid input. Try again.");
                        PrintMenu();
                    }
                    //confirm
                    ConfirmMenuChoice($"You selected '{output}. {menuOptions[output]}'.");
                    return output;
                }
                else
                {
                    Console.WriteLine("Invalid input. Try again.");
                    PrintMenu();
                }
            }
        }

        //create a category menu
        static void CategoryMenu()
        {
            // user input for letter
            Console.WriteLine("Please choose a category: [type 'esc' to quit]");
            string[] categories = new string[]
                {
            "Ammo", "Arrows", "Bolts", "Construction materials", "Cooking ingredients",
            "Crafting materials", "Farming produce", "Fletching materials", "Food & drink", "Herblore ingredients",
            "Hunting equipment", "Jewellery", "Mage armour", "Mage weapons", "Melee armour",
            "Melee weapons", "Miscellaneous", "Potions", "Prayer items", "Range armour",
            "Range weapons", "Runecrafting", "Seeds", "Summoning", "Tools",
            "Woodcutting", "Mining & Smithing", "Divination", "Dungeoneering", "Invention",
            "Treasure Hunter items", "Skilling supplies", "Slayer", "Necromancy", "Archaeology",
            "Farming animals", "Hunter creatures", "Miscellaneous (catch‑all)"
                };

            int columns = 4; // how many columns you want
            int colWidth = 25; // spacing for each column

            Console.WriteLine("Grand Exchange Categories:\n");

            for (int i = 0; i < categories.Length; i++)
            {
                string entry = $"{i + 1}. {categories[i]}";
                Console.Write(entry.PadRight(colWidth));

                // wrap to next line after 'columns' entries
                if ((i + 1) % columns == 0)
                    Console.WriteLine();
            }

            // ensure final line break
            Console.WriteLine();
            while (true)
            {
                
            }
        }
        private static void ConfirmMenuChoice(string prompt)
        {
            System.Console.Write($"{prompt} Confirm [y/n] ");
            while (true)
            {
                string inputString = Console.ReadLine() ?? string.Empty;
                if (inputString.StartsWith('y'))
                {
                    return;
                }
                else if (inputString.StartsWith('n'))
                {
                    PrintMenu();
                }
                else
                {
                    System.Console.WriteLine("Invalid input. Please try again.");
                    PrintMenu();
                }
            }
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

        public static async Task GetTop10Trades()
        {
            string url = "https://secure.runescape.com/m=itemdb_rs/top100?list=0";
            using HttpClient client = new HttpClient();
            string html = await client.GetStringAsync(url);

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            // save file
            //File.WriteAllText("top100.html", html);
            var rows = doc.DocumentNode.SelectNodes("//table/tbody/tr");

            if (rows != null)
            {
                for (int i = 0; i < Math.Min(10, rows.Count); i++)
                {
                    var itemCell = rows[i].SelectSingleNode("td/a/span");
                    if (itemCell != null)
                    {
                        string itemName = itemCell.InnerText.Trim();
                        Console.WriteLine($"{i + 1}. {itemName}");
                    }
                }
            }
            else
            {
                Console.WriteLine("No item rows found — page may be JavaScript-rendered.");
            }
        }
    }
}

