// Copyright © Joerg Battermann 2014, Matt Hunt 2017

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Azure.Cosmos.Spatial;

namespace GeoJSON.Text.Converters;

/// <summary>
/// Converter to read and write the <see cref="IList{LinearRing}" /> type.
/// </summary>
public class LinearRingEnumerableConverter : JsonConverter<IList<LinearRing>>
{
    private static readonly PositionEnumerableConverter LineStringConverter = new PositionEnumerableConverter();

    /// <inheritdoc/>
    public override bool CanConvert(Type objectType)
    {
        return objectType.IsAssignableFrom(typeof(IList<LinearRing>));
    }

    /// <inheritdoc/>
    public override IList<LinearRing> Read(
        ref Utf8JsonReader reader,
        Type type,
        JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.Null:
                return null;
            case JsonTokenType.StartArray:
                break;
            default:
                throw new InvalidOperationException("Incorrect json type");
        }

        var startDepth = reader.CurrentDepth;
        var result = new List<LinearRing>();
        while (reader.Read())
        {
            if (JsonTokenType.EndArray == reader.TokenType && reader.CurrentDepth == startDepth)
            {
                return result;
            }
            if (reader.TokenType == JsonTokenType.StartArray)
            {
                result.Add(new LinearRing(LineStringConverter.Read(
                    ref reader,
                    typeof(IEnumerable<double>),
                    options)));
            }
        }

        throw new JsonException($"expected null, object or array token but received {reader.TokenType}");
    }

    /// <inheritdoc/>
    public override void Write(
        Utf8JsonWriter writer,
        IList<LinearRing> value,
        JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        foreach (var subPolygon in value)
        {
            LineStringConverter.Write(writer, subPolygon.Positions, options);
        }
        writer.WriteEndArray();
    }
}