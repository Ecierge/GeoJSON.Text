// Copyright © Joerg Battermann 2014, Matt Hunt 2017

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

using Microsoft.Azure.Cosmos.Spatial;

namespace GeoJSON.Text.Converters;

/// <summary>
/// Converter to read and write the <see cref="IEnumerable{MultiPolygon}" /> type.
/// </summary>
public class PolygonEnumerableConverter : JsonConverter<IList<PolygonCoordinates>>
{
    private static readonly LinearRingEnumerableConverter PolygonConverter = new LinearRingEnumerableConverter();

    /// <inheritdoc/>
    public override bool CanConvert(Type objectType)
    {
        return objectType.IsAssignableFrom(typeof(IList<PolygonCoordinates>));
    }

    /// <inheritdoc/>
    public override IList<PolygonCoordinates> Read(
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
        var result = new List<PolygonCoordinates>();
        while (reader.Read())
        {
            if (JsonTokenType.EndArray == reader.TokenType && reader.CurrentDepth == startDepth)
            {
                return result;
            }
            if (reader.TokenType == JsonTokenType.StartArray)
            {
                result.Add(new PolygonCoordinates(PolygonConverter.Read(
                    ref reader,
                    typeof(IEnumerable<LineString>),
                    options)));
            }
        }

        throw new JsonException($"expected null, object or array token but received {reader.TokenType}");
    }

    /// <inheritdoc/>
    public override void Write(
        Utf8JsonWriter writer,
        IList<PolygonCoordinates> value,
        JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        foreach (var polygon in value)
        {
            PolygonConverter.Write(writer, polygon.Rings, options);
        }
        writer.WriteEndArray();
    }
}