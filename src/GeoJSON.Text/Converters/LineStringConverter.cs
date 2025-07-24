using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Azure.Cosmos.Spatial;
using System.Collections.Generic;

namespace GeoJSON.Text.Converters;

/// <summary>
/// Converts <see cref="LineString"/> objects to and from JSON for System.Text.Json.
/// </summary>
public class LineStringConverter : JsonConverter<LineString>
{
    public override LineString Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException("Expected StartObject token");

        IList<Position> positions = null;
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
                    positions = JsonSerializer.Deserialize<IList<Position>>(ref reader, options);
                }
            }
        }

        if (positions == null)
            throw new JsonException("Missing 'coordinates' property for LineString");

        return new LineString(positions, geometryParams);
    }

    public override void Write(Utf8JsonWriter writer, LineString value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteString("type", "LineString");
        writer.WritePropertyName("coordinates");
        JsonSerializer.Serialize(writer, value.Positions, options);
        writer.WriteEndObject();
    }
}
