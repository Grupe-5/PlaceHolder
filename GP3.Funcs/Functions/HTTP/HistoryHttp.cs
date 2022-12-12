using GP3.Common.Constants;
using GP3.Common.Entities;
using GP3.Funcs.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.OpenApi.Models;
using System.Net;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace GP3.Funcs.Functions.HTTP;
public class HistoryHttp
{
    private readonly ReqAuthService _reqAuthService;
    private readonly ProviderService _provider;

    public HistoryHttp(ReqAuthService reqAuthService, ProviderService provider)
    {
        _reqAuthService = reqAuthService;
        _provider = provider;
    }

    [FunctionName("HistoryCurrentDraw")]
    [OpenApiOperation(operationId: "Get current power draw")]
    [OpenApiSecurity("bearer_auth", SecuritySchemeType.Http, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(double), Description = "Current power draw")]
    public Task<IActionResult> GetCurrentDraw([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = Routes.HistoryCurrentDraw)] HttpRequest req) =>
        _reqAuthService.HandledAuthAction(req, async (req, principal) => 
        {
            return new OkObjectResult(await _provider.GetCurrentDraw(principal.FindFirstValue("user_id")));
        });

    [FunctionName("HistoryDailyDraw")]
    [OpenApiOperation(operationId: "Get amount of power used today")]
    [OpenApiSecurity("bearer_auth", SecuritySchemeType.Http, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(int), Description = "Power used today")]
    public Task<IActionResult> GetDailyUsage([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = Routes.HistoryDailyUsage)] HttpRequest req) =>
        _reqAuthService.HandledAuthAction(req, async (req, principal) => 
        {
            return new OkObjectResult(await _provider.GetDailyUsage(principal.FindFirstValue("user_id")));
        });

    [FunctionName("HistoryMonthlyDraw")]
    [OpenApiOperation(operationId: "Get amount of power used this month")]
    [OpenApiSecurity("bearer_auth", SecuritySchemeType.Http, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(int), Description = "Power used this month")]
    public Task<IActionResult> GetMonthlyUsage([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = Routes.HistoryMonthlyUsage)] HttpRequest req) =>
        _reqAuthService.HandledAuthAction(req, async (req, principal) => 
        {
            return new OkObjectResult(await _provider.GetMonthyUsage(principal.FindFirstValue("user_id")));
        });

    [FunctionName("HistoryRegisterProvider")]
    [OpenApiOperation(operationId: "Register provider")]
    [OpenApiSecurity("bearer_auth", SecuritySchemeType.Http, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
    [OpenApiRequestBody("application/json", typeof(HistoryRegistration), Required = true)]
    [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.OK)]
    public Task<IActionResult> RegisterProvider([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = Routes.HistoryRegisterProvider)] HttpRequest req) =>
        _reqAuthService.HandledAuthAction(req, async (req, principal) => 
        {
            HistoryRegistration data = await JsonSerializer.DeserializeAsync<HistoryRegistration>(req.Body, new JsonSerializerOptions(JsonSerializerDefaults.Web));
            if (data == null || data.Token == null)
                return new BadRequestObjectResult("Invalid history registration");

            if (await _provider.RegisterProvider(principal.FindFirstValue("user_id"), data.Token, data.Provider))
            {
                return new OkResult();
            }
            else
            {
                return new BadRequestObjectResult("Web server encountered an error");
            }
        });

    [FunctionName("HistoryUnregisterProvider")]
    [OpenApiOperation(operationId: "Register provider")]
    [OpenApiSecurity("bearer_auth", SecuritySchemeType.Http, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
    [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.OK)]
    public Task<IActionResult> UnregisterProvider([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = Routes.HistoryRegisterProvider)] HttpRequest req) =>
        _reqAuthService.HandledAuthAction(req, async (req, principal) => 
        {
            await _provider.UnregisterProvider(principal.FindFirstValue("user_id"));
            return new OkResult();
        });

    [FunctionName("HistoryIsRegistered")]
    [OpenApiOperation(operationId: "Get whether user has registered provider")]
    [OpenApiSecurity("bearer_auth", SecuritySchemeType.Http, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(bool), Description = "Whether provider is registered for user")]
    public Task<IActionResult> IsRegistered([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = Routes.HistoryIsRegistered)] HttpRequest req) =>
        _reqAuthService.HandledAuthAction(req, async (req, principal) => 
        {
            return new OkObjectResult(await _provider.IsRegistered(principal.FindFirstValue("user_id")));
        });
}

