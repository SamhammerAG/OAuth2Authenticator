# OAuth2Authenticator
[![ci](https://github.com/SamhammerAG/OAuth2Authenticator/workflows/CI/badge.svg)](https://github.com/SamhammerAG/OAuth2Authenticator)

OAuth2Authenticator is an OAuth client to simplify the process of retrieving and managing OAuth tokens. It supports multiple grant types and provides built-in handlers for token refresh and expiration.

[Nuget Package](https://www.nuget.org/packages/OAuth2Authenticator)

## Currently supported grant types
* [Password](https://oauth.net/2/grant-types/password/)
* [Refresh Token](https://oauth.net/2/grant-types/refresh-token/)
* [Client Credentials](https://oauth.net/2/grant-types/client-credentials/)

## Setup
### Installation
Install via NuGet:
```bash
dotnet add package OAuth2Authenticator
```

### Initialize
Initialize the client service in the application startup. 
```cs
public void ConfigureServices(IServiceCollection services)
{
    services.AddOAuth2Authenticator();
}
```

## Usage
### OAuth2Authenticator
This class holds the request logic for all OAuth2 grant types. Injectable over the `IOAuth2Authenticator` interface.
```cs
private readonly IOAuth2Authenticator _authenticator; // Retrieve this service via dependency injection.

await _authenticator.PasswordGrant(url, clientId, username, password);

await _authenticator.RefreshTokenGrant(url, clientId, refreshToken);

await _authenticator.ClientCredentialsGrant(url, clientId, clientSecret);
```
Returns an `OAuth2TokenResponse` or `null`. Usage examples for manual token handling can be found in the [OAuth2TokenHandler.cs](OAuth2Authenticator/OAuth2TokenHandler.cs).


### OAuth2TokenHandler
This class holds common logic which is needed for token handling. Injectable over the `IOAuth2TokenHandler` interface.

#### RefreshHandler
The refresh handler checks whether the access token is about to expire or has already expired and automatically attempts to renew the token with the refresh token. If a renewal with the refresh token is not possible, a new token is retrieved via the specified callback. The handler always attempts to return a valid token.
```cs
private readonly IOAuth2TokenHandler _handler; // Retrieve this service via dependency injection.
private static OAuth2TokenResponse token; // Save the last token somewhere static or distributed.

token = await _handler.RefreshHandler(
    token,
    url,
    clientId,
    async () =>
    {
        // Executed to obtain a new token if an refresh was not possible or none exists yet.
        return await _authenticator.PasswordGrant(url, clientId, username, password);
    });
```
Returns an `OAuth2TokenResponse` or `null`.

#### ClientCredentialsHandler
The client credentials handler checks whether the access token is about to expire or has already expired and automatically requests a new token.
```cs
private readonly IOAuth2TokenHandler _handler; // Retrieve this service via dependency injection.
private static OAuth2TokenResponse token; // Save the last token somewhere static or distributed.

token = await _handler.ClientCredentialsHandler(
    token,
    url,
    clientId,
    clientSecret);
```
Returns an `OAuth2TokenResponse` or `null`.

### OAuth2TokenResponseExtension
Checks if the token request was successful.
```cs
token.Successful()
```

Checks that the token is not expired.
```cs
token.Valid()
```

# License
[Apache 2.0](LICENSE)
