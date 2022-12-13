using GP3.Client.Services;
using System.Net.Http.Headers;

namespace GP3.Client.Refit
{
    public class AuthMessageHandler : DelegatingHandler
    {
        private readonly AuthService _auth;

        public AuthMessageHandler(AuthService auth)
        {
            _auth = auth;
        }

        protected override async Task <HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancelToken)
        {
            HttpRequestHeaders headers = request.Headers;

            AuthenticationHeaderValue authHeader = headers.Authorization;

            if(authHeader != null)
            {
                var token = await _auth.GetToken();
                headers.Authorization = new AuthenticationHeaderValue(authHeader.Scheme, token);
            }

            return await base.SendAsync(request, cancelToken);
        }
    }
}
