using NetTopologySuite.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitBuildingLimits
{
    /// <summary>
    /// Used for sharing the the Feature collections between validators.
    /// </summary>
    public static class Common
    {
        public const string BuildingLimits = "BuildingLimits";

        public const string HeightPlateaus = "HeightPlateaus";

        public const string SplittedBuildingLimits = "SplittedBuildingLimits";

        public const string Elevation = "elevation";
        /// <summary>
        /// Database name in the cosmos db
        /// </summary>
        public const string DatabaseName = "autodesk-test";

        /// <summary>
        /// Container name in cosmos db
        /// </summary>
        public const string ContainerName = "autodesk-container";


        public static FeatureCollection? BuildingFeatures { get; set; }
        public static FeatureCollection? HeightFeatures { get; set; }
    }
}
