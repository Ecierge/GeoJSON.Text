using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Azure.Cosmos.Spatial;

namespace GeoJSON.Text.Converters;

/// <summary>
/// System.Text.Json converter for LinearRing.
/// </summary>
public class LinearRingCoordinatesConverter : JsonConverter<LinearRing>
{
    public override LinearRing Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var positions = JsonSerializer.Deserialize<List<Position>>(ref reader, options);
        if (positions == null)
            throw new JsonException("Missing or invalid positions for LinearRing");
        return new LinearRing(positions);
    }

    public override void Write(Utf8JsonWriter writer, LinearRing value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value.Positions, options);
    }
}
