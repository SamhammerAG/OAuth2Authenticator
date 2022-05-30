namespace OAuth2Authenticator
{
    /// <summary>
    /// This class holds the OAuth2 error codes constants.
    /// </summary>
    public static class OAuth2ResponseErrors
    {
        // https://datatracker.ietf.org/doc/html/rfc6749#section-5.2

        /// <summary>
        /// The request is missing a required parameter, a value, or is in an incorrect format.
        /// </summary>
        public const string InvalidRequest = "invalid_request";

        /// <summary>
        /// Client authentication failed (e.g. unknown client, no client authentication included, or unsupported authentication method).
        /// </summary>
        public const string InvalidClient = "invalid_client";

        /// <summary>
        /// The provided authorization grant (e.g. credentials) or refresh token is invalid, expired, revoked or was issued to another client.
        /// </summary>
        public const string InvalidGrant = "invalid_grant";

        /// <summary>
        /// The client is not authorized to use the grant type.
        /// </summary>
        public const string UnauthorizedClient = "unauthorized_client";

        /// <summary>
        /// The grant type is not supported by the server.
        /// </summary>
        public const string UnsupportedGrantType = "unsupported_grant_type";
    }
}
