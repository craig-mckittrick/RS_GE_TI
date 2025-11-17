using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;

class TextConverterProgram
{
    static void TextConverterMain()
    {
        // Read all lines from the text file
        var lines = File.ReadAllLines("Top100GELibrary.txt");

        // Convert to a list of objects
        var items = new List<Item>();
        foreach (var line in lines)
        {
            items.Add(new Item { Name = line.Trim() });
        }

        // Serialize to JSON
        var options = new JsonSerializerOptions { WriteIndented = true };
        string json = JsonSerializer.Serialize(items, options);

        // Save to file
        File.WriteAllText("Top100GELibrary.json", json);

        Console.WriteLine("Conversion complete! JSON saved to items.json");
    }
}

public class Item
{
    public string Name { get; set; }
}
