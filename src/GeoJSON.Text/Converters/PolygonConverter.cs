using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Azure.Cosmos.Spatial;
using System.Collections.Generic;

namespace GeoJSON.Text.Converters;

/// <summary>
/// Converts <see cref="Polygon"/> objects to and from JSON for System.Text.Json.
/// </summary>
public class PolygonConverter : JsonConverter<Polygon>
{
    public override Polygon Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException("Expected StartObject token");

        IList<LinearRing> rings = null;
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
                    rings = JsonSerializer.Deserialize<IList<LinearRing>>(ref reader, options);
                }
            }
        }

        if (rings == null)
            throw new JsonException("Missing 'coordinates' property for Polygon");

        return new Polygon(rings, geometryParams);
    }

    public override void Write(Utf8JsonWriter writer, Polygon value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteString("type", "Polygon");
        writer.WritePropertyName("coordinates");
        JsonSerializer.Serialize(writer, value.Rings, options);
        writer.WriteEndObject();
    }
}
