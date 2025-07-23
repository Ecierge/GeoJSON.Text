using Microsoft.Azure.Cosmos.Spatial;

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GeoJSON.Text.Converters;

public class CrsRequiredPropertyConverter : JsonConverter<Crs>
{
    public override Crs Read(
        ref Utf8JsonReader reader,
        Type type,
        JsonSerializerOptions options)
    {
        // Don't pass in options when recursively calling Deserialize.
        var CrsClass = JsonSerializer.Deserialize<Crs>(ref reader);

        if (CrsClass.Type == default)
            throw new JsonException("Required property Type not set in the JSON");

        if (CrsClass.Properties == default)
            throw new JsonException("Required property Properties not set in the JSON");

        // Check for required fields set by values in JSON
        return CrsClass;
    }

    public override void Write(
        Utf8JsonWriter writer,
        Crs CrsClass,
        JsonSerializerOptions options)
    {
        // Don't pass in options when recursively calling Serialize.
        JsonSerializer.Serialize(writer, CrsClass);
    }
}