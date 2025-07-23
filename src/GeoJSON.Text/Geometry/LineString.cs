// Copyright © Joerg Battermann 2014, Matt Hunt 2017

using System;
using System.Linq;

namespace Microsoft.Azure.Cosmos.Spatial
{
    public static class LineStringExtensions
    {
        /// <summary>
        /// Determines whether this instance has its first and last coordinate at the same position and thereby is closed.
        /// </summary>
        /// <returns>
        /// <c>true</c> if this instance is closed; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsClosed(this LineString lineString)
        {
            var firstCoordinate = lineString.Positions.First();
            var lastCoordinate = lineString.Positions.Last();

            return firstCoordinate.Longitude.Equals(lastCoordinate.Longitude)
                   && firstCoordinate.Latitude.Equals(lastCoordinate.Latitude)
                   && Nullable.Equals(firstCoordinate.Altitude, lastCoordinate.Altitude);
        }
    }
}