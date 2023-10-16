using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitBuildingLimits.GeoJSONValidators
{
    /// <summary>
    /// This class is to valid whether the collection of height plateaus or building limits overlap one another in an invalid way (aka crossing over boundaries of other height plateaus or building limit)
    /// </summary>
    internal class BoundaryOverlapValidator : IValidator
    {
        public string PropertyName { get; }
        public BoundaryOverlapValidator(string propertyName) {
            PropertyName = propertyName;
        }

        /// <summary>
        /// inheritdoc
        /// </summary>
        public void Validate()
        {
            Geometry[] geometries = new Geometry[0];
            switch (this.PropertyName)
            {
                case Common.BuildingLimits:
                    geometries = Common.BuildingFeatures.Select(f => f.Geometry).ToArray();
                    break;
                case Common.HeightPlateaus:
                    geometries = Common.HeightFeatures.Select(f => f.Geometry).ToArray();
                    break;
                default:
                    throw new InternalValidationException("Unknown property name");
            }

            for (int i = 0; i < geometries.Length - 1; i++)
            {
                Geometry geometry = geometries[i];
                for (int j = i + 1; j < geometries.Length; j++)
                {
                    Geometry otherGeometry = geometries[j];
                    if (geometry.Crosses(otherGeometry))
                    {
                        throw new ArgumentException($"{PropertyName}'s features crosses into the boundary of one another");
                    }
                }
            }
        }

    }
}
