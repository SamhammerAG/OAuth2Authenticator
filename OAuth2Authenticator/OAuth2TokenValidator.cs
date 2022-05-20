using System;

namespace OAuth2Authenticator
{
    public static class OAuth2TokenValidator
    {
        /// <summary>
        /// Validates that the token is not expired.
        /// </summary>
        /// <param name="token">Token</param>
        /// <param name="threshold">The given threshold in seconds gets removed of the token life span. So the token expires before the actual expiration time.</param>
        /// <returns>Token validity</returns>
        public static bool ValidateExpiry(OAuth2TokenResponse token, int threshold = 0)
        {
            return ValidateExpiry(token.IssueDate, token.ExpiresIn, threshold);
        }

        /// <summary>
        /// Validates that the token is not expired.
        /// </summary>
        /// <param name="issueDate">Issue date of the Token.</param>
        /// <param name="expiresIn">Expires in value.</param>
        /// <param name="threshold">The given threshold in seconds gets removed of the token life span. So the token expires before the actual expiration time.</param>
        /// <returns>Token validity</returns>
        public static bool ValidateExpiry(DateTime issueDate, int expiresIn, int threshold = 0)
        {
            return issueDate.AddSeconds(expiresIn - threshold) >= DateTime.Now;
        }
    }
}
