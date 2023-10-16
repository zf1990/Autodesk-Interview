using NetTopologySuite.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SplitBuildingLimits.GeoJSONValidators
{
    /// <summary>
    /// This would indicate something is wrong with the validation itself.
    /// </summary>
    public class InternalValidationException : Exception
    {
        public InternalValidationException(string msg) : base(msg)
        { }
    }
    public class CombinedValidator : IValidator
    {
        /// <summary>
        /// String representation of the building limits
        /// </summary>
        internal string buildingLimits { get; set; }
        /// <summary>
        /// String representation of the height plateaus limits
        /// </summary>
        internal string heightPlateaus { get; set; }
        /// <summary>
        /// Collection of all the individual validators each validating one aspect of the geometry.
        /// </summary>
        internal IList<IValidator> validators { get; set; } = new List<IValidator>();
        public CombinedValidator(string buildingLimits, string heightPlateaus)
        {
            if(string.IsNullOrWhiteSpace(buildingLimits))
                throw new ArgumentNullException(nameof(buildingLimits));

            if(string.IsNullOrWhiteSpace(heightPlateaus))
                throw new ArgumentNullException(nameof(buildingLimits));

            validators.Add(new InputStringValidator(buildingLimits, Common.BuildingLimits));
            validators.Add(new InputStringValidator(heightPlateaus, Common.HeightPlateaus));
            validators.Add(new BoundaryOverlapValidator(Common.BuildingLimits));
            validators.Add(new BoundaryOverlapValidator(Common.HeightPlateaus));
            validators.Add(new BuildingLimitWithinHeightPlateausValidator());
        }
        
        public void Validate()
        {
            foreach (var validator in validators)
            {
                validator.Validate();
            }
        }
    }
}
