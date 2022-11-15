using GP3.Common.Constants;
using GP3.Common.Entities;
using GP3.Common.Repositories;
using GP3.Funcs.Services;
using GP3.Scraper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace GP3.Funcs.Functions.HTTP
{
    public class PriceHttp
    {
        private readonly IDayPriceRepository _priceRepository;
        private readonly ReqAuthService _reqAuthService;

        public PriceHttp(IDayPriceRepository priceRepository, ReqAuthService reqAuthService)
        {
            _priceRepository = priceRepository;
            _reqAuthService = reqAuthService;
        }

        [FunctionName("PriceHttp")]
        [OpenApiOperation(operationId: "Get Price")]
        [OpenApiSecurity("bearer_auth", SecuritySchemeType.Http, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
        [OpenApiParameter(name: "date", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "Date in yyyy-MM-dd format")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(DayPrice), Description = "Price found")]
        public async Task<IActionResult> GetPrice([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = Routes.Price)] HttpRequest req)
            => await _reqAuthService.HandledAuthAction(req, async (req, _) =>
            {
                string date = req.Query["date"];
                if (!DateTime.TryParse(date, out DateTime dateVal))
                    return new BadRequestObjectResult("Date must be supplied in this format: yyyy-MM-dd");

                var price = await _priceRepository.GetDayPriceAsync(dateVal);
                if (price == null)
                    return new NotFoundObjectResult($"Price for {dateVal:yyyy-MM-dd} not available!");
                else
                    return new OkObjectResult(price);
            });

        [FunctionName("PriceOffsetHttp")]
        [OpenApiOperation(operationId: "Get Price /w offset")]
        [OpenApiSecurity("bearer_auth", SecuritySchemeType.Http, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
        [OpenApiParameter(name: "date", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "Date and hour in yyyy-MM-dd:HH format (assumed as UTC!)")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(double[]), Description = "Prices that were available")]
        public async Task<IActionResult> GetPriceOffset([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = Routes.PriceOffset)] HttpRequest req)
            => await _reqAuthService.HandledAuthAction(req, async (req, _) =>
            {
                string date = req.Query["date"];
                if (!DateTime.TryParseExact(date, "yyyy-MM-dd:HH", null, System.Globalization.DateTimeStyles.None, out DateTime dateVal))
                    return new BadRequestObjectResult("Date must be supplied in this format: yyyy-MM-dd:HH");

                dateVal = dateVal.UTCtoCEST();
                List<double> priceCollection = new();

                var priceDay = await _priceRepository.GetDayPriceAsync(dateVal);
                if (priceDay != null)
                {
                    priceCollection.AddRange(priceDay.HourlyPrices.Skip(dateVal.Hour));
                    var priceNext = await _priceRepository.GetDayPriceAsync(dateVal.AddDays(1));
                    if (priceNext != null)
                    {
                        priceCollection.AddRange(priceNext.HourlyPrices.Take(dateVal.Hour));
                    }
                }

                priceCollection.AddRange(Enumerable.Range(0, 24 - priceCollection.Count).Select(i => 0.0));
                return new OkObjectResult(priceCollection);
            });
    }
}

