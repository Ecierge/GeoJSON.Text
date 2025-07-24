// Copyright © Joerg Battermann 2014, Matt Hunt 2017

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Azure.Cosmos.Spatial;

namespace GeoJSON.Text.Converters;

/// <summary>
/// <see cref="JsonConverter"/> for <see cref="Crs" /> class and all its implementations.
/// </summary>
public sealed class CrsConverter : JsonConverter<Crs>
{
    public override bool HandleNull => true;

    /// <inheritdoc/>
    public override bool CanConvert(Type objectType)
    {
        return typeof(Crs).IsAssignableFromType(objectType);
    }

    /// <inheritdoc/>
    public override void Write(
        Utf8JsonWriter writer,
        Crs value,
        JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
            return;
        }
        switch (value.Type)
        {
            case CrsType.Linked:
                var linkedCrs = (LinkedCrs)value;
                writer.WriteStartObject();
                writer.WriteString("type", "link");
                writer.WritePropertyName("properties");
                writer.WriteStartObject();
                writer.WriteString("href", linkedCrs.Href);
                if (!string.IsNullOrEmpty(linkedCrs.HrefType))
                {
                    writer.WriteString("type", linkedCrs.HrefType);
                }
                writer.WriteEndObject();
                writer.WriteEndObject();
                break;
            case CrsType.Named:
                var namedCrs = (NamedCrs)value;
                writer.WriteStartObject();
                writer.WriteString("type", "name");
                writer.WritePropertyName("properties");
                writer.WriteStartObject();
                writer.WriteString("name", namedCrs.Name);
                writer.WriteEndObject();
                writer.WriteEndObject();
                break;
            case CrsType.Unspecified:
                writer.WriteNullValue();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    /// <inheritdoc/>
    public override Crs Read(
        ref Utf8JsonReader reader,
        Type type,
        JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return Crs.Unspecified;
        }
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException("CRS must be null or a json object");
        }
        var jObject = JsonDocument.ParseValue(ref reader).RootElement;
        if (!jObject.TryGetProperty("type", out var typeToken) || typeToken.ValueKind != JsonValueKind.String)
        {
            throw new JsonException("CRS must have a 'type' property");
        }
        if (!jObject.TryGetProperty("properties", out var properties) || properties.ValueKind != JsonValueKind.Object)
        {
            throw new JsonException("CRS must have a 'properties' object");
        }
        var crsType = typeToken.GetString();
        switch (crsType)
        {
            case "name":
                if (!properties.TryGetProperty("name", out var nameToken) || nameToken.ValueKind != JsonValueKind.String)
                {
                    throw new JsonException("CRS 'name' property must be a string");
                }
                return Crs.Named(nameToken.GetString());
            case "link":
                if (!properties.TryGetProperty("href", out var hrefToken) || hrefToken.ValueKind != JsonValueKind.String)
                {
                    throw new JsonException("CRS 'href' property must be a string");
                }
                string hrefType = null;
                if (properties.TryGetProperty("type", out var hrefTypeToken) && hrefTypeToken.ValueKind == JsonValueKind.String)
                {
                    hrefType = hrefTypeToken.GetString();
                }
                return Crs.Linked(hrefToken.GetString()); // Only pass href, as previous code did
            default:
                throw new JsonException($"CRS type '{crsType}' is not supported");
        }
    }
}
