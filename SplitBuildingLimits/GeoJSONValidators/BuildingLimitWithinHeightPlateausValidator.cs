using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitBuildingLimits.GeoJSONValidators
{
    /// <summary>
    /// Check whether the building limits is within the height plateaus defined.
    /// </summary>
    internal class BuildingLimitWithinHeightPlateausValidator : IValidator
    {
        public void Validate()
        {
            // First we need to get the outer layer of the height plateaus boundary.
            var heightGeometries = Common.HeightFeatures.Select(f => f.Geometry).ToArray();
            var unionizedHeightGeometry = heightGeometries[0];            

            for (int i = 1; i < heightGeometries.Length; i++)
            {
                // Union method will combine the geometries and form an outer boundaries based on them.
                unionizedHeightGeometry = unionizedHeightGeometry.Union(heightGeometries[i]);
            }

            // Get the hashes of the all the height geometry set.
            HashSet<int> unionizedHeightGeometryCordinateSet = unionizedHeightGeometry.Coordinates
                .Select(c => c.GetHashCode())
                .ToHashSet();

            // Once we get the outer boundary, we can check whether the geometries of the building limits all fit within the boundary.
            var buildingLimitGeometries = Common.BuildingFeatures.Select(f => f.Geometry).ToList();

            foreach (var buildingLimitGeometry in buildingLimitGeometries)
            {
                // Doing this because covers didn't work as intended for some reason. May need to investigate further
                HashSet<int> buildingLimitGeometryCordinateSet = buildingLimitGeometry.Coordinates.Select(c => c.GetHashCode()).ToHashSet();

                // If it is a subset of heightplateaus cordinates, then all building limit geometry cordinates must be within the other.
                if (buildingLimitGeometryCordinateSet.IsSubsetOf(unionizedHeightGeometryCordinateSet))
                    continue;

                // There might be a bug in the framework code. Based on the comments within the framework and the wikipedia article here, it should have returned a postive answer
                // For some reason, it didn't.
                // https://en.wikipedia.org/wiki/DE-9IM
                if (!unionizedHeightGeometry.Covers(buildingLimitGeometry))
                    throw new ArgumentException($"{buildingLimitGeometry} with cordinate {buildingLimitGeometry.Coordinate.ToString()}  is not covered by height plateaus. Please recheck the cordinates of each.");
            }
        }
    }
}
