using System.Threading;
using System.Threading.Tasks;

namespace OAuth2Authenticator
{
    /// <summary>
    /// This class holds the request logic for all OAuth2 grant types.
    /// </summary>
    public interface IOAuth2Authenticator
    {
        /// <summary>
        /// Requests a token using the password grant type.
        /// </summary>
        /// <param name="url">Token endpoint URL.</param>
        /// <param name="clientId">Client ID</param>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <param name="scope">Scope</param>
        /// <param name="cancellationToken">Optional cancellation token</param>
        /// <returns>Token Response</returns>
        Task<T?> PasswordGrant<T>(
            string url,
            string clientId,
            string username,
            string password,
            string? scope = default,
            CancellationToken cancellationToken = default) where T : OAuth2TokenResponse?;

        /// <inheritdoc cref="PasswordGrant{T}"/>
        Task<OAuth2TokenResponse?> PasswordGrant(
            string url,
            string clientId,
            string username,
            string password,
            string? scope = default,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Requests a new token using the refresh token grant type.
        /// </summary>
        /// <param name="url">Token endpoint URL.</param>
        /// <param name="clientId">Client ID</param>
        /// <param name="refreshToken">Token</param>
        /// <param name="scope">Scope</param>
        /// <param name="cancellationToken">Optional cancellation token</param>
        /// <returns>Token Response</returns>
        Task<T?> RefreshTokenGrant<T>(
            string url,
            string clientId,
            string refreshToken,
            string? scope = default,
            CancellationToken cancellationToken = default) where T : OAuth2TokenResponse?;

        /// <inheritdoc cref="RefreshTokenGrant{T}"/>
        Task<OAuth2TokenResponse?> RefreshTokenGrant(
            string url,
            string clientId,
            string refreshToken,
            string? scope = default,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Requests a new token using the client credentials grant type.
        /// </summary>
        /// <param name="url">Token endpoint URL.</param>
        /// <param name="clientId">Client ID</param>
        /// <param name="clientSecret">Client Secret</param>
        /// <param name="scope">Scope</param>
        /// <param name="cancellationToken">Optional cancellation token</param>
        /// <returns>Token Response</returns>
        Task<T?> ClientCredentialsGrant<T>(
            string url,
            string clientId,
            string clientSecret,
            string? scope = default,
            CancellationToken cancellationToken = default) where T : OAuth2TokenResponse?;

        /// <inheritdoc cref="ClientCredentialsGrant{T}"/>
        Task<OAuth2TokenResponse?> ClientCredentialsGrant(
            string url,
            string clientId,
            string clientSecret,
            string? scope = default,
            CancellationToken cancellationToken = default);
    }
}
