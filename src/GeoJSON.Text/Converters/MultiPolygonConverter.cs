using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Azure.Cosmos.Spatial;
using System.Collections.Generic;

namespace GeoJSON.Text.Converters;

/// <summary>
/// Converts <see cref="MultiPolygon"/> objects to and from JSON for System.Text.Json.
/// </summary>
public class MultiPolygonConverter : JsonConverter<MultiPolygon>
{
    public override MultiPolygon Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException("Expected StartObject token");

        IList<PolygonCoordinates> polygons = null;
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
                    polygons = JsonSerializer.Deserialize<IList<PolygonCoordinates>>(ref reader, options);
                }
            }
        }

        if (polygons == null)
            throw new JsonException("Missing 'coordinates' property for MultiPolygon");

        return new MultiPolygon(polygons, geometryParams);
    }

    public override void Write(Utf8JsonWriter writer, MultiPolygon value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("coordinates");
        JsonSerializer.Serialize(writer, value.Polygons, options);
        writer.WriteString("type", "MultiPolygon");
        writer.WriteEndObject();
    }
}
