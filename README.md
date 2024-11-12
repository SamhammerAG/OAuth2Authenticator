# OAuth2Authenticator
[![ci](https://github.com/SamhammerAG/OAuth2Authenticator/workflows/CI/badge.svg)](https://github.com/SamhammerAG/OAuth2Authenticator)

OAuth2 client for retrieving OAuth2 tokens and common token handling logic such as refresh and client credentials.

[Nuget Package](https://www.nuget.org/packages/OAuth2Authenticator)

## Currently supported grant types
* [Password](https://oauth.net/2/grant-types/password/)
* [Refresh Token](https://oauth.net/2/grant-types/refresh-token/)
* [Client Credentials](https://oauth.net/2/grant-types/client-credentials/)

## Setup
Initialize the client service in the application startup. 
```cs
public void ConfigureServices(IServiceCollection services)
{
    services.InitOAuth2Authenticator();
}
```

## Usage
### OAuth2Authenticator
This class holds the request logic for all OAuth2 grant types. Injectable over the `IOAuth2Authenticator` interface.
```cs
private readonly IOAuth2Authenticator _authenticator;

await _authenticator.PasswordGrant(url, clientId, username, password);

await _authenticator.RefreshTokenGrant(url, clientId, refreshToken);

await _authenticator.ClientCredentialsGrant(url, clientId, clientSecret);
```
After the request, a `OAuth2TokenResponse` or `null` returns.


### OAuth2TokenHandler
This class holds common logic which is needed for token handling. Injectable over the `IOAuth2TokenHandler` interface.

#### RefreshHandler
The refresh handler checks whether the access token is about to expire or has already expired and automatically attempts to renew the token with the refresh token. If a renewal with the refresh token is not possible, a new token is retrieved via the specified callback. The handler always attempts to return a valid token.
```cs
private readonly IOAuth2TokenHandler _handler;
private static OAuth2TokenResponse token; // Save the last token somewhere static or distributed.

token = await _handler.RefreshHandler(
    token,
    url,
    clientId,
    async (url, clientId, cancellationToken) =>
    {
        // Is executed to obtain a new token if an refresh was not possible or none exists yet.
        return await _authenticator.PasswordGrant(url, clientId, username, password);
    });
```

#### ClientCredentialsHandler
The client credentials handler checks whether the access token is about to expire or has already expired and automatically requests a new token.
```cs
private readonly IOAuth2TokenHandler _handler;
private static OAuth2TokenResponse token; // Save the last token somewhere static or distributed.

OAuth2TokenResponse token;
token = await _handler.ClientCredentialsHandler(
    token,
    url,
    clientId,
    clientSecret);
```

### OAuth2TokenResponseExtension
Checks if the token request was successful.
```cs
token.Successful()
```

Checks that the token is not expired.
```cs
token.Valid()
```


## API Reference
* [OAuth2Authenticator](https://www.fuget.org/packages/OAuth2Authenticator)
