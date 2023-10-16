using System.Text;
using NetTopologySuite.Features;
using NetTopologySuite.IO;
using Newtonsoft.Json;

namespace SplitBuildingLimits;

public static class ReadGeojsonClass
{
    public static string ReadFileToString(string filePath)
    {
        string text;
        var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
        {
            text = streamReader.ReadToEnd();
        }
        if (string.IsNullOrEmpty(text))
        {
            throw new Exception("Contents of file are empty");
        }
        return text;
    }

    public static FeatureCollection DeserializeGeojson(string geojson)
    {
        var serializer = GeoJsonSerializer.Create();
        using (var stringReader = new StringReader(geojson))
        using (var jsonReader = new JsonTextReader(stringReader))
        {
            var geometry = serializer.Deserialize<FeatureCollection>(jsonReader) ?? throw new Exception("Geometry is null");
            return geometry;
        }
        throw new Exception("Unable to deserialize json");
    }

}