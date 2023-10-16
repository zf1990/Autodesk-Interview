using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using Newtonsoft.Json;
using SplitBuildingLimits.Models;
using System.Dynamic;

namespace SplitBuildingLimits;

public static class SplitBuildingLimitsClass<TPolygon> where TPolygon : IFeature
{
    /**
    * Example usage: 
    * GetPolygonMember<double>(polygon, "elevation"); // Returns the elevation property corresponding to the polygon if it exists, else null
    */
    private static T GetPolygonMember<T>(TPolygon polygon, string key)
    {
        return (T) polygon.Attributes.GetOptionalValue(key);
    }

    /**
     * Consumes a list of building limits (polygons) and a list of height plateaus (polygons). Splits up the building
     * limits according to the height plateaus.
     * 
     * <param name="buildingLimits"> A list of buildings limits. A building limit is a polygon indicating where building can happen. </param>
     * <param name="heightPlateaus"> A list of height plateaus. A height plateau is a discrete polygon with a constant elevation. </param>
     */
    public static IEnumerable<SplittedBuildingLimit> SplitBuildingLimits(List<TPolygon> buildingLimits, List<TPolygon> heightPlateaus)
    {
        Console.WriteLine("Splitting building limits according to height plateaus");
        var gf = NetTopologySuite.NtsGeometryServices.Instance.CreateGeometryFactory(4326);
        List<SplittedBuildingLimit> splittedBuildingFeatures = new List<SplittedBuildingLimit>();

        foreach(var buildingLimit in buildingLimits)
        {
            var buildingGeomtry = buildingLimit.Geometry;
            foreach(var heightPlateau in heightPlateaus)
            {
                var heightGeometry = heightPlateau.Geometry;

                // Check if they even overlap, if not, no need to do anything for this height plateau
                if (heightGeometry.Disjoint(buildingGeomtry))
                    continue;

                // Take the difference in geometry between them
                Geometry splittedGeometry = buildingGeomtry.Difference(heightGeometry);

                var splittedFeature = new SplittedBuildingLimit
                {
                    Geometry = splittedGeometry,
                    Elevation = GetPolygonMember<double>(heightPlateau, Common.Elevation)
                };
                splittedBuildingFeatures.Add(splittedFeature);
            }
        }
        
        return splittedBuildingFeatures;
    }
}