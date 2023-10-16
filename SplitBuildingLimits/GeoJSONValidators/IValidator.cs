using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitBuildingLimits.GeoJSONValidators
{
    /// <summary>
    /// Interface for all the validators.
    /// </summary>
    public interface IValidator
    {
        /// <summary>
        /// Validate the geometry
        /// </summary>
        void Validate();
    }
}
