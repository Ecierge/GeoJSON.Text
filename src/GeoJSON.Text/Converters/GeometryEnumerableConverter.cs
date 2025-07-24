// Copyright © Joerg Battermann 2014, Matt Hunt 2017

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.Json.Serialization;

using Microsoft.Azure.Cosmos.Spatial;

namespace GeoJSON.Text.Converters;

/// <summary>
/// Converts <see cref="Geometry"/> types to and from JSON.
/// </summary>
public class GeometryEnumerableConverter : JsonConverter<IList<Geometry>>
{
    private static readonly GeometryConverter GeometryConverter = new GeometryConverter();

    /// <inheritdoc/>
    public override bool CanConvert(Type objectType)
    {
        return typeof(IList<Geometry>).IsAssignableFromType(objectType);
    }

    /// <inheritdoc/>
    public override IList<Geometry> Read(
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
        }

        var startDepth = reader.CurrentDepth;
        var result = new List<Geometry>();
        while (reader.Read())
        {
            if (JsonTokenType.EndArray == reader.TokenType && reader.CurrentDepth == startDepth)
            {
                return result;
            }
            if (reader.TokenType == JsonTokenType.StartObject)
            {
                result.Add((Geometry)GeometryConverter.Read(
                    ref reader,
                    typeof(IEnumerable<Position>),
                    options));
            }
        }

        throw new JsonException($"expected null, object or array token but received {reader.TokenType}");
    }

    /// <inheritdoc/>
    public override void Write(
        Utf8JsonWriter writer,
        IList<Geometry> value,
        JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        foreach(var item in value)
        {
            GeometryConverter.Write(writer, item, options);
        }
        writer.WriteEndArray();
    }
}