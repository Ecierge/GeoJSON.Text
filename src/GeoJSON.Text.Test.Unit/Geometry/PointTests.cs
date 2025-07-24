using Microsoft.Azure.Cosmos.Spatial;

using NUnit.Framework;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GeoJSON.Text.Tests.Geometry;

[TestFixture]
public class PointTests : TestBase
{
    [Test]
    public void Can_Serialize_With_Lat_Lon()
    {
        var point = new Point(new Position(90.65464646, 53.2455662)); // longitude, latitude
        var expectedJson = "{\"coordinates\":[90.65464646,53.2455662],\"type\":\"Point\"}";
        var actualJson = JsonSerializer.Serialize(point, DefaultJsonSerializerOptions);
        JsonAssert.AreEqual(expectedJson, actualJson);
    }

    [Test]
    public void Can_Serialize_With_Lat_Lon_Alt()
    {
        var point = new Point(new Position(90.65464646, 53.2455662, 200.4567)); // longitude, latitude, altitude
        var expectedJson = "{\"coordinates\":[90.65464646,53.2455662,200.4567],\"type\":\"Point\"}";
        var actualJson = JsonSerializer.Serialize(point, DefaultJsonSerializerOptions);
        JsonAssert.AreEqual(expectedJson, actualJson);
    }

    [Test]
    public void Can_Deserialize_With_Lat_Lon_Alt()
    {
        var json = "{\"coordinates\":[90.65464646,53.2455662,200.4567],\"type\":\"Point\"}";
        var expectedPoint = new Point(new Position(90.65464646, 53.2455662, 200.4567));
        var actualPoint = JsonSerializer.Deserialize<Point>(json, DefaultJsonSerializerOptions);
        Assert.IsNotNull(actualPoint);
        Assert.IsNotNull(actualPoint.Position);
        Assert.AreEqual(expectedPoint.Position.Latitude, actualPoint.Position.Latitude); // latitude
        Assert.AreEqual(expectedPoint.Position.Longitude, actualPoint.Position.Longitude); // longitude
        Assert.AreEqual(expectedPoint.Position.Altitude, actualPoint.Position.Altitude);
    }

    [Test]
    public void Can_Deserialize_With_Lat_Lon()
    {
        var json = "{\"coordinates\":[90.65464646,53.2455662],\"type\":\"Point\"}";
        var expectedPoint = new Point(new Position(90.65464646, 53.2455662));
        var actualPoint = JsonSerializer.Deserialize<Point>(json, DefaultJsonSerializerOptions);
        Assert.IsNotNull(actualPoint);
        Assert.IsNotNull(actualPoint.Position);
        Assert.AreEqual(expectedPoint.Position.Latitude, actualPoint.Position.Latitude); // latitude
        Assert.AreEqual(expectedPoint.Position.Longitude, actualPoint.Position.Longitude); // longitude
        Assert.IsFalse(actualPoint.Position.Altitude.HasValue);
        Assert.IsNull(actualPoint.Position.Altitude);
        Assert.AreEqual(expectedPoint, actualPoint);
    }

    [Test]
    public void Equals_GetHashCode_Contract()
    {
        var json = "{\"coordinates\":[90.65464646,53.2455662],\"type\":\"Point\"}";
        var expectedPoint = new Point(new Position(90.65464646, 53.2455662));
        var actualPoint = JsonSerializer.Deserialize<Point>(json, DefaultJsonSerializerOptions);
        Assert.AreEqual(expectedPoint, actualPoint);
        Assert.IsTrue(expectedPoint.Equals(actualPoint));
        Assert.IsTrue(actualPoint.Equals(expectedPoint));
        Assert.AreEqual(expectedPoint.GetHashCode(), actualPoint.GetHashCode());
    }

    [Test]
    public void Can_Serialize_With_Lat_Lon_Alt_DefaultValueHandling_Ignore()
    {
        var point = new Point(new Position(53.2455662, 90.65464646, 200.4567));
        var expectedJson = "{\"coordinates\":[90.65464646,53.2455662,200.4567],\"type\":\"Point\"}";
        var options = new JsonSerializerOptions(DefaultJsonSerializerOptions)
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
        };
        var actualJson = JsonSerializer.Serialize(point, options);
        JsonAssert.AreEqual(expectedJson, actualJson);
    }
}