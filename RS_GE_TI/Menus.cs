namespace RS_GE_TI
{
  public static class Menus
  {
    public static string[] mainMenuOptions = new string[] { "Browse items by first letter",
                                                             "List Top 100 Trades",
                                                             "List recommended buys",
                                                             "List recommended sells",
                                                             "Quit" };
    public static async Task MainMenu()
    {
      Console.WriteLine();
      Console.WriteLine("Welcome to the Runescape Grand Exchange Technical Indicator!");
      int option = PrintMenu();
      switch (option)
      {
        //Browse items by first letter
        case 1:
          await Program.BrowseByLetter();
          break;
        // Top 100 trades
        case 2:
          await API_Caller.PrintTop100Trades();
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

    public static int PrintMenu()
    {

      Console.WriteLine(new String('*', 40));
      Console.WriteLine($"{"* What would you like to do today?",-39}*");
      int i = 1;
      foreach (string menuOption in mainMenuOptions)
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
          ConfirmMenuChoice($"You selected '{output}. {mainMenuOptions[output-1]}'.");
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
    public static int CategoryMenu()
    {
      // user input for letter
      Console.WriteLine("Please choose a category: [type 'esc' to quit]");
      Dictionary<int, string> categories = new Dictionary<int, string>
      {
          {1, "Ammo"},
          {41, "Archaeology materials"},
          {2, "Arrows"},
          {3, "Bolts"},
          {4, "Construction materials"},
          {5, "Construction products"},
          {6, "Cooking ingredients"},
          {7, "Costumes"},
          {8, "Crafting materials"},
          {9, "Familiars"},
          {10, "Farming produce"},
          {40, "Firemaking products"},
          {11, "Fletching materials"},
          {12, "Food and Drink"},
          {13, "Herblore materials"},
          {14, "Hunting equipment"},
          {15, "Hunting Produce"},
          {16, "Jewellery"},
          {17, "Magic armour"},
          {18, "Magic weapons"},
          {21, "Melee armour - high level"},
          {19, "Melee armour - low level"},
          {20, "Melee armour - mid level"},
          {24, "Melee weapons - high level"},
          {22, "Melee weapons - low level"},
          {23, "Melee weapons - mid level"},
          {25, "Mining and Smithing"},
          {0, "Miscellaneous"},
          {42, "Miscellaneous"},
          {43, "Necromancy armour"},
          {37, "Pocket items"},
          {26, "Potions"},
          {27, "Prayer armour"},
          {28, "Prayer materials"},
          {29, "Ranged armour"},
          {30, "Ranged weapons"},
          {31, "Runecrafting"},
          {32, "Runes, Spells and Teleports"},
          {39, "Salvage"},
          {33, "Seeds"},
          {38, "Stone spirits"},
          {34, "Summoning scrolls"},
          {35, "Tools and containers"},
          {36, "Woodcutting product"}
      };

      int columns = 4; // how many columns you want
      int colWidth = 35; // spacing for each column

      Console.WriteLine("Grand Exchange Categories:\n");

      // Order by key so they print in numeric order
      var ordered = categories.OrderBy(kvp => kvp.Key).ToList();

      for (int i = 0; i < ordered.Count; i++)
      {
        var kvp = ordered[i];
        string entry = $"{kvp.Key,2}: {kvp.Value}";
        Console.Write(entry.PadRight(colWidth));

        // wrap to next line after 'columns' entries
        if ((i + 1) % columns == 0)
          Console.WriteLine();
      }

      // final line break if not already wrapped
      if (ordered.Count % columns != 0)
        Console.WriteLine();

      // input from user  
      while (true)
      {
        string prompt = "Please choose a category: ";
        System.Console.WriteLine(prompt);
        var input = System.Console.ReadLine();
        int i = categories.Count;
        if (int.TryParse(input, out int output))
        {
          if (output > i || output < 0)
          {
            Console.WriteLine("Invalid input. Try again.");
            CategoryMenu();
          }
          //confirm
          ConfirmMenuChoice($"You selected '{output}. {categories[output]}'.");
          return output;
        }
        else
        {
          Console.WriteLine("Invalid input. Try again.");
          CategoryMenu();
        }
      }
    }

    //TODO create input helpers
    public static int InputHelperInt(string prompt, int min, int max, bool confirm)
    {
      return 0;
    }

    public static string InputHelperString(string prompt, bool confirm)
    {
      return "1";
    }

    public static void ConfirmMenuChoice(string prompt)
    {
      System.Console.Write($"{prompt} Confirm [y/n] ");
      while (true)
      {
        string inputString = Console.ReadLine() ?? string.Empty;
        if (inputString == "esc")
        {
          break;
        }
        else if (inputString.StartsWith('y'))
        {
          return;
        }
        else if (inputString.StartsWith('n'))
        {
          return;
        }
        else
        {
          System.Console.WriteLine("Invalid input. Please try again. [type 'esc' to quit]");
          return;
        }
      }
    }

  }
}