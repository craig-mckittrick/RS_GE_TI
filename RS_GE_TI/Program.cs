using HtmlAgilityPack;
using System.Text.Json;


namespace RS_GE_TI
{
    class Program
    {
        public static char itemName;
        public static int itemId;

        static async Task Main()
        {
            await MainMenu();
        }

        public static async Task BrowseByLetter()
        {
            // user input for letter
            Console.WriteLine("Enter the first letter of the item: ");
            while (true)
            {
                string userFirstLetter = Console.ReadLine() ?? string.Empty;
                if (char.TryParse(userFirstLetter, out char c) && char.IsLetter(c))
                {
                    Console.WriteLine($"You entered the letter: {c}");
                    await Item_call_API(c);
                    return;
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a single letter a-z.");
                }
            }

            //TODO
            //parse api data
            //string[] apiDataArrayed = new string[];

            //print organized data

        }

        public static async Task Item_call_API(char userLetter)
        {
            var itemLibrary = new Dictionary<int, string>();
            int pageNumber = 1;
            using HttpClient client = new HttpClient();

            string url = $"https://secure.runescape.com/m=itemdb_rs/api/catalogue/items.json?category=1&alpha={userLetter}&page={pageNumber}";
            string json = await client.GetStringAsync(url);

            using JsonDocument doc = JsonDocument.Parse(json);
            var items = doc.RootElement.GetProperty("items");
            Console.WriteLine($"The list of items is shown below:\n {items}");
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

        private static async Task MainMenu()
        {
            //TODO pass the true menu option string instead of hard code

            Console.WriteLine();
            Console.WriteLine("Welcome to the Runescape Grand Exchange Technical Indicator!");
            int option = PrintMenu();
            switch (option)
            {
                // Top 10 trades
                case 1:
                    Console.WriteLine($"You selected {option}. See top 10 trades");
                    await GetTop10Trades();
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
            string[] menuOptions = new string[] { "Browse items by first letter", "Lookup item value", "List recommended buys", "List recommended sells", "Quit" };

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
                    if (output > i-1 || output < 1 )
                    {
                        Console.WriteLine("Invalid input.");
                        PrintMenu();
                    }
                    return output;
                }
                else
                {
                    Console.WriteLine("Invalid input.");
                    PrintMenu();
                }

            }
        }

        private static void ClassBasics()
        {
            GE_Items a = new GE_Items();
            GE_Items b = new GE_Items();

            a.Name = "Maple logs";
            a.ItemNumber = 1517;
            a.Price = 319;
            a.IsMembers = false;

            b.Name = "Dragon hatchet";
            b.ItemNumber = 6739;
            b.Price = 36.8;
            b.IsMembers = true;

            System.Console.WriteLine($"My item: {a}");
            System.Console.WriteLine($"My item: {b}");

            System.Console.WriteLine($"My item: Name: {a.Name}, item number: {a.ItemNumber}, price: {a.Price}, is Member's: {a.IsMembers}");
            System.Console.WriteLine($"My item: Name: {b.Name}, item number: {b.ItemNumber}, price: {b.Price}, is Member's: {b.IsMembers}");

            System.Console.WriteLine($"My item equals your item? {a.Equals(b)}");
        }
    }
}

