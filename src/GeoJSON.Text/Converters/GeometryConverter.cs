// Copyright © Joerg Battermann 2014, Matt Hunt 2017

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.Json.Serialization;

using Microsoft.Azure.Cosmos.Spatial;

namespace GeoJSON.Text.Converters
{
    /// <summary>
    /// Converts <see cref="Geometry"/> types to and from JSON.
    /// </summary>
    public class GeometryConverter : JsonConverter<Geometry>
    {
        /// <summary>
        ///     Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>
        ///     <c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanConvert(Type objectType)
        {
            return typeof(Geometry).IsAssignableFromType(objectType);
        }

        /// <summary>
        ///     Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>
        ///     The object value.
        /// </returns>
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

        /// <summary>
        /// Reads the geo json.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        /// <exception cref="Newtonsoft.Json.JsonReaderException">
        /// json must contain a "type" property
        /// or
        /// type must be a valid geojson geometry object type
        /// </exception>
        /// <exception cref="System.NotSupportedException">
        /// Feature and FeatureCollection types are Feature objects and not Geometry objects
        /// </exception>
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

                case GeometryType.Point:
                    return value.Deserialize<Point>(options);
                case GeometryType.MultiPoint:
                    return value.Deserialize<MultiPoint>(options);
                case GeometryType.LineString:
                    return value.Deserialize<LineString>(options);
                case GeometryType.MultiLineString:
                    return value.Deserialize<MultiLineString>(options);
                case GeometryType.Polygon:
                    return value.Deserialize<Polygon>(options);
                case GeometryType.MultiPolygon:
                    return value.Deserialize<MultiPolygon>(options);
                case GeometryType.GeometryCollection:
                    return value.Deserialize<GeometryCollection>(options);
                case GeometryType.Feature:
                case GeometryType.FeatureCollection:
                default:
                    throw new NotSupportedException("Feature and FeatureCollection types are Feature objects and not Geometry objects");
            }
        }

        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Text.Json.Utf8JsonWriter" /> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void Write(
            Utf8JsonWriter writer,
            Geometry value,
            JsonSerializerOptions options)
        {
            // Standard serialization
            switch (value.Type)
            {
                case GeometryType.Point:
                    JsonSerializer.Serialize<Point>(writer, (Point)value);
                    break;
                case GeometryType.MultiPoint:
                    JsonSerializer.Serialize<MultiPoint>(writer, (MultiPoint)value);
                    break;
                case GeometryType.LineString:
                    JsonSerializer.Serialize<LineString>(writer, (LineString)value);
                    break;
                case GeometryType.MultiLineString:
                    JsonSerializer.Serialize<MultiLineString>(writer, (MultiLineString)value);
                    break;
                case GeometryType.Polygon:
                    JsonSerializer.Serialize<Polygon>(writer, (Polygon)value);
                    break;
                case GeometryType.MultiPolygon:
                    JsonSerializer.Serialize<MultiPolygon>(writer, (MultiPolygon)value);
                    break;
                case GeometryType.GeometryCollection:
                    JsonSerializer.Serialize<GeometryCollection>(writer, (GeometryCollection)value);
                    break;
                default:
                    throw new NotSupportedException("Feature and FeatureCollection types are Feature objects and not Geometry objects");
            }
        }

    }
}