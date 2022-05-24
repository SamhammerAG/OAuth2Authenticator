using System.Threading;
using System.Threading.Tasks;

namespace OAuth2Authenticator
{
    public interface IOAuth2Authenticator
    {
        /// <summary>
        /// Requests a token using the password grant type.
        /// </summary>
        /// <param name="url">Token endpoint URL.</param>
        /// <param name="clientId">Client ID</param>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <param name="cancellationToken">Optional cancellation token</param>
        /// <returns>Token Response</returns>
        Task<T> PasswordGrant<T>(
            string url,
            string clientId,
            string username,
            string password,
            CancellationToken cancellationToken = default) where T : OAuth2TokenResponse;


        /// <summary>
        /// Requests a token using the password grant type.
        /// </summary>
        /// <param name="url">Token endpoint URL.</param>
        /// <param name="clientId">Client ID</param>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <param name="cancellationToken">Optional cancellation token</param>
        /// <returns>Token Response</returns>
        Task<OAuth2TokenResponse> PasswordGrant(
            string url,
            string clientId,
            string username,
            string password,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Requests a new token using the refresh token grant type.
        /// </summary>
        /// <param name="url">Token endpoint URL.</param>
        /// <param name="clientId">Client ID</param>
        /// <param name="refreshToken">Token</param>
        /// <param name="cancellationToken">Optional cancellation token</param>
        /// <returns>Token Response</returns>
        Task<T> RefreshTokenGrant<T>(
            string url,
            string clientId,
            string refreshToken,
            CancellationToken cancellationToken = default) where T : OAuth2TokenResponse;

        /// <summary>
        /// Requests a new token using the refresh token grant type.
        /// </summary>
        /// <param name="url">Token endpoint URL.</param>
        /// <param name="clientId">Client ID</param>
        /// <param name="refreshToken">Token</param>
        /// <param name="cancellationToken">Optional cancellation token</param>
        /// <returns>Token Response</returns>
        Task<OAuth2TokenResponse> RefreshTokenGrant(
            string url,
            string clientId,
            string refreshToken,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Requests a new token using the client credentials grant type.
        /// </summary>
        /// <param name="url">Token endpoint URL.</param>
        /// <param name="clientId">Client ID</param>
        /// <param name="clientSecret">Client Secret</param>
        /// <param name="cancellationToken">Optional cancellation token</param>
        /// <returns>Token Response</returns>
        Task<T> ClientCredentialsGrant<T>(
            string url,
            string clientId,
            string clientSecret,
            CancellationToken cancellationToken = default) where T : OAuth2TokenResponse;

        /// <summary>
        /// Requests a new token using the client credentials grant type.
        /// </summary>
        /// <param name="url">Token endpoint URL.</param>
        /// <param name="clientId">Client ID</param>
        /// <param name="clientSecret">Client Secret</param>
        /// <param name="cancellationToken">Optional cancellation token</param>
        /// <returns>Token Response</returns>
        Task<OAuth2TokenResponse> ClientCredentialsGrant(
            string url,
            string clientId,
            string clientSecret,
            CancellationToken cancellationToken = default);
    }
}
