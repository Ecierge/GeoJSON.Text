// Copyright � Joerg Battermann 2014, Matt Hunt 2017

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Azure.Cosmos.Spatial;

namespace GeoJSON.Text.Converters;

/// <summary>
/// Converter to read and write the <see cref="IEnumerable{Point}" /> type.
/// </summary>
public class PointEnumerableConverter : JsonConverter<ReadOnlyCollection<Point>>
{
    private static readonly PositionConverter PositionConverter = new PositionConverter();
    /// <inheritdoc />
    public override void Write(
        Utf8JsonWriter writer,
        ReadOnlyCollection<Point> value,
        JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        foreach (var point in value)
        {
            PositionConverter.Write(writer, point.Position, options);
        }
        writer.WriteEndArray();
    }

    /// <inheritdoc />
    public override ReadOnlyCollection<Point> Read(
        ref Utf8JsonReader reader,
        Type type,
        JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            // This converter has to read something starting with an array
            case JsonTokenType.StartArray:
                break;
            case JsonTokenType.Null:
                return null;
            default:
                throw new InvalidOperationException("Incorrect json type");
        }

        var startDepth = reader.CurrentDepth;
        var result = new List<Point>();
        List<double> numbers = new List<double>();
        while (reader.Read())
        {
            if (JsonTokenType.EndArray == reader.TokenType && reader.CurrentDepth == startDepth)
            {
                return new ReadOnlyCollection<Point>(result);
            }
            //if(JsonTokenType.EndArray == reader.TokenType)
            //{
            //    result.Add(new Point(numbers.ToPosition()));

            //    // We have finished reading this internal point array, clear so we can read next (If needed)
            //    numbers.Clear();
            //}
            if(reader.TokenType == JsonTokenType.Number)
            {  
                var position = new Position(numbers.ToArray());
                var point = ConvertToCosmosPosition(position);
                result.Add(point);
                numbers.Add(reader.GetDouble());
             }
        }

        throw new JsonException($"expected null, object or array token but received {reader.TokenType}");
    }

    /// <inheritdoc />
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(ReadOnlyCollection<Point>);
    }
    // Add a helper method to convert GeoJSON.Text.Geometry.Position to Microsoft.Azure.Cosmos.Spatial.Position
    private static Point ConvertToCosmosPosition(Position position)
    {
        return new Point(position.Longitude, position.Latitude);
    }
}