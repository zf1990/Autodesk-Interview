using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StackExchange.Redis;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Redis;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Azure.Cosmos;
using SplitBuildingLimits.Models;

namespace SplitBuildingLimits.DataStore
{
    public class AzureDataStore : IDataStore
    {
        private readonly string cosmosConnectionString;
        private RedisCache redisClient;
        private CosmosClient cosmosClient;
        private Container cosmosContainer;
        private DistributedCacheEntryOptions redisOption;
        public AzureDataStore()
        {
            // This is more of a hack, ideally, this should be in startup.cs somewhere or injected through dependency injection
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfiguration config = builder.Build();
            cosmosConnectionString = config["ConnectionStrings:ComsosDB"];
            string azureCacheConnectionString = config["ConnectionStrings:AzureCache"];
            this.redisClient = new RedisCache(new RedisCacheOptions() { Configuration = azureCacheConnectionString, InstanceName = "Master" });
            redisOption = new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromHours(24) //This is just default assumption. Value to be changed based on user requirement.
            };

            // Cosmos Client initialization
            cosmosClient = new CosmosClient(cosmosConnectionString);
            var dataBase = cosmosClient.CreateDatabaseIfNotExistsAsync(Common.DatabaseName).GetAwaiter().GetResult().Database;
            cosmosContainer = dataBase.CreateContainerIfNotExistsAsync(new ContainerProperties(Common.ContainerName, $"/projectId")).GetAwaiter().GetResult().Container;
        }
        public async Task<string> GetData(string projectId, string propertyName)
        {
            string key = $"{projectId.ToString()}-{propertyName}";
            // Look for data in redis first, if not there, 
            string value = await redisClient.GetStringAsync(key);
            if(string.IsNullOrWhiteSpace(value)) {
                // Cache seems to have been deleted. Find the corresponding data in cosmos db.
                var item = await cosmosContainer.ReadItemAsync<CosmosItem>(id: key, partitionKey: new PartitionKey($"{projectId}"));
                var cosmosValue = item.Resource.data;
                await this.redisClient.SetStringAsync(key, cosmosValue, redisOption);
                return cosmosValue;
            } else
            {
                return value;
            }
        }

        public async Task UpsertData(string projectId, string propertyName, string value)
        {
            // Saving data in redis first
            string key = $"{projectId.ToString()}-{propertyName}";
            var redisTask = this.redisClient.SetStringAsync(key, value, redisOption);

            // Saving data in Comsos 
            CosmosItem newItem = new CosmosItem
            {
                id = key,
                data = value,
                projectId = projectId
            };
            var cosmosTask = this.cosmosContainer.UpsertItemAsync<CosmosItem>(newItem, new PartitionKey($"{projectId}"));

            await Task.WhenAll(new Task[] { redisTask, cosmosTask});
        }
    }
}
