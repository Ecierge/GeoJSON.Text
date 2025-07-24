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
public class GeometryConverter : JsonConverter<Geometry>
{
    /// <inheritdoc/>
    public override Geometry Read(
        ref Utf8JsonReader reader,
        Type type,
        JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.Null:
                return null;
            case JsonTokenType.StartObject:
                return ReadGeoJson(ref reader, options);
        }

        throw new JsonException($"expected null, object or array token but received {reader.TokenType}");
    }

    /// <inheritdoc/>
    private static Geometry ReadGeoJson(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        var document = JsonDocument.ParseValue(ref reader);
        JsonElement value = document.RootElement;
        JsonElement token;

        if (!value.TryGetProperty("type", out token))
        {
            throw new JsonException("json must contain a \"type\" property");
        }

        GeometryType geoJsonType;

        if (!Enum.TryParse(token.GetString(), true, out geoJsonType))
        {
            throw new JsonException("type must be a valid geojson geometry object type");
        }

        switch (geoJsonType)
        {
            // https://github.com/Azure/azure-cosmos-dotnet-v3/issues/5312
            case GeometryType.Point:
                return value.Deserialize<Point>(options);
            //case GeometryType.MultiPoint:
            //    return value.Deserialize<MultiPoint>(options);
            case GeometryType.LineString:
                return value.Deserialize<LineString>(options);
            //case GeometryType.MultiLineString:
            //    return value.Deserialize<MultiLineString>(options);
            case GeometryType.Polygon:
                return value.Deserialize<Polygon>(options);
            case GeometryType.MultiPolygon:
                return value.Deserialize<MultiPolygon>(options);
            //case GeometryType.GeometryCollection:
            //    return value.Deserialize<GeometryCollection>(options);
            default:
                throw new NotSupportedException("Feature and FeatureCollection types are Feature objects and not Geometry objects");
        }
    }

    /// <inheritdoc/>
    public override void Write(
        Utf8JsonWriter writer,
        Geometry value,
        JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
}