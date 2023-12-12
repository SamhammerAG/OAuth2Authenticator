using System;
using FluentAssertions;
using NUnit.Framework;
using OAuth2Authenticator.Extensions;

namespace OAuth2Authenticator.Tests
{
    public class OAuth2TokenResponseExtensionTest : BaseUnitTest
    {
        [Test]
        public void Successful()
        {
            new OAuth2TokenResponse().Successful().Should().BeTrue();
            new OAuth2TokenResponse {Error = string.Empty}.Successful().Should().BeTrue();

            OAuth2TokenResponse token = null;
            token.Successful().Should().BeFalse();
        }

        [Test]
        public void Valid()
        {
            new OAuth2TokenResponse
            {
                IssueDate = DateTime.Now,
                ExpiresIn = 300
            }.Valid().Should().BeTrue();

            new OAuth2TokenResponse
            {
                IssueDate = DateTime.Now.Subtract(TimeSpan.FromSeconds(10)),
                ExpiresIn = 20
            }.Valid(10).Should().BeFalse();

            new OAuth2TokenResponse
            {
                IssueDate = DateTime.Now.Subtract(TimeSpan.FromMinutes(10)),
                ExpiresIn = 300
            }.Valid().Should().BeFalse();

            new OAuth2TokenResponse().Valid(10).Should().BeFalse();

            OAuth2TokenResponse token = null;
            token.Valid().Should().BeFalse();
        }
    }
}
