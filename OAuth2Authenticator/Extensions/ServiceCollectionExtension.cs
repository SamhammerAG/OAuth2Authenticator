using Microsoft.Extensions.DependencyInjection;
using OAuth2Authenticator.Internal;

namespace OAuth2Authenticator.Extensions
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
            services.AddScoped<IHandlerAuthenticator, HandlerAuthenticator>();
            services.AddScoped<IOAuth2TokenHandler, OAuth2TokenHandler>();
        }
    }
}
