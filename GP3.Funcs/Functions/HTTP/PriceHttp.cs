using GP3.Common.Constants;
using GP3.Common.Entities;
using GP3.Common.Repositories;
using GP3.Funcs.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.OpenApi.Models;
using System;
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
    }
}

