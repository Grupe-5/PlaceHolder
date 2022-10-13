using System.IO.Compression;

namespace ScraperLib
{
    internal static class BrotliCompressor
    {
        public static async Task<byte[]> CompressAsync(Stream input)
        {
            await using var output = new MemoryStream();
            await using var stream = new BrotliStream(output, CompressionLevel.Optimal);

            input.Position = 0;
            await input.CopyToAsync(stream);
            await stream.FlushAsync();
            return output.ToArray();
        }

        public static async Task DecompressAsync(Stream output, byte[] value)
        {
            await using var input = new MemoryStream(value);
            await using var stream = new BrotliStream(input, CompressionMode.Decompress);

            await stream.CopyToAsync(output);
            output.Position = 0;
        }
    }
}
