using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace OAuth2Authenticator.Tests
{
    public class OAuth2AuthenticatorTest : BaseUnitTest
    {
        private IHttpClientFactory _clientFactory;
        private ILogger<OAuth2Authenticator> _logger;

        private OAuth2Authenticator _oAuth2Authenticator;

        [SetUp]
        public void Setup()
        {
            _clientFactory = A.Fake<IHttpClientFactory>();
            _logger = A.Fake<ILogger<OAuth2Authenticator>>();

            _oAuth2Authenticator = new OAuth2Authenticator(
                _clientFactory,
                _logger);
        }
        
        [Test]
        public async Task PasswordGrant()
        {
            var token = FillObject<OAuth2TokenResponse>();
            var handler = new FakeMessageHandler(HttpStatusCode.OK, JsonSerializer.Serialize(token));

            A.CallTo(() => _clientFactory.CreateClient(A<string>.Ignored))
                .Returns(new HttpClient(handler));

            var response = await _oAuth2Authenticator.PasswordGrant(
                "https://youtu.be/dQw4w9WgXcQ",
                GetRandomString(),
                GetRandomString(),
                GetRandomString());
            
            AssertTokenResponse(token, response);
        }
        
        [Test]
        public async Task RefreshTokenGrant()
        {
            var token = FillObject<OAuth2TokenResponse>();
            var handler = new FakeMessageHandler(HttpStatusCode.OK, JsonSerializer.Serialize(token));
            
            A.CallTo(() => _clientFactory.CreateClient(A<string>.Ignored))
                .Returns(new HttpClient(handler));

            var response = await _oAuth2Authenticator.RefreshTokenGrant(
                "https://youtu.be/dQw4w9WgXcQ",
                GetRandomString(),
                GetRandomString());
            
            AssertTokenResponse(token, response);
        }
        
        [Test]
        public async Task ClientCredentialsGrant()
        {
            var token = FillObject<OAuth2TokenResponse>();
            var handler = new FakeMessageHandler(HttpStatusCode.OK, JsonSerializer.Serialize(token));
            
            A.CallTo(() => _clientFactory.CreateClient(A<string>.Ignored))
                .Returns(new HttpClient(handler));

            var response = await _oAuth2Authenticator.ClientCredentialsGrant(
                "https://youtu.be/dQw4w9WgXcQ",
                GetRandomString(),
                GetRandomString());
            
            AssertTokenResponse(token, response);
        }
        
        [Test]
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

        private void AssertTokenResponse(OAuth2TokenResponse expected, OAuth2TokenResponse actual)
        {
            Assert.AreEqual(expected.AccessToken, actual.AccessToken);
            Assert.AreEqual(expected.TokenType, actual.TokenType);
            Assert.AreEqual(expected.ExpiresIn, actual.ExpiresIn);
            Assert.AreEqual(expected.RefreshToken, actual.RefreshToken);
            Assert.AreEqual(expected.Scope, actual.Scope);
            Assert.IsNotNull(expected.IssueDate);
        }

        private class FakeMessageHandler : HttpMessageHandler
        {
            private readonly HttpStatusCode _responseCode;
            private readonly string _responseContent;
            
            public FakeMessageHandler(HttpStatusCode statusCode, string content)
            {
                _responseCode = statusCode;
                _responseContent = content;
            }
            
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                return Task.FromResult(new HttpResponseMessage
                {
                    StatusCode = _responseCode,
                    Content = new StringContent(_responseContent)
                });
            }
        }
    }
}