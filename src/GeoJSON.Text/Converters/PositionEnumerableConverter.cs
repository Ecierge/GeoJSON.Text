// Copyright © Joerg Battermann 2014, Matt Hunt 2017

using Microsoft.Azure.Cosmos.Spatial;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GeoJSON.Text.Converters;

/// <summary>
/// Converter to read and write the <see cref="IReadOnlyCollection{IPosition}" /> type.
/// </summary>
public class PositionEnumerableConverter : JsonConverter<IList<Position>>
{
    private static readonly PositionConverter PositionConverter = new();

    /// <inheritdoc/>
    public override bool CanConvert(Type objectType)
    {
        return typeof(IReadOnlyCollection<Position>).IsAssignableFromType(objectType);
    }

    /// <inheritdoc/>
    public override IList<Position> Read(
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
        var result = new List<Position>();
        while (reader.Read())
        {
            if (JsonTokenType.EndArray == reader.TokenType && reader.CurrentDepth == startDepth)
            {
                return result;
            }
            if (reader.TokenType == JsonTokenType.StartArray)
            {
                result.Add(PositionConverter.Read(
                        ref reader,
                        typeof(Position),
                        options));
            }
        }

        throw new JsonException($"expected null, object or array token but received {reader.TokenType}");
    }

    /// <inheritdoc/>
    public override void Write(
        Utf8JsonWriter writer,
        IList<Position> coordinateElements,
        JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        foreach (var position in coordinateElements)
        {
            PositionConverter.Write(writer, position, options);
        }
        writer.WriteEndArray();
    }
}