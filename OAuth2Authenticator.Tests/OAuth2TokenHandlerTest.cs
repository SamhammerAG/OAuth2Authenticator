using System;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace OAuth2Authenticator.Tests
{
    public class OAuth2TokenHandlerTest : BaseUnitTest
    {
        private IOAuth2Authenticator _authenticator;
        private ILogger<OAuth2TokenHandler> _logger;

        private OAuth2TokenHandler _handler;

        [SetUp]
        public void Setup()
        {
            _authenticator = A.Fake<IOAuth2Authenticator>();
            _logger = A.Fake<ILogger<OAuth2TokenHandler>>();

            _handler = new OAuth2TokenHandler(
                _authenticator,
                _logger);
        }

        [Test]
        public async Task RefreshHandlerValidToken()
        {
            var token = FillObject<OAuth2TokenResponse>();
            token.IssueDate = DateTime.Now;
            token.ExpiresIn = 300;

            var result = await RefreshHandler(token);

            Assert.AreEqual(token, result);
        }

        [Test]
        public async Task RefreshHandlerEmptyToken()
        {
            var result = await RefreshHandler(null);

            A.CallTo(() => _authenticator.PasswordGrant(
                A<string>.Ignored,
                A<string>.Ignored,
                A<string>.Ignored,
                A<string>.Ignored,
                A<CancellationToken>.Ignored)).MustHaveHappened();

            Assert.IsNotNull(result);
        }

        [Test]
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
                A<CancellationToken>.Ignored)).MustHaveHappened();

            Assert.IsNotNull(token);
            Assert.AreNotEqual(result.AccessToken, token.AccessToken);
        }

        [Test]
        public async Task RefreshHandlerInvalidGrantError()
        {
            var token = FillObject<OAuth2TokenResponse>();
            token.IssueDate = DateTime.Now.Subtract(TimeSpan.FromMinutes(10));
            token.ExpiresIn = 300;

            A.CallTo(() => _authenticator.RefreshTokenGrant<OAuth2TokenResponse>(
                A<string>.Ignored,
                A<string>.Ignored,
                token.RefreshToken,
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
                A<CancellationToken>.Ignored)).MustHaveHappened();

            Assert.IsNotNull(result);
            Assert.AreNotEqual(result.AccessToken, token.AccessToken);
        }

        [Test]
        public async Task RefreshHandlerError()
        {
            var token = FillObject<OAuth2TokenResponse>();
            token.IssueDate = DateTime.Now.Subtract(TimeSpan.FromMinutes(10));
            token.ExpiresIn = 300;

            A.CallTo(() => _authenticator.RefreshTokenGrant<OAuth2TokenResponse>(
                A<string>.Ignored,
                A<string>.Ignored,
                token.RefreshToken,
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
                async (url, clientId, cancellationToken) =>
                {
                    return await _authenticator.PasswordGrant(
                        GetRandomString(),
                        GetRandomString(),
                        GetRandomString(),
                        GetRandomString(),
                        cancellationToken);
                });
        }
    }
}
