using Microsoft.Extensions.Caching.Distributed;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GP3.Common.DB
{
    public static class DistributedCacheExtensions
    {
        private static readonly JsonSerializerOptions _serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = null,
            WriteIndented = true,
            AllowTrailingCommas = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public static Task SetAsync<T>(this IDistributedCache cache, string key, T value)
        {
            return SetAsync(cache, key, value, new DistributedCacheEntryOptions());
        }
        public static Task SetAsync<T>(this IDistributedCache cache, string key, T value, DistributedCacheEntryOptions options)
        {
            var bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(value, _serializerOptions));
            return cache.SetAsync(key, bytes, options);
        }
        public static async Task<T?> GetValueAsync<T>(this IDistributedCache cache, string key)
        {
            var val = await cache.GetAsync(key);
            if (val == null) return default;
            return JsonSerializer.Deserialize<T>(val, _serializerOptions);
        }
    }
}
