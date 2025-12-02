using System.Text.Json;
using System.Text.Json.Serialization;

namespace RS_GE_TI;

public class Item
{
    [JsonPropertyName("icon")]
    public string Icon { get; set; }

    [JsonPropertyName("icon_large")]
    public string IconLarge { get; set; }

    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("typeIcon")]
    public string TypeIcon { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("current")]
    public Current Current { get; set; }

    [JsonPropertyName("today")]
    public Today Today { get; set; }

    [JsonPropertyName("members")]
    public string Members { get; set; }

    public DateTime GEDate { get; set; }
}


public class Current
{
    [JsonPropertyName("trend")]
    public string Trend { get; set; }

    [JsonPropertyName("price")]
    [JsonConverter(typeof(RuneScapePriceConverter))]
    public decimal Price { get; set; }
}

public class Today
{
    [JsonPropertyName("trend")]
    public string Trend { get; set; }

    [JsonPropertyName("price")]
    [JsonConverter(typeof(RuneScapePriceConverter))]
    public decimal Price { get; set; }
}

public class RuneScapePriceConverter : JsonConverter<decimal>
{
    public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
            return reader.GetDecimal();

        if (reader.TokenType == JsonTokenType.String)
        {
            string? s = reader.GetString()?.ToLower().Replace("current guide price", "").Trim();
            if (string.IsNullOrEmpty(s)) return 0;

            if (s.EndsWith("k"))
                return decimal.Parse(s[..^1]) * 1000;
            if (s.EndsWith("m"))
                return decimal.Parse(s[..^1]) * 1_000_000;

            return decimal.TryParse(s, out var value) ? value : 0;
        }

        throw new JsonException($"Unexpected token {reader.TokenType} when parsing price.");
    }

    public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value);
    }
}
