using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitBuildingLimits.GeoJSONValidators
{
    /// <summary>
    /// This is to make sure that the input string is actually 
    /// </summary>
    internal class InputStringValidator : IValidator
    {
        /// <summary>
        /// String representing the feature collection object
        /// </summary>
        private string InputString { get; }
        public string PropertyName { get; }

        public InputStringValidator(string inputString, string propertyName) {
            InputString = inputString;
            PropertyName = propertyName;
        }

        public void Validate()
        {
            if (string.IsNullOrEmpty(InputString))
                throw new ArgumentException($"{PropertyName} is empty. Please provide a format string that fits the FeatureCollection Model");
            try
            {
                var featureCollection = ReadGeojsonClass.DeserializeGeojson(InputString);
                if (featureCollection.Any(f => f.Geometry.GeometryType != Geometry.TypeNamePolygon))
                    throw new ArgumentException($"Not all cordinates in {PropertyName} form a Polygon. Please try again");

                if (featureCollection.Any(f => !f.Geometry.IsValid))
                    throw new ArgumentException($"Not all cordinates in {PropertyName} is a valid Polygon. Please try again");

                switch(PropertyName)
                {
                    case Common.BuildingLimits:
                        Common.BuildingFeatures = featureCollection;
                        break;
                    case Common.HeightPlateaus:
                        Common.HeightFeatures = featureCollection;
                        break;
                    default:
                        throw new InternalValidationException("Unknown property name");
                }
                
            }
            catch (Exception) {
                throw new Exception($"Unable to deserialize the {PropertyName} string. Please make sure it is in a proper format and try again");
            }
        }
    }
}
