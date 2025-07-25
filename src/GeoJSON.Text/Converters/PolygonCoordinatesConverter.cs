using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Azure.Cosmos.Spatial;

namespace GeoJSON.Text.Converters;

/// <summary>
/// System.Text.Json converter for PolygonCoordinates.
/// </summary>
public class PolygonCoordinatesConverter : JsonConverter<PolygonCoordinates>
{
    public override PolygonCoordinates Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Expecting array of arrays of positions
        var rings = JsonSerializer.Deserialize<List<List<Position>>>(ref reader, options);
        if (rings == null)
            throw new JsonException("Missing or invalid coordinates for PolygonCoordinates");
        return new PolygonCoordinates(rings.Select(r => new LinearRing(r)).ToList());
    }

    public override void Write(Utf8JsonWriter writer, PolygonCoordinates value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        foreach (var ring in value.Rings)
        {
            JsonSerializer.Serialize(writer, ring.Positions, options);
        }
        writer.WriteEndArray();
    }
}
