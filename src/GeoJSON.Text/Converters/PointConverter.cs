using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Azure.Cosmos.Spatial;

namespace GeoJSON.Text.Converters;

/// <summary>
/// Converts <see cref="Point"/> objects to and from JSON for System.Text.Json.
/// </summary>
public class PointConverter : JsonConverter<Point>
{
    public override Point Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException("Expected StartObject token");

        Position position = null;
        GeometryParams geometryParams = new GeometryParams();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                break;

            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                string propertyName = reader.GetString();
                reader.Read();
                if (propertyName == "coordinates")
                {
                    position = JsonSerializer.Deserialize<Position>(ref reader, options);
                }
                //// Ignore all other properties
                //else
                //{
                //    reader.Skip();
                //}
            }
        }

        if (position == null)
            throw new JsonException("Missing 'coordinates' property for Point");

        return new Point(position, geometryParams);
    }

    public override void Write(Utf8JsonWriter writer, Point value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteString("type", "Point");
        writer.WritePropertyName("coordinates");
        JsonSerializer.Serialize(writer, value.Position, options);
        writer.WriteEndObject();
    }
}
