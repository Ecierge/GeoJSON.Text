// Copyright © Joerg Battermann 2014, Matt Hunt 2017

using Microsoft.Azure.Cosmos.Spatial;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GeoJSON.Text.Converters;

/// <summary>
///     Converter to read and write an <see cref="Position" />, that is,
///     the coordinates of a <see cref="Point" />.
/// </summary>
public class PositionConverter : JsonConverter<Position>
{
    /// <inheritdoc/>
    public override Position Read(
           ref Utf8JsonReader reader,
           Type objectType,
           JsonSerializerOptions options)
    {
        double[] coordinates = JsonSerializer.Deserialize<double[]>(ref reader, options);

        if (coordinates == null || coordinates.Length < 2)
        {
            throw new ArgumentException("Expected 2 coordinates type of double");
        }
        return new Position(coordinates);
    }

    /// <inheritdoc/>
    public override void Write(
        Utf8JsonWriter writer,
        Position coordinates,
        JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, coordinates.Coordinates, options);
    }
}