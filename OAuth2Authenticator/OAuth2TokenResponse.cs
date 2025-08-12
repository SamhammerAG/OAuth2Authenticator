using System.Text.Json.Serialization;
using Newtonsoft.Json;
using System;

namespace OAuth2Authenticator
{
    /// <summary>
    /// OAuth2 token response model.
    /// </summary>
    public class OAuth2TokenResponse
    {
        #region Success
        // https://datatracker.ietf.org/doc/html/rfc6749#section-5.1

        [JsonProperty("access_token")]
        [JsonPropertyName("access_token")]
        public virtual string AccessToken { get; set; }

        [JsonProperty("token_type")]
        [JsonPropertyName("token_type")]
        public virtual string TokenType { get; set; }

        [JsonProperty("expires_in")]
        [JsonPropertyName("expires_in")]
        public virtual int ExpiresIn { get; set; }

        /// <summary>
        /// Optionally
        /// </summary>
        [JsonProperty("refresh_token")]
        [JsonPropertyName("refresh_token")]
        public virtual string RefreshToken { get; set; }

        /// <summary>
        /// Optionally
        /// </summary>
        [JsonProperty("scope")]
        [JsonPropertyName("scope")]
        public virtual string Scope { get; set; }

        #endregion

        #region Error
        // https://datatracker.ietf.org/doc/html/rfc6749#section-5.2

        [JsonProperty("error")]
        [JsonPropertyName("error")]
        public virtual string Error { get; set; }

        /// <summary>
        /// Optionally
        /// </summary>
        [JsonProperty("error_description")]
        [JsonPropertyName("error_description")]
        public virtual string ErrorDescription { get; set; }

        /// <summary>
        /// Optionally
        /// </summary>
        [JsonProperty("error_uri")]
        [JsonPropertyName("error_uri")]
        public virtual string ErrorUri { get; set; }

        #endregion

        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual DateTime IssueDate { get; set; }
    }
}
