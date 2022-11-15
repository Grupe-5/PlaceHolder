using Azure.Messaging.ServiceBus;
using GP3.Common.Constants;
using GP3.Common.Entities;
using GP3.Funcs.Services;
using GP3.Scraper;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GP3.Funcs.Functions.Timer
{
    public class ScrapeTimer
    {
        private readonly ILogger<ScrapeTimer> _logger;
        private readonly PriceService _priceService;
        public ScrapeTimer(PriceService priceService, ILogger<ScrapeTimer> logger)
        {
            _priceService = priceService;
            _logger = logger;
        }

        private struct PriceMessages
        {
            public ServiceBusMessage lowestPrice;
            public ServiceBusMessage highestPrice;
        }

        private static PriceMessages GeneratePriceMessage (DayPrice price)
        {
            PriceMessages x;
            x.lowestPrice = new (IntegrationCallbackReason.LowestPrice.ToString())
            {
                ScheduledEnqueueTime = price.LowestPriceTimeUTC()
            };

            x.highestPrice = new (IntegrationCallbackReason.HighestPrice.ToString())
            {
                ScheduledEnqueueTime = price.HighestPriceTimeUTC()
            };

            return x;
        }

        [FunctionName("Scrape")]
        public async Task Run(
            [TimerTrigger("0 * * * *", RunOnStartup = true)] TimerInfo funcTimer,
            [ServiceBus(ConnStrings.IntegrationQ, Connection = ConnStrings.IntegrationQConn)] ServiceBusSender sender
            )
        {
            var newPrices = await _priceService.GetMissingDates();
            foreach (var price in newPrices)
            {
                var msg = GeneratePriceMessage(price);
                if (msg.lowestPrice.ScheduledEnqueueTime > DateTime.UtcNow)
                {
                    _logger.LogInformation($"Enqueued LP msg for {msg.lowestPrice.ScheduledEnqueueTime}");
                    await sender.SendMessageAsync(msg.lowestPrice);
                }
                if (msg.highestPrice.ScheduledEnqueueTime > DateTime.UtcNow)
                {
                    _logger.LogInformation($"Enqueued HP msg for {msg.highestPrice.ScheduledEnqueueTime}");
                    await sender.SendMessageAsync(msg.highestPrice);
                }
            }
        }
    }
}
