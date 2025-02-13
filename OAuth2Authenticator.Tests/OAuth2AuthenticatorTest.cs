using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OAuth2Authenticator.Tests;

[TestClass]
public class OAuth2AuthenticatorTest : BaseUnitTest
{
    private IHttpClientFactory _clientFactory;
    private ILogger<OAuth2Authenticator> _logger;

    private OAuth2Authenticator _oAuth2Authenticator;

    [TestInitialize]
    public void Setup()
    {
        _clientFactory = A.Fake<IHttpClientFactory>();
        _logger = A.Fake<ILogger<OAuth2Authenticator>>();

        _oAuth2Authenticator = new OAuth2Authenticator(
            _clientFactory,
            _logger);
    }

    [TestMethod]
    public async Task PasswordGrant()
    {
        var clientId = GetRandomString();
        var scope = GetRandomString();
        var username = GetRandomString();
        var password = GetRandomString();

        var handler = new FakeMessageHandler(
            HttpStatusCode.OK,
            JsonSerializer.Serialize(FillObject<OAuth2TokenResponse>()),
            (req) =>
            {
                AssertTokenRequest(req, new Dictionary<string, string>
                {
                    { OAuth2RequestParams.GrantType, "password" },
                    { OAuth2RequestParams.ClientId, clientId },
                    { OAuth2RequestParams.Scope, scope },
                    { "username", username },
                    { "password", password },
                });
            });

        A.CallTo(() => _clientFactory.CreateClient(A<string>.Ignored))
         .Returns(new HttpClient(handler));

        var response = await _oAuth2Authenticator.PasswordGrant(
            "https://youtu.be/dQw4w9WgXcQ",
            clientId,
            username,
            password,
            scope);

        Assert.IsNotNull(response);
    }

    [TestMethod]
    public async Task RefreshTokenGrant()
    {
        var clientId = GetRandomString();
        var scope = GetRandomString();
        var refreshToken = GetRandomString();

        var handler = new FakeMessageHandler(
            HttpStatusCode.OK,
            JsonSerializer.Serialize(FillObject<OAuth2TokenResponse>()),
            (req) =>
            {
                AssertTokenRequest(req, new Dictionary<string, string>
                {
                    { OAuth2RequestParams.GrantType, "refresh_token" },
                    { OAuth2RequestParams.ClientId, clientId },
                    { "refresh_token", refreshToken }
                });
            });

        A.CallTo(() => _clientFactory.CreateClient(A<string>.Ignored))
         .Returns(new HttpClient(handler));

        var response = await _oAuth2Authenticator.RefreshTokenGrant(
            "https://youtu.be/dQw4w9WgXcQ",
            clientId,
            refreshToken,
            scope);

        Assert.IsNotNull(response);
    }

    [TestMethod]
    public async Task ClientCredentialsGrant()
    {
        var clientId = GetRandomString();
        var scope = GetRandomString();
        var clientSecret = GetRandomString();

        var handler = new FakeMessageHandler(
            HttpStatusCode.OK,
            JsonSerializer.Serialize(FillObject<OAuth2TokenResponse>()),
            (req) =>
            {
                AssertTokenRequest(req, new Dictionary<string, string>
                {
                    { OAuth2RequestParams.GrantType, "client_credentials" },
                    { OAuth2RequestParams.ClientId, clientId },
                    { "client_secret", clientSecret }
                });
            });

        A.CallTo(() => _clientFactory.CreateClient(A<string>.Ignored))
         .Returns(new HttpClient(handler));

        var response = await _oAuth2Authenticator.ClientCredentialsGrant(
            "https://youtu.be/dQw4w9WgXcQ",
            clientId,
            clientSecret,
            scope);

        Assert.IsNotNull(response);
    }

    [TestMethod]
    public async Task HandleFaultyResponse()
    {
        var handler = new FakeMessageHandler(HttpStatusCode.Forbidden, string.Empty);

        A.CallTo(() => _clientFactory.CreateClient(A<string>.Ignored))
         .Returns(new HttpClient(handler));

        var response = await _oAuth2Authenticator.PasswordGrant(
            "https://youtu.be/dQw4w9WgXcQ",
            GetRandomString(),
            GetRandomString(),
            GetRandomString());

        Assert.IsNull(response);
    }

    [TestMethod]
    public async Task HandleNoneScopedRequest()
    {
        var handler = new FakeMessageHandler(
            HttpStatusCode.OK,
            JsonSerializer.Serialize(FillObject<OAuth2TokenResponse>()),
        (req) =>
        {
                AssertTokenRequest(req, new Dictionary<string, string>
                {
                    { OAuth2RequestParams.Scope, null }
                });
            });

        A.CallTo(() => _clientFactory.CreateClient(A<string>.Ignored))
         .Returns(new HttpClient(handler));

        await _oAuth2Authenticator.PasswordGrant(
            "https://youtu.be/dQw4w9WgXcQ",
            GetRandomString(),
            GetRandomString(),
            GetRandomString());
    }

    private static void AssertTokenRequest(
        HttpRequestMessage req,
        Dictionary<string, string> parameters)
    {
        var query = HttpUtility.ParseQueryString(req.Content.ReadAsStringAsync().Result);

        foreach (var x in parameters)
        {
            Assert.AreEqual(x.Value, query.Get(x.Key));
        }
    }

    private class OAuth2RequestParams
    {
        public const string ClientId = "client_id";
        public const string GrantType = "grant_type";
        public const string Scope = "scope";
    }

    private class FakeMessageHandler : HttpMessageHandler
    {
        private readonly HttpStatusCode _responseCode;
        private readonly string _responseContent;
        private readonly Action<HttpRequestMessage>? _onRequest;

        public FakeMessageHandler(HttpStatusCode statusCode, string content, Action<HttpRequestMessage>? onRequest = default)
        {
            _responseCode = statusCode;
            _responseContent = content;
            _onRequest = onRequest;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (_onRequest is not null) _onRequest(request);

            return Task.FromResult(new HttpResponseMessage
            {
                StatusCode = _responseCode,
                Content = new StringContent(_responseContent)
            });
        }
    }
}
