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
        /// <param name="url">Token endpoint URL.</param>
        /// <param name="clientId">Client ID</param>
        /// <param name="getNewToken">Function to get a new token. Arguments: URL, Client ID, Cancellation Token</param>
        /// <param name="threshold">The given threshold in seconds gets removed of the token life span. So the token expires before the actual expiration time.</param>
        /// <param name="cancellationToken">Optional cancellation token</param>
        /// <returns>Valid token or null.</returns>
        Task<T> RefreshHandler<T>(
            T token,
            string url,
            string clientId,
            Func<string, string, CancellationToken, Task<T>> getNewToken,
            int threshold = 10,
            CancellationToken cancellationToken = default) where T : OAuth2TokenResponse;


        /// <inheritdoc cref="RefreshHandler{T}"/>
        Task<OAuth2TokenResponse> RefreshHandler(
            OAuth2TokenResponse token,
            string url,
            string clientId,
            Func<string, string, CancellationToken, Task<OAuth2TokenResponse>> getNewToken,
            int threshold = 10,
            CancellationToken cancellationToken = default);
    }
}
