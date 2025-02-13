using System;
using System.Threading;
using System.Threading.Tasks;

namespace OAuth2Authenticator
{
    /// <summary>
    /// This class holds common logic which is needed for token handling.
    /// </summary>
    public interface IOAuth2TokenHandler
    {
        /// <summary>
        /// Validates the lifespan of the given toke and tries to refresh it when expired and returns a valid token.
        /// </summary>
        /// <param name="token">Token</param>
        /// <param name="url">Token endpoint URL</param>
        /// <param name="clientId">Client ID</param>
        /// <param name="getNewToken">Function to get a new token.</param>
        /// <param name="threshold">The given threshold in seconds gets removed of the token life span. So the token expires before the actual expiration time.</param>
        /// <param name="cancellationToken">Optional cancellation token</param>
        /// <returns>Valid token or null.</returns>
        Task<T?> RefreshHandler<T>(
            T? token,
            string url,
            string clientId,
            Func<Task<T>> getNewToken,
            string? scope = default,
            int threshold = 10,
            CancellationToken cancellationToken = default) where T : OAuth2TokenResponse?;


        /// <inheritdoc cref="RefreshHandler{T}"/>
        Task<OAuth2TokenResponse?> RefreshHandler(
            OAuth2TokenResponse? token,
            string url,
            string clientId,
            Func<Task<OAuth2TokenResponse>> getNewToken,
            string? scope = default,
            int threshold = 10,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Validates the lifetime of the given token and retrieves a new one when it has expired and returns a valid token.
        /// </summary>
        /// <param name="token">Token</param>
        /// <param name="url">Token endpoint URL</param>
        /// <param name="clientId">Client ID</param>
        /// <param name="clientSecret">Client Secret</param>
        /// <param name="threshold">The given threshold in seconds gets removed of the token life span. So the token expires before the actual expiration time.</param>
        /// <param name="cancellationToken">Optional cancellation token</param>
        /// <returns>Valid token or null.</returns>
        Task<T?> ClientCredentialsHandler<T>(
            T? token,
            string url,
            string clientId,
            string clientSecret,
            string? scope = default,
            int threshold = 10,
            CancellationToken cancellationToken = default) where T : OAuth2TokenResponse?;

        /// <inheritdoc cref="ClientCredentialsHandler{T}"/>
        Task<OAuth2TokenResponse?> ClientCredentialsHandler(
            OAuth2TokenResponse? token,
            string url,
            string clientId,
            string clientSecret,
            string? scope = default,
            int threshold = 10,
            CancellationToken cancellationToken = default);
    }
}
