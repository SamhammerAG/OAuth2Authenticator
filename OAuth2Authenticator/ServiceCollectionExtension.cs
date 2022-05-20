using Microsoft.Extensions.DependencyInjection;

namespace OAuth2Authenticator
{
    public static class ServiceCollectionExtension
    {
        /// <summary>
        /// Adds an HTTP client instance and the authenticator to the services.
        /// </summary>
        public static void InitOAuth2Authenticator(this IServiceCollection services)
        {
            services.AddHttpClient<OAuth2Authenticator>();
            services.AddScoped<IOAuth2Authenticator, OAuth2Authenticator>();
        }
    }
}
