// Copyright © Joerg Battermann 2014, Matt Hunt 2017

//using GeoJSON.Text.Geometry;
using System;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GeoJSON.Text.Converters;

/// <summary>
/// Converts <see cref="Geometry"/> types to and from JSON.
/// </summary>
public class BoundingBoxConverter : JsonConverter<double[]>
{
    /// <inheritdoc/>
    public override bool CanConvert(Type objectType)
    {
        return typeof(double[]).IsAssignableFromType(objectType);
    }

    /// <inheritdoc/>
    public override double[] Read(
        ref Utf8JsonReader reader,
        Type type,
        JsonSerializerOptions options)
    {
        return JsonSerializer.Deserialize<double[]>(ref reader, options);
    }

    /// <inheritdoc/>
    public override void Write(
        Utf8JsonWriter writer,
        double[] value,
        JsonSerializerOptions options)
    {
        // Standard serialization
        JsonSerializer.Serialize(writer, value, typeof(double[]), options);
    }
}