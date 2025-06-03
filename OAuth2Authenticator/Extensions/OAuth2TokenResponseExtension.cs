using System;
using System.Diagnostics.CodeAnalysis;

namespace OAuth2Authenticator.Extensions
{
    public static class OAuth2TokenResponseExtension
    {
        /// <summary>
        /// Checks that the response is present, and no error code is included.
        /// </summary>
        /// <param name="token">Token</param>
        /// <returns>Successful</returns>
        public static bool Successful([NotNullWhen(true)] this OAuth2TokenResponse? token)
        {
            return token != null && string.IsNullOrEmpty(token.Error);
        }

        /// <summary>
        /// Checks that the token is not expired.
        /// </summary>
        /// <param name="token">Token</param>
        /// <param name="threshold">The given threshold in seconds gets removed of the token life span. So the token expires before the actual expiration time.</param>
        /// <returns>Valid</returns>
        public static bool Valid([NotNullWhen(true)] this OAuth2TokenResponse? token, int threshold = 0)
        {
            return token != null && token.IssueDate.AddSeconds(Math.Max(token.ExpiresIn - threshold, 0)) >= DateTime.Now;
        }
    }
}
