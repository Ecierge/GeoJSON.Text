using System.Collections.Generic;
using System.Text.Json;

using Microsoft.Azure.Cosmos.Spatial;

using NUnit.Framework;

namespace GeoJSON.Text.Tests.Geometry;

[TestFixture]
public class MultiPolygonTests : TestBase
{
    [Test]
    public void Can_Deserialize()
    {
        var json = GetExpectedJson();
        var expectMultiPolygon = GetMultiPolygon();
        var actualMultiPolygon = JsonSerializer.Deserialize<MultiPolygon>(json, DefaultJsonSerializerOptions);
        Assert.AreEqual(expectMultiPolygon, actualMultiPolygon);
    }

    private MultiPolygon GetMultiPolygon(double offset = 0.0)
    {
        var multiPolygon = new MultiPolygon(new List<PolygonCoordinates>
        {
            new PolygonCoordinates(new List<LinearRing>
            {
                new LinearRing(new List<Position>
                {
                    new Position(-2.6797102391514338 + offset, 52.959676831105995 + offset),
                    new Position(-2.6769029474483279 + offset, 52.9608756693609 + offset),
                    new Position(-2.6079763270327119 + offset, 52.908449372833715 + offset),
                    new Position(-2.5815104708998668 + offset, 52.891287242948195 + offset),
                    new Position(-2.5851645010668989 + offset, 52.875476700983896 + offset),
                    new Position(-2.6050779098387191 + offset, 52.882954723868622 + offset),
                    new Position(-2.6373482332006359 + offset, 52.875255907042678 + offset),
                    new Position(-2.6932445076063951 + offset, 52.878791122091066 + offset),
                    new Position(-2.6931334629377890 + offset, 52.89564268523565 + offset),
                    new Position(-2.6548779332193022 + offset, 52.930592009390175 + offset),
                    new Position(-2.6797102391514338 + offset, 52.959676831105995 + offset)
                })
            }),
            new PolygonCoordinates(new List<LinearRing>
            {
                new LinearRing(new List<Position>
                {
                    new Position(-2.69628632041613 + offset, 52.89610842810761 + offset),
                    new Position(-2.75901233808515 + offset,52.8894641454077 + offset),
                    new Position(-2.7663172788742449 + offset, 52.89938894657412 + offset),
                    new Position(-2.804554822840895 + offset, 52.90253773227807 + offset),
                    new Position(-2.83848602260174 + offset, 52.929801009654575 + offset),
                    new Position(-2.838979264607087 + offset, 52.94013913205788 + offset),
                    new Position(-2.7978187468478741 + offset, 52.937353122653533 + offset),
                    new Position(-2.772273870352612 + offset, 52.920394929466184 + offset),
                    new Position(-2.6996509024137052 + offset, 52.926572918779222 + offset),
                    new Position(-2.69628632041613 + offset, 52.89610842810761 + offset)
                })
            })
        });
        return multiPolygon;
    }

    [Test]
    public void Can_Serialize()
    {
        // Arrange
        var polygon1 = new PolygonCoordinates(new List<LinearRing>
        {
            new LinearRing(new List<Position>
            {
                new Position(0, 0),
                new Position(1, 0),
                new Position(1, 1),
                new Position(0, 1),
                new Position(0, 0)
            })
        });
        var polygon2 = new PolygonCoordinates(new List<LinearRing>
        {
            new LinearRing(new List<Position>
            {
                new Position(60, 60),
                new Position(61, 60),
                new Position(61, 61),
                new Position(60, 61),
                new Position(60, 60)
            }),
            new LinearRing(new List<Position>
            {
                new Position(70, 70),
                new Position(70, 71),
                new Position(71, 71),
                new Position(71, 70),
                new Position(70, 70)
            })
        });
        var multiPolygon = new MultiPolygon(new List<PolygonCoordinates> { polygon1, polygon2 });
        var expectedJson = GetExpectedJson();
        // Act
        var actualJson = JsonSerializer.Serialize(multiPolygon, DefaultJsonSerializerOptions);
        // Assert
        JsonAssert.AreEqual(expectedJson, actualJson);
    }

    [Test]
    public void Equals_GetHashCode_Contract()
    {
        //var rnd = new System.Random();
        //var offset = rnd.NextDouble() * 20;
        //if (rnd.NextDouble() < 0.5)
        //{
        //    offset *= -1;
        //}

        double offset = 0d;

        var left = GetMultiPolygon(offset);
        var right = GetMultiPolygon(offset);

        Assert.AreEqual(left, right);
        Assert.AreEqual(right, left);

        Assert.IsTrue(left.Equals(right));
        Assert.IsTrue(left.Equals(left));
        Assert.IsTrue(right.Equals(left));
        Assert.IsTrue(right.Equals(right));

        //Assert.IsTrue(left == right);
        //Assert.IsTrue(right == left);

        Assert.AreEqual(left.GetHashCode(), right.GetHashCode());
    }
}