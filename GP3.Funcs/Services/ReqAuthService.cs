using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Extensions.JwtCustomHandler;
using Microsoft.Azure.Functions.Extensions.JwtCustomHandler.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace GP3.Funcs.Services
{
    public class ReqAuthService
    {
        /*
         * TODO: Firebase token provider could be replaced with generic token provider 
         * See: gh - kuromukira/azure-functions-jwt-validation-extension
         */
        private readonly IFirebaseTokenProvider _tokenProvider;
        private readonly HashSet<string> _admins;
        private readonly ILogger<ReqAuthService> _logger;

        public ReqAuthService(IConfiguration config, IFirebaseTokenProvider tokenProvider, ILogger<ReqAuthService> logger)
        {
            try
            {
                var en = config.GetSection("Admins").AsEnumerable();
                _admins = en.Select(i => i.Value).ToHashSet();
            }
            catch
            {
                _admins = new ();
            }
            _tokenProvider = tokenProvider;
            _logger = logger;
        }

        public async Task<ClaimsPrincipal> GetPrincipal (HttpRequest req)
        {
            if (req.Headers is null)
                throw new NullReferenceException();
            else if (req.Body is null)
                throw new NullReferenceException();

            var tokenResult = await _tokenProvider.ValidateToken(req);
            if (tokenResult.Status != AccessTokenStatus.Valid)
                return null;

            return tokenResult.Principal;
        }

        public async Task<IActionResult> AuthAction(HttpRequest request, Func<HttpRequest,ClaimsPrincipal,Task<IActionResult>> func)
        {
            var principal = await GetPrincipal(request);
            if (principal == null)
                return new UnauthorizedObjectResult("Invalid access token");
            return await func(request, principal);
        }

        public async Task<IActionResult> HandledAuthAction(HttpRequest request, Func<HttpRequest,ClaimsPrincipal,Task<IActionResult>> func)
        {
            try
            {
                var principal = await GetPrincipal(request);
                if (principal == null)
                    return new UnauthorizedObjectResult("Invalid access token");
                return await func(request, principal);
            }
            catch(Exception e)
            {
                _logger.LogError("Server encountered an error: {0}", e.Message);
                return new BadRequestObjectResult("Web server encountered an error");
            }
        }

        public bool IsAdmin(ClaimsPrincipal principal)
        {
            return _admins.Contains(principal.FindFirstValue("user_id"));
        }
    }
}
