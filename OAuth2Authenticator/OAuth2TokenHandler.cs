using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace OAuth2Authenticator
{
    public class OAuth2TokenHandler : IOAuth2TokenHandler
    {
        private readonly IOAuth2Authenticator _authenticator;
        private readonly ILogger<OAuth2TokenHandler> _logger;

        public OAuth2TokenHandler(
            IOAuth2Authenticator authenticator,
            ILogger<OAuth2TokenHandler> logger)
        {
            _authenticator = authenticator;
            _logger = logger;
        }

        public async Task<T> RefreshHandler<T>(
            T token,
            string url,
            string clientId,
            Func<string, string, CancellationToken, Task<T>> getNewToken,
            int threshold = 10,
            CancellationToken cancellationToken = default) where T : OAuth2TokenResponse
        {
            if (token != null && token.Valid(threshold))
            {
                return token;
            }

            T response;

            if (token is null || string.IsNullOrWhiteSpace(token.AccessToken))
            {
                response = await getNewToken(url, clientId, cancellationToken);
            }
            else
            {
                response = await _authenticator.RefreshTokenGrant<T>(url, clientId, token.RefreshToken, cancellationToken);
            }

            // Refresh token is invalid, expired or revoked.
            // https://tools.ietf.org/html/rfc6749#section-5.2
            if (response?.Error == OAuth2ResponseErrors.InvalidGrant)
            {
                response = await getNewToken(url, clientId, cancellationToken);
            }

            if (response.Successful())
            {
                return response;
            }

            _logger.LogError("Token request failed with unexpected error! Error: {error}", response?.Error);
            return null;
        }

        public async Task<OAuth2TokenResponse> RefreshHandler(
            OAuth2TokenResponse token,
            string url,
            string clientId,
            Func<string, string, CancellationToken, Task<OAuth2TokenResponse>> getNewToken,
            int threshold = 10,
            CancellationToken cancellationToken = default)
        {
            return await RefreshHandler<OAuth2TokenResponse>(
                token,
                url,
                clientId,
                getNewToken,
                threshold,
                cancellationToken);
        }
    }
}
