using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace GeoJSON.Text.Tests;

public abstract class TestBase
{
    protected static readonly JsonSerializerOptions DefaultJsonSerializerOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true,
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        Converters =
        {
            new Converters.CrsConverter(),
            new Converters.JsonStringEnumMemberConverter(),
            new Converters.GeometryConverter(),
            new Converters.GeometryEnumerableConverter(),
            new Converters.PointEnumerableConverter(),
            new Converters.LineStringConverter(),
            new Converters.LinearRingEnumerableConverter(),
            new Converters.PointConverter(),
            new Converters.PolygonConverter(),
            new Converters.PolygonEnumerableConverter(),
            new Converters.MultiPolygonConverter(),
            new Converters.PositionConverter(),
            new Converters.PositionEnumerableConverter(),
        }
    };

    protected string GetExpectedJson([CallerMemberName] string name = null)
    {
        var names = Assembly.GetExecutingAssembly().GetManifestResourceNames();

        var assembly = Assembly.GetExecutingAssembly();
        var type = GetType().FullName;
        using (Stream stream = assembly.GetManifestResourceStream($"{type}_{name}.json"))
        using (StreamReader reader = new StreamReader(stream))
        {
            string result = reader.ReadToEnd();
            return result;
        }

        throw new ArgumentException("File with name could not be found");
    }
}