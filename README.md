# OAuth2Authenticator
[![ci](https://github.com/SamhammerAG/OAuth2Authenticator/workflows/CI/badge.svg)](https://github.com/SamhammerAG/OAuth2Authenticator)

OAuth2 client for obtaining and refreshing of access tokens.

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

await _authenticator.PasswordGrant(url, clientId, username, password, cancellationToken);

await _authenticator.RefreshTokenGrant(url, clientId, refreshToken);

await _authenticator.ClientCredentialsGrant(url, clientId, clientSecret);
```
After the request, a `OAuth2TokenResponse` or `null` returns.


### OAuth2TokenHandler
This class holds common logic which is needed for token handling. Injectable over the `IOAuth2TokenHandler` interface.
```cs
private readonly IOAuth2TokenHandler _handler;

await _handler.RefreshHandler(token, url, clientId, Func<...> /*To get a new token*/, threshold, cancellationToken));
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
