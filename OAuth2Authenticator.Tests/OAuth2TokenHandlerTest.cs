using System;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OAuth2Authenticator.Internal;

namespace OAuth2Authenticator.Tests;

[TestClass]
public class OAuth2TokenHandlerTest : BaseUnitTest
{
    private IHandlerAuthenticator _authenticator;
    private ILogger<OAuth2TokenHandler> _logger;

    private OAuth2TokenHandler _handler;

    [TestInitialize]
    public void Setup()
    {
        _authenticator = A.Fake<IHandlerAuthenticator>();
        _logger = A.Fake<ILogger<OAuth2TokenHandler>>();

        _handler = new OAuth2TokenHandler(
            _authenticator,
            _logger);
    }

    [TestMethod]
    public async Task RefreshHandlerValidToken()
    {
        var token = FillObject<OAuth2TokenResponse>();
        token.IssueDate = DateTime.Now;
        token.ExpiresIn = 300;

        var result = await RefreshHandler(token);

        Assert.AreEqual(token, result);
    }

    [TestMethod]
    public async Task RefreshHandlerEmptyToken()
    {
        var result = await RefreshHandler(new OAuth2TokenResponse());

        A.CallTo(() => _authenticator.PasswordGrant(
            A<string>.Ignored,
            A<string>.Ignored,
            A<string>.Ignored,
            A<string>.Ignored,
            A<string>.Ignored,
            A<CancellationToken>.Ignored)).MustHaveHappened();

        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task RefreshHandlerRefreshToken()
    {
        var token = FillObject<OAuth2TokenResponse>();
        token.IssueDate = DateTime.Now.Subtract(TimeSpan.FromMinutes(10));
        token.ExpiresIn = 300;

        var result = await RefreshHandler(token);

        A.CallTo(() => _authenticator.RefreshTokenGrant<OAuth2TokenResponse>(
            A<string>.Ignored,
            A<string>.Ignored,
            token.RefreshToken,
            A<string>.Ignored,
            A<CancellationToken>.Ignored)).MustHaveHappened();

        Assert.IsNotNull(token);
        Assert.AreNotEqual(result.AccessToken, token.AccessToken);
    }

    [TestMethod]
    public async Task RefreshHandlerInvalidGrantError()
    {
        var token = FillObject<OAuth2TokenResponse>();
        token.IssueDate = DateTime.Now.Subtract(TimeSpan.FromMinutes(10));
        token.ExpiresIn = 300;

        A.CallTo(() => _authenticator.RefreshTokenGrant<OAuth2TokenResponse>(
            A<string>.Ignored,
            A<string>.Ignored,
            token.RefreshToken,
            A<string>.Ignored,
            A<CancellationToken>.Ignored)).Returns(new OAuth2TokenResponse
        {
            Error = OAuth2ResponseErrors.InvalidGrant
        });

        var result = await RefreshHandler(token);

        A.CallTo(() => _authenticator.PasswordGrant(
            A<string>.Ignored,
            A<string>.Ignored,
            A<string>.Ignored,
            A<string>.Ignored,
            A<string>.Ignored,
            A<CancellationToken>.Ignored)).MustHaveHappened();

        Assert.IsNotNull(result);
        Assert.AreNotEqual(result.AccessToken, token.AccessToken);
    }

    [TestMethod]
    public async Task RefreshHandlerError()
    {
        var token = FillObject<OAuth2TokenResponse>();
        token.IssueDate = DateTime.Now.Subtract(TimeSpan.FromMinutes(10));
        token.ExpiresIn = 300;

        A.CallTo(() => _authenticator.RefreshTokenGrant<OAuth2TokenResponse>(
            A<string>.Ignored,
            A<string>.Ignored,
            token.RefreshToken,
            A<string>.Ignored,
            A<CancellationToken>.Ignored)).Returns(new OAuth2TokenResponse
        {
            Error = GetRandomString()
        });

        var result = await RefreshHandler(token);

        Assert.IsNull(result);
    }

    private async Task<OAuth2TokenResponse> RefreshHandler(OAuth2TokenResponse token)
    {
        return await _handler.RefreshHandler(
            token,
            "https://youtu.be/dQw4w9WgXcQ",
            GetRandomString(),
            async () =>
            {
                return await _authenticator.PasswordGrant(
                    GetRandomString(),
                    GetRandomString(),
                    GetRandomString(),
                    GetRandomString(),
                    GetRandomString());
            });
    }

    [TestMethod]
    public async Task ClientCredentialsHandlerValidToken()
    {
        var token = FillObject<OAuth2TokenResponse>();
        token.IssueDate = DateTime.Now;
        token.ExpiresIn = 300;

        var result = await ClientCredentialsHandler(token);

        Assert.AreEqual(token, result);
    }

    [TestMethod]
    public async Task ClientCredentialsHandlerEmptyToken()
    {
        var result = await ClientCredentialsHandler(new OAuth2TokenResponse());

        A.CallTo(() => _authenticator.ClientCredentialsGrant<OAuth2TokenResponse>(
            A<string>.Ignored,
            A<string>.Ignored,
            A<string>.Ignored,
            A<string>.Ignored,
            A<CancellationToken>.Ignored)).MustHaveHappened();

        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task ClientCredentialsHandlerNewToken()
    {
        var token = FillObject<OAuth2TokenResponse>();
        token.IssueDate = DateTime.Now.Subtract(TimeSpan.FromMinutes(10));
        token.ExpiresIn = 300;

        var clientId = GetRandomString();
        var clientSecret = GetRandomString();

        var result = await ClientCredentialsHandler(token, clientId, clientSecret);

        A.CallTo(() => _authenticator.ClientCredentialsGrant<OAuth2TokenResponse>(
            A<string>.Ignored,
            clientId,
            clientSecret,
            A<string>.Ignored,
            A<CancellationToken>.Ignored)).MustHaveHappened();

        Assert.IsNotNull(token);
        Assert.AreNotEqual(result.AccessToken, token.AccessToken);
    }

    [TestMethod]
    public async Task ClientCredentialsHandlerError()
    {
        var token = FillObject<OAuth2TokenResponse>();
        token.IssueDate = DateTime.Now.Subtract(TimeSpan.FromMinutes(10));
        token.ExpiresIn = 300;

        var clientId = GetRandomString();
        var clientSecret = GetRandomString();

        A.CallTo(() => _authenticator.ClientCredentialsGrant<OAuth2TokenResponse>(
            A<string>.Ignored,
            clientId,
            clientSecret,
            A<string>.Ignored,
            A<CancellationToken>.Ignored)).Returns(new OAuth2TokenResponse
            {
                Error = GetRandomString()
            });

        var result = await ClientCredentialsHandler(token, clientId, clientSecret);

        Assert.IsNull(result);
    }

    private async Task<OAuth2TokenResponse> ClientCredentialsHandler(
        OAuth2TokenResponse token, string clientId = default, string clientSecret = default)
    {
        return await _handler.ClientCredentialsHandler(
            token,
            "https://youtu.be/q2RQyrp6j_A",
            clientId ?? GetRandomString(),
            clientSecret ?? GetRandomString());
    }
}
