using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OAuth2Authenticator.Extensions;
using OAuth2Authenticator.Internal;

namespace OAuth2Authenticator
{
    /// <inheritdoc />
    public class OAuth2TokenHandler : IOAuth2TokenHandler
    {
        private readonly IHandlerAuthenticator _authenticator;
        private readonly ILogger<OAuth2TokenHandler> _logger;

        public OAuth2TokenHandler(
            IHandlerAuthenticator authenticator,
            ILogger<OAuth2TokenHandler> logger)
        {
            _authenticator = authenticator;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<T?> RefreshHandler<T>(
            T? token,
            string url,
            string clientId,
            Func<Task<T>> getNewToken,
            string? scope = default,
            int threshold = 10,
            CancellationToken cancellationToken = default) where T : OAuth2TokenResponse?
        {
            if (token != null && token.Valid(threshold))
            {
                return token;
            }

            T? resp;

            if (token is null || string.IsNullOrWhiteSpace(token.AccessToken))
            {
                resp = await getNewToken();
            }
            else
            {
                resp = await _authenticator.RefreshTokenGrant<T?>(url, clientId, token.RefreshToken, scope, cancellationToken);
            }

            // Refresh token is invalid, expired or revoked.
            // https://tools.ietf.org/html/rfc6749#section-5.2
            if (resp?.Error == OAuth2ResponseErrors.InvalidGrant)
            {
                resp = await getNewToken();
            }

            if (!resp.Successful())
            {
                _logger.LogError("Token request failed with unexpected error! Error: {0}", resp?.Error);
                return null;
            }

            return resp;
        }

        /// <inheritdoc />
        public async Task<OAuth2TokenResponse?> RefreshHandler(
            OAuth2TokenResponse? token,
            string url,
            string clientId,
            Func<Task<OAuth2TokenResponse>> getNewToken,
            string? scope = default,
            int threshold = 10,
            CancellationToken cancellationToken = default)
        {
            return await RefreshHandler<OAuth2TokenResponse>(
                token,
                url,
                clientId,
                getNewToken,
                scope,
                threshold,
                cancellationToken);
        }

        /// <inheritdoc />
        public async Task<T?> ClientCredentialsHandler<T>(
            T? token,
            string url,
            string clientId,
            string clientSecret,
            string? scope = default,
            int threshold = 10,
            CancellationToken cancellationToken = default) where T : OAuth2TokenResponse?
        {
            if (token is not null && token.Valid(threshold)) return token;

            T? resp = await _authenticator.ClientCredentialsGrant<T?>(url, clientId, clientSecret, scope, cancellationToken);

            if (!resp.Successful())
            {
                _logger.LogError("Token request failed with unexpected error! Error: {0}", resp?.Error);
                return null;
            }

            return resp;
        }

        /// <inheritdoc />
        public async Task<OAuth2TokenResponse?> ClientCredentialsHandler(
            OAuth2TokenResponse? token,
            string url,
            string clientId,
            string clientSecret,
            string? scope = default,
            int threshold = 10,
            CancellationToken cancellationToken = default)
        {
            return await ClientCredentialsHandler<OAuth2TokenResponse>(
                token,
                url,
                clientId,
                clientSecret,
                scope,
                threshold,
                cancellationToken);
        }
    }
}
