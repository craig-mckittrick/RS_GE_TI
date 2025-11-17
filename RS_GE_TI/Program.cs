using System;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Text.Json;
using System.IO;

namespace RS_GE_TI
{
    class Program
    {
        public static char itemName;
        public static int itemId;
        static async Task Main()
        {
            Console.WriteLine();
            Console.WriteLine("Welcome to the Runescape Grand Exchange Technical Indicator!");
            var option = PrintMenu();
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
                case 5 or "" or null:
                    break;
            }
            /*
            Console.Write("Enter item number or starting letter [type 'esc' to quit]:  ");
            string input = Console.ReadLine()?.ToLower() ?? "";

            // input is a number
            if (int.TryParse(input, out itemId))
            {
                Console.WriteLine($"You entered item number: {itemId}");
                await Item_call_API();
                break;
            }
            //cancel operation
            else if (input.ToLower() == "esc")
            {
                break;
            }
            // input is a character
            else if (!string.IsNullOrEmpty(input) && char.IsLetter(input[0]))
            {
                itemName = input[0];
                Console.WriteLine($"You entered starting letter: {input[0]}");
                // Call method to fetch items by letter
                break;
            }
            // input is neither a number nor a character
            else
            {
                Console.WriteLine("Invalid input. Please enter a number or a single letter [type 'esc' to quit]:  ");
            }
            */
        }


        public static async Task Item_call_API()
        {
            var itemLibrary = new Dictionary<int, string>();
            using HttpClient client = new HttpClient();

            for (char alpha = 'a'; alpha <= 'b'; alpha++)
            {
                for (int page = 1; page <= 2; page++) // adjust page limit as needed
                {
                    string url = $"https://secure.runescape.com/m=itemdb_rs/api/catalogue/items.json?category=1&alpha={alpha}&page={page}";
                    string json = await client.GetStringAsync(url);

                    using JsonDocument doc = JsonDocument.Parse(json);
                    var items = doc.RootElement.GetProperty("items");
                }
            }
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

        private static int PrintMenu()
        {
            Console.WriteLine(new String('*', 40));
            Console.WriteLine($"{"* What would you like to do today?",-39}*");
            Console.WriteLine($"{"* 1. See top 10 trades",-39}*");
            Console.WriteLine($"{"* 2. Lookup item value",-39}*");
            Console.WriteLine($"{"* 3. List recommended buys",-39}*");
            Console.WriteLine($"{"* 4. List recommended sells",-39}*");
            Console.WriteLine($"{"* 5. Quit",-39}*");
            Console.WriteLine(new String('*', 40));
            string input = Console.ReadLine();
        }
    }
}

