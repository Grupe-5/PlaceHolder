using GP3.Common.Constants;
using GP3.Common.Entities;
using GP3.Common.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Extensions.JwtCustomHandler;
using Microsoft.Azure.Functions.Extensions.JwtCustomHandler.Interface;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Net;
using System.Threading.Tasks;

namespace GP3.Funcs
{
    public class PriceFunc
    {
        private readonly ILogger<PriceFunc> _logger;
        private readonly IDayPriceRepository _priceRepository;
        /*
         * TODO: Firebase token provider could be replaced with generic token provider 
         * See: gh - kuromukira/azure-functions-jwt-validation-extension
         */
        private readonly IFirebaseTokenProvider _tokenProvider;

        public PriceFunc(IDayPriceRepository priceRepository, IFirebaseTokenProvider tokenProvider, ILogger<PriceFunc> log)
        {
            _priceRepository = priceRepository;
            _logger = log;
            _tokenProvider = tokenProvider;
        }

        [FunctionName("PriceHttpGet")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "date" })]
        [OpenApiParameter(name: "date", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "Date in yyyy-MM-dd format")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(DayPrice), Description = "Price found")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.NotFound, contentType: "text/plain", bodyType: typeof(string), Description = "Price not found")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "text/plain", bodyType: typeof(string), Description = "Invalid date")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.Unauthorized, contentType: "text/plain", bodyType: typeof(string), Description = "Invalid token")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = Routes.GetPrice)] HttpRequest req)
        {
            try
            {
                if (req.Headers is null)
                    throw new NullReferenceException();
                else if (req.Body is null)
                    throw new NullReferenceException();

                var tokenResult = await _tokenProvider.ValidateToken(req);
                if (tokenResult.Status != AccessTokenStatus.Valid)
                    return new UnauthorizedObjectResult("Invalid access token");

                string date = req.Query["date"];
                if (!DateTime.TryParse(date, out DateTime dateVal))
                {
                    return new BadRequestObjectResult("Date must be supplied in this format: yyyy-MM-dd");
                }

                var price = await _priceRepository.GetByDateAsync(dateVal);
                if (price == null)
                {
                    return new NotFoundObjectResult($"Price for {dateVal:yyyy-MM-dd} not available!");
                }
                return new OkObjectResult(price);
            }
            catch (Exception e)
            {
                _logger.LogError("Server encountered an error: {0}", e.Message);
                return new BadRequestObjectResult("Web server encountered an error");
            }
        }
    }
}

