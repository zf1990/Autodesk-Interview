using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitBuildingLimits.DataStore
{
    public interface IDataStore
    {
        Task UpsertData(string projectId, string propertyName, string value);
        Task<string> GetData(string projectId, string propertyName);
    }
}
