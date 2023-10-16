using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO.Converters;
using SplitBuildingLimits.GeoJSONValidators;
using System.Text.Json.Serialization;
using SplitBuildingLimits.DataStore;
using Newtonsoft.Json;

namespace SplitBuildingLimits;

public class SplitBuildingLimits
{
    private static Guid projectId = Guid.NewGuid();

    private static IDataStore dataStore = new AzureDataStore();
    public static void Main(string[] args)
    {
        string buildingLimitPath = @"C:\\Users\zhouf\Downloads\SplitBuildingLimits\SplitBuildingLimits\samples\SampleBuildingLimits.json";
        string buildingStr = ReadGeojsonClass.ReadFileToString(buildingLimitPath);

        string heightPlateausPath = @"C:\\Users\zhouf\Downloads\SplitBuildingLimits\SplitBuildingLimits\samples\SampleHeightPlateaus.json";
        string heightStr = ReadGeojsonClass.ReadFileToString(heightPlateausPath);
        Process(buildingStr, heightStr, projectId);
        Get(projectId, Common.BuildingLimits);
        Get(projectId, Common.HeightPlateaus);
        Get(projectId, Common.SplittedBuildingLimits);
    }
    /**
    * This method is the core entry point for the assignment. 
    *
    * It accepts as input the building limit and height plateau geojsons. It then splits the 
    * building limits according to the height plateaus and persists:
    * 1. The original building limits
    * 2. The original height plateaus
    * 3. The split building limits
    *
    * You are of course free to change the method signature as you see fit.
    */

    public static void Process(string buildingLimits, string heightPlateaus, Guid projectId, bool overrideChanges = false) {
        // Validate the inputs
        IValidator validator = new CombinedValidator(buildingLimits, heightPlateaus);
        validator.Validate();

        //Split the buildingLimits to heights
        var splitted = SplitBuildingLimitsClass<IFeature>.SplitBuildingLimits(Common.BuildingFeatures.ToList(), Common.HeightFeatures.ToList());
        var strSplitted = JsonConvert.SerializeObject(splitted, Formatting.None,
                        new JsonSerializerSettings()
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        });

        //Persist the data
        dataStore.UpsertData(projectId.ToString(), Common.BuildingLimits, buildingLimits);
        dataStore.UpsertData(projectId.ToString(), Common.HeightPlateaus, heightPlateaus);
        dataStore.UpsertData(projectId.ToString(), Common.SplittedBuildingLimits, strSplitted);
    }

    /// <summary>
    /// This method is used to get the building limits, height plateaus, and split building limits
    /// from the persistence layer.
    /// </summary>
    /// <param name="projectId"></param>
    public static string Get(Guid projectId, string propertyName) {
        if(string.Equals(propertyName, Common.HeightPlateaus, StringComparison.InvariantCultureIgnoreCase) ||
            string.Equals(propertyName, Common.BuildingLimits, StringComparison.InvariantCultureIgnoreCase) ||
            string.Equals(propertyName, Common.SplittedBuildingLimits, StringComparison.InvariantCultureIgnoreCase))
        {
            Console.WriteLine("Entered Get");
            string value = dataStore.GetData(projectId.ToString(), propertyName).GetAwaiter().GetResult();
            Console.WriteLine(value);
            //Returning a string for now. We can choose to deserialize object.
            return value;
        }

        throw new ArgumentException($"No such property name is present. Please specify either {Common.HeightPlateaus}, {Common.BuildingLimits} or {Common.SplittedBuildingLimits}");
    } 
}