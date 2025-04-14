using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http.Json;
using System.Threading;

namespace OAuth2Authenticator
{
    /// <inheritdoc />
    public class OAuth2Authenticator : IOAuth2Authenticator
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<OAuth2Authenticator> _logger;

        public OAuth2Authenticator(
            IHttpClientFactory httpClientFactory,
            ILogger<OAuth2Authenticator> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<T?> PasswordGrant<T>(
            string url,
            string clientId,
            string username,
            string password,
            string? scope = default,
            CancellationToken cancellationToken = default) where T : OAuth2TokenResponse?
        {
            return await RequestToken<T?>(url, clientId, "password", new Dictionary<string, string>
            {
                { "username", username },
                { "password", password }
            }, scope, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<OAuth2TokenResponse?> PasswordGrant(
            string url,
            string clientId,
            string username,
            string password,
            string? scope = default,
            CancellationToken cancellationToken = default)
        {
            return await PasswordGrant<OAuth2TokenResponse?>(url, clientId, username, password, scope, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<T?> RefreshTokenGrant<T>(
            string url,
            string clientId,
            string refreshToken,
            string? scope = default,
            CancellationToken cancellationToken = default) where T : OAuth2TokenResponse?
        {
            return await RequestToken<T>(url, clientId, "refresh_token", new Dictionary<string, string>
            {
                { "refresh_token", refreshToken }
            }, scope, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<OAuth2TokenResponse?> RefreshTokenGrant(
            string url,
            string clientId,
            string refreshToken,
            string? scope = default,
            CancellationToken cancellationToken = default)
        {
            return await RefreshTokenGrant<OAuth2TokenResponse?>(url, clientId, refreshToken, scope, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<T?> ClientCredentialsGrant<T>(
            string url,
            string clientId,
            string clientSecret,
            string? scope = default,
            CancellationToken cancellationToken = default) where T : OAuth2TokenResponse?
        {
            return await RequestToken<T>(url, clientId, "client_credentials", new Dictionary<string, string>
            {
                { "client_secret", clientSecret }
            }, scope, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<OAuth2TokenResponse?> ClientCredentialsGrant(
            string url,
            string clientId,
            string clientSecret,
            string? scope = default,
            CancellationToken cancellationToken = default)
        {
            return await ClientCredentialsGrant<OAuth2TokenResponse?>(url, clientId, clientSecret, scope, cancellationToken);
        }

        private async Task<T?> RequestToken<T>(
            string url,
            string clientId,
            string grant,
            Dictionary<string, string> parameters,
            string? scope = default,
            CancellationToken cancellationToken = default) where T : OAuth2TokenResponse?
        {
            // Create a named client so that it can also be unit tested.
            var client = _httpClientFactory.CreateClient(Options.DefaultName);

            parameters.Add("client_id", clientId);
            parameters.Add("grant_type", grant);

            if (!string.IsNullOrEmpty(scope)) parameters.Add("scope", scope!);

            _logger.LogDebug("Requesting token from {0} for client {1} with the {2} grant type.", url, clientId, grant);

            var response = await client.PostAsync(url, new FormUrlEncodedContent(parameters), cancellationToken);

            try
            {
                await OnResponse(response);

                var token = await response.Content.ReadFromJsonAsync<T?>(cancellationToken: cancellationToken);

                if (token is null)
                {
                    _logger.LogError("Parsed token from {0} is empty!", url);
                    return null;
                }

                token.IssueDate = DateTime.Now;

                return token;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Token response from {0} could not be parsed!", url);
                return null;
            }
        }

        protected virtual async Task OnResponse(HttpResponseMessage? resp)
        {
            if (resp is null)
            {
                _logger.LogError("Token request failed and returned an null response!");
                return;
            }

            if (!resp.IsSuccessStatusCode)
            {
                _logger.LogError("Token request to {0} failed with an {1} response! {2}",
                    resp.RequestMessage?.RequestUri?.ToString(),
                    resp.StatusCode,
                    await resp.Content.ReadAsStringAsync());
            }
        }
    }
}
