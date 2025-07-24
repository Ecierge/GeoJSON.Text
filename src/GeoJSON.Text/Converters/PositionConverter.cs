// Copyright © Joerg Battermann 2014, Matt Hunt 2017

using Microsoft.Azure.Cosmos.Spatial;
using System;
using System.Collections.Generic;
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
        Type type,
        JsonSerializerOptions options)
    {
        try
        {
            if (reader.TokenType != JsonTokenType.StartArray)
            {
                throw new ArgumentException("Expected start of array");
            }

            double lon, lat;
            double? alt;

            // Read longitude (GeoJSON)
            if (!reader.Read())
            {
                throw new ArgumentException("Expected number, but got end of data");
            }

            if (reader.TokenType == JsonTokenType.EndArray)
            {
                throw new ArgumentException("Expected 2 or 3 coordinates but got 0");
            }

            if (reader.TokenType == JsonTokenType.Number)
            {
                lon = reader.GetDouble();
            }
            else if (reader.TokenType == JsonTokenType.String)
            {
                lon = JsonSerializer.Deserialize<double>(ref reader, options);
            }
            else
            {
                throw new ArgumentException("Expected number but got other type");
            }

            // Read latitude (GeoJSON)
            if (!reader.Read())
            {
                throw new ArgumentException("Expected number, but got end of data");
            }

            if (reader.TokenType == JsonTokenType.EndArray)
            {
                throw new ArgumentException("Expected 2 or 3 coordinates but got 1");
            }

            if (reader.TokenType == JsonTokenType.Number)
            {
                lat = reader.GetDouble();
            }
            else if (reader.TokenType == JsonTokenType.String)
            {
                lat = JsonSerializer.Deserialize<double>(ref reader, options);
            }
            else
            {
                throw new ArgumentException("Expected number but got other type");
            }

            // Read altitude, or return if end of array is found
            if (!reader.Read())
            {
                throw new ArgumentException("Unexpected end of data");
            }
            if (reader.TokenType == JsonTokenType.EndArray)
            {
                return new Position(lat, lon); // Cosmos expects (lat, lon)
            }
            else if (reader.TokenType == JsonTokenType.Null)
            {
                alt = null;
            }
            else if (reader.TokenType == JsonTokenType.Number)
            {
                alt = reader.GetDouble();
            }
            else if (reader.TokenType == JsonTokenType.String)
            {
                alt = JsonSerializer.Deserialize<double>(ref reader, options);
            }
            else
            {
                throw new ArgumentException("Expected number but got other type");
            }

            // Check what comes next. Expects end of array.
            if (!reader.Read())
            {
                throw new ArgumentException("Expected end of array, but got end of data");
            }
            if (reader.TokenType != JsonTokenType.EndArray)
            {
                throw new ArgumentException("Expected 2 or 3 coordinates but got >= 4");
            }

            return new Position(lat, lon, alt); // Cosmos expects (lat, lon, alt)
        }
        catch (Exception e)
        {
            throw new JsonException("Error parsing coordinates", e);
        }
    }

    /// <inheritdoc/>
    public override void Write(
        Utf8JsonWriter writer,
        Position coordinates,
        JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        // GeoJSON order: [longitude, latitude, altitude]
        writer.WriteNumberValue(coordinates.Longitude);
        writer.WriteNumberValue(coordinates.Latitude);
        if (coordinates.Altitude.HasValue)
        {
            writer.WriteNumberValue(coordinates.Altitude.Value);
        }
        writer.WriteEndArray();
    }
}