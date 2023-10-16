using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitBuildingLimits.Models
{
    public class SplittedBuildingLimit
    {
        public Geometry Geometry { get; set; }
        public double Elevation { get; set; }
    }
}
