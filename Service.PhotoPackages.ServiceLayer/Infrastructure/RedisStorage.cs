using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Service.PhotoPackages.ServiceLayer.Infrastructure
{
    public class RedisStorage
    {
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private readonly IConfiguration _configuration;
        private readonly IDatabase _database;
        private const string ServicePrefix = "{photo_packages}";

        public RedisStorage(IConnectionMultiplexer connectionMultiplexer, IConfiguration configuration,
            IDatabase database)
        {
            _connectionMultiplexer = connectionMultiplexer;
            _configuration = configuration;
            _database = database;
        }

        public async Task<Dictionary<string, T>> GetKeysByProcess<T>(string processType) where T : class
        {

            var server = _connectionMultiplexer.GetServer(_connectionMultiplexer.Configuration.Split(',').First());
            var keys = new Dictionary<string, T>();

            var pattern = $"{ServicePrefix}:{processType}:";
            
            await foreach (var key in server.KeysAsync(pattern: $"{pattern}*"))
                keys.Add(key.ToString()[pattern.Length..], await GetValueByKey<T>(null, key.ToString()));

            return keys;
        }

        public async Task<T> GetValueByKey<T>(string processType, string key) where T : class
        {
            var value = await _database.StringGetAsync(BuildRedisKey(processType, key));

            if (!value.HasValue) return default;

            if (typeof(T) == typeof(string)) return value.ToString() as T;

            return JsonConvert.DeserializeObject<T>(value.ToString());
        }

        public async Task SetValueByKey<T>(string processType, string key, T value, TimeSpan expiryAfter)
        {
            await _database.StringSetAsync(BuildRedisKey(processType, key),
                new RedisValue(JsonConvert.SerializeObject(value)),
                expiryAfter);
        }

        public async Task SetKey(string processType, string key, TimeSpan expiryAfter)
        {
            await _database.StringSetAsync(BuildRedisKey(processType, key), RedisValue.EmptyString, expiryAfter);
        }

        public async Task DeleteKey(string processType, string key)
        {
            await _database.KeyDeleteAsync(BuildRedisKey(processType, key));
        }

        private RedisKey BuildRedisKey(string processType, string key)
        {
            return string.IsNullOrEmpty(processType) || key.Contains(processType)
                ? new RedisKey(key)
                : new RedisKey($"{ServicePrefix}:{processType}:{key}");
        }
    }
}