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
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MySqlConnector;
using System;
using System.Net;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace GP3.Funcs.Functions.HTTP
{
    public class IntegrationHttp
    {
        private readonly ReqAuthService _reqAuthService;
        private readonly IIntegrationRepository _integrationRepository;
        public IntegrationHttp(ReqAuthService reqAuthService, IIntegrationRepository integrationRepository)
        {
            _reqAuthService = reqAuthService;
            _integrationRepository = integrationRepository;
        }

        [FunctionName("IntegrationHttpGet")]
        [OpenApiOperation(operationId: "Get Integrations")]
        [OpenApiSecurity("bearer_auth", SecuritySchemeType.Http, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(IntegrationCallback), Description = "Found integrations")]
        public async Task<IActionResult> GetIntegrations([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = Routes.Integration)] HttpRequest req) =>
            await _reqAuthService.HandledAuthAction(req, async (req, principal) => 
            {
                var integrations = await _integrationRepository.GetIntegrationsByUserAsync(principal.FindFirstValue("user_id"));
                if (integrations == null)
                {
                    return new BadRequestObjectResult("Something went wrong?");
                }

                return new OkObjectResult(integrations);
            });

        [FunctionName("IntegrationHttpPost")]
        [OpenApiOperation(operationId: "Add Integrations")]
        [OpenApiSecurity("bearer_auth", SecuritySchemeType.Http, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
        [OpenApiRequestBody("application/json", typeof(IntegrationCallback), Required = true)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(IntegrationCallback), Description = "Added integration")]
        public async Task<IActionResult> AddIntegration([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = Routes.Integration)] HttpRequest req) =>
            await _reqAuthService.HandledAuthAction(req, async (req, principal) =>
            {
                IntegrationCallback data = await JsonSerializer.DeserializeAsync<IntegrationCallback>(req.Body, new JsonSerializerOptions(JsonSerializerDefaults.Web));
                if (data == null || data.CallbackUrl == null)
                    return new BadRequestObjectResult("Invalid integration callback");

                data.Id = default;
                data.User = principal.FindFirstValue("user_id");

                if (!Uri.TryCreate(data.CallbackUrl, UriKind.Absolute, out var uriResult)
                || !(uriResult.Scheme == Uri.UriSchemeHttps || uriResult.Scheme == Uri.UriSchemeHttp)
                || uriResult.IsLoopback)
                {
                    return new BadRequestObjectResult("Invalid integration URL");
                }

                try
                {
                    await _integrationRepository.AddIntegrationAsync(data);
                }
                catch (DbUpdateException ex)
                {
                    if (ex.InnerException is MySqlException inner && inner.ErrorCode == MySqlErrorCode.DuplicateKeyEntry)
                        return new UnprocessableEntityObjectResult("Integration already exists");

                    throw;
                }

                return new OkObjectResult(data);
            });

        [FunctionName("IntegrationHttpDelete")]
        [OpenApiOperation(operationId: "Delete integration")]
        [OpenApiSecurity("bearer_auth", SecuritySchemeType.Http, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
        [OpenApiParameter(name: "id", In = ParameterLocation.Query, Required = true, Type = typeof(int), Description = "Id of integration to remove")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "Removed integration")]
        public async Task<IActionResult> DeleteIntegration([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = Routes.Integration)] HttpRequest req) =>
            await _reqAuthService.HandledAuthAction(req, async (req, principal) =>
            {
                if (!int.TryParse(req.Query["id"], out int integrationId))
                    return new BadRequestObjectResult("No ID provided");

                var userId = principal.FindFirstValue("user_id");
                var integration = await _integrationRepository.GetIntegrationAsync(integrationId);
                if (integration == null || integration.User != userId)
                    return new BadRequestObjectResult("Invalid ID provided");

                await _integrationRepository.DeleteIntegrationAsync(integration);
                return new OkResult();
            });
    }
}

