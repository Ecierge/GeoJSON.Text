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
public class GeometryEnumerableConverter : JsonConverter<ReadOnlyCollection<Geometry>>
{
    private static readonly GeometryConverter GeometryConverter = new GeometryConverter();

    /// <summary>
    ///     Determines whether this instance can convert the specified object type.
    /// </summary>
    /// <param name="objectType">Type of the object.</param>
    /// <returns>
    ///     <c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
    /// </returns>
    public override bool CanConvert(Type objectType)
    {
        return typeof(ReadOnlyCollection<Geometry>).IsAssignableFromType(objectType);
    }

    /// <summary>
    ///     Reads the JSON representation of the object.
    /// </summary>
    /// <param name="reader">The <see cref="T:System.Text.Json.Utf8JsonReader" /> to read from.</param>
    /// <param name="objectType">Type of the object.</param>
    /// <param name="existingValue">The existing value of object being read.</param>
    /// <param name="serializer">The calling serializer.</param>
    /// <returns>
    ///     The object value.
    /// </returns>
    public override ReadOnlyCollection<Geometry> Read(
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
                return new ReadOnlyCollection<Geometry>(result);
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

    /// <summary>
    /// Writes the JSON representation of the object.
    /// </summary>
    /// <param name="writer">The <see cref="T:System.Text.Json.Utf8JsonWriter" /> to write to.</param>
    /// <param name="value">The value.</param>
    /// <param name="serializer">The calling serializer.</param>
    public override void Write(
        Utf8JsonWriter writer,
        ReadOnlyCollection<Geometry> value,
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