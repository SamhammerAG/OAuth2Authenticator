using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace OAuth2Authenticator.Internal
{
    internal class HandlerAuthenticator : OAuth2Authenticator, IHandlerAuthenticator
    {
        public HandlerAuthenticator(IHttpClientFactory httpClientFactory, ILogger<OAuth2Authenticator> logger) : base(httpClientFactory, logger)
        {
        }

        protected override async Task OnResponse(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.BadRequest)
            {
                await base.OnResponse(response);
            }
        }
    }
}
