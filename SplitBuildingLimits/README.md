### Split Building Limits Boilerplate

Included in this project is some boilerplate code to get started on the programming task. You may of course alter any of the boilerplate code to fit your solution.

`samples` 

Includes [GeoJSON](https://en.wikipedia.org/wiki/GeoJSON) sample inputs for the building limits and height plateaus.

`ReadGeojsonClass.cs`

Includes the following methods:
- `ReadFileToString`: Reads GeoJSON from a local file to a string.
- `DeserializeGeojson`: Deserializes a GeoJSON string to a [NetTopologySuite](https://github.com/NetTopologySuite/NetTopologySuite) feature collection.

Note that the expectation is that the API accepts GeoJSON strings as input. The method to read from a local file is there simply to help you get started on the problem with the samples provided. 

`SplitBuildingLimitsClass.cs`

This is where you are expected to work from! There is a utility function `GetPolygonMember` to retrieve a specified 
attribute from a geometry feature. 

`Program.cs`

This is the expected entrypoint for the API (which you are expected to extend). Note there are two APIs:
- `Process` - This is the focal point of the assignment, in which you should implement the API which processes 
building limits and height plateaus. You will likely call `SplitBuildingLimitsClass.SplitBuildingLimits` from here.
- `Get` - This retrieves a specified set of geometry from the persistence layer. Note that you are not specifically required to implement this.
- 


###Edit:
All Validators are under GeoJsonValidators

DataStore are where data are being saved and read from.

Models represent an object in use.

Common.cs is used to share information across classes and validators as well as storing some constants

Now, few things I haven't got to do due to limited time:
-Telemetry. Ideally, APIs like these should definitely have logging and other type of Telemetry hooked up.
-Retry for the reading/updating data due to network failure etc. Due to limited time, I can't complete this now
-Setting up KeyVault (Microsoft's version of secret management) to store the connection string to the redis cache and the comsos database
-Unit testing


Side note:
-I removed the connection string on purpose due to security reasons. I'd be happy to show you the program running tomorrow during the interview.
