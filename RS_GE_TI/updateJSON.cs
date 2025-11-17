using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;

class JSONUpdaterProgram
{
    static void JSONUpdaterMain()
    {
        // Read all lines from the text file
        var HTML = File.ReadAllLines("top100.html");
        Console.WriteLine("I successfully grabbed the HTML file.");

    }
}