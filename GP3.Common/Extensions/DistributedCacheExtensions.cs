using Microsoft.Extensions.Caching.Distributed;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GP3.Common.Extensions
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

        public static async Task SetAsync<T>(this IDistributedCache cache, string key, T value)
            => await cache.SetAsync(key, value, new DistributedCacheEntryOptions());
        public static async Task SetAsync<T>(this IDistributedCache cache, string key, T value, DistributedCacheEntryOptions options)
        {
            var bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(value, _serializerOptions));
            await cache.SetAsync(key, bytes, options);
        }
        public static async Task<T?> GetValueAsync<T>(this IDistributedCache cache, string key)
        {
            var val = await cache.GetAsync(key);
            if (val == null) return default;
            return JsonSerializer.Deserialize<T>(val, _serializerOptions);
        }
    }
}
