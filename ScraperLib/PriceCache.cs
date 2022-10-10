using Common;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Runtime.Serialization;

namespace ScraperLib
{
    /* TODO: Potential issues if multiple PriceCaches are created! */
    internal class PriceCache
    {
        private readonly String _cachePath;
        /* Stores which dates are cached */
        IDictionary<DateTime, DayPrices> _cache;

        public PriceCache(String cachePath = "./price-cache.bin")
        {
            _cachePath = cachePath;
            _cache = LoadCache() ?? new Dictionary<DateTime, DayPrices>();
        }

        /* File format: */
        /* U32 - Entry count
         * n * (U32 - Days after epoch, Double[24] - Prices) - DayPrices
         */
        private IDictionary<DateTime, DayPrices>? LoadCache()
        {
            /* Potential problems - different timezone bug */
            try
            {
                BinaryReader reader = new BinaryReader(File.Open(_cachePath, FileMode.Open));
                UInt32 entryCount = reader.ReadUInt32();
                long length = new System.IO.FileInfo(_cachePath).Length;
                if (length != sizeof(UInt32) + entryCount * (sizeof(UInt32) + 24 * sizeof(Double)))
                {
                    /* TODO: Probably not the right exception */
                    throw new FileLoadException("Invalid format");
                }

                Dictionary<DateTime, DayPrices> cache = new Dictionary<DateTime, DayPrices>();
                for (uint i = 0; i < entryCount; i++)
                {
                    UInt32 daysAfterEpoch = reader.ReadUInt32();
                    Double[] priceVals = new Double[24];
                    for (int j = 0; j < 24; j++)
                    {
                        priceVals[j] = reader.ReadDouble();
                    }

                    DateTime date = DateTime.UnixEpoch.AddDays(daysAfterEpoch);
                    DayPrices dayPrices = new DayPrices(date, priceVals);
                    cache.Add(date, dayPrices);
                }

                reader.Close();
                Console.WriteLine($"Loaded cache with {cache.Count} entries");
                return cache;
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine("Cache doesn't exist");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to load cache ({e.Message})");
            }

            return null;
        }

        /* Saves price to file */
        private void SaveCache()
        {
            try
            {
                BinaryWriter writer = new BinaryWriter(File.Open(_cachePath, FileMode.Create));
                writer.Write((UInt32)_cache.Count);
                foreach(KeyValuePair<DateTime, DayPrices> entry in _cache)
                {
                    writer.Write((UInt32)(entry.Key - DateTime.UnixEpoch).TotalDays);
                    for (int j = 0; j < 24; j++)
                    {
                        writer.Write((Double)entry.Value.HourlyPrices[j]);
                    }
                }

                writer.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to save cache ({e.Message})");
            }
        }

        public DayPrices? SearchCache(DateTime date)
        {
            if (_cache.ContainsKey(date))
            {
                return _cache[date];
            }

            return null;
        }

        public void PopulateCache(IEnumerable<DayPrices> pricesToCache)
        {
            int newEntries = 0;
            foreach (var day in pricesToCache)
            {
                if (!_cache.ContainsKey(day.Date))
                {
                    _cache.Add(day.Date, day);
                    newEntries++;
                }
                else
                {
                    /* Overwrite previous entry if different */
                    if (_cache[day.Date].CompareTo(day) != 0)
                    {
                        _cache[day.Date] = day;
                        newEntries++;
                    }
                }
            }

            /* Only write cache if there are new entries */
            if (newEntries != 0)
            {
                Console.WriteLine($"Saving cache ({newEntries} new entries)");
                SaveCache();
            }
        }
    }
}
