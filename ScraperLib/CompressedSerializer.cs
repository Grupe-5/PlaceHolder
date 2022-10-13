using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ScraperLib
{
    static internal class CompressedSerializer<T>
    {
        public static async Task WriteFileAsync(String path, T value)
        {
            try
            {
                await using var data = new MemoryStream();
                await JsonSerializer.SerializeAsync<T>(data, value);
                var compressed = await BrotliCompressor.CompressAsync(data);
                await File.WriteAllBytesAsync(path, compressed);
            }
            catch { } /* TODO: Re-throw exception */
        }

        public static async Task<T?> LoadFileAsync(String path)
        {
            try
            {
                var compressed = await File.ReadAllBytesAsync(path);
                await using var jsonStream = new MemoryStream();
                await BrotliCompressor.DecompressAsync(jsonStream, compressed);
                var str = await JsonSerializer.DeserializeAsync<T>(jsonStream);
                return str;
            } /* Only exception that shouldn't be re-thrown is file not found */
            catch { } /* TODO: Re-throw exception */
            return default;
        }
    }
}
