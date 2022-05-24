using System;
using NUnit.Framework;

namespace OAuth2Authenticator.Tests
{
    public class OAuth2TokenResponseExtensionTest : BaseUnitTest
    {
        [Test]
        public void Successful()
        {
            Assert.IsTrue(new OAuth2TokenResponse().Successful());
            Assert.IsTrue(new OAuth2TokenResponse { Error = string.Empty }.Successful());

            OAuth2TokenResponse token = null;
            Assert.IsFalse(token.Successful());
        }

        [Test]
        public void Valid()
        {
            Assert.IsTrue(new OAuth2TokenResponse
            {
                IssueDate = DateTime.Now,
                ExpiresIn = 300
            }.Valid());

            Assert.IsFalse(new OAuth2TokenResponse
            {
                IssueDate = DateTime.Now.Subtract(TimeSpan.FromSeconds(10)),
                ExpiresIn = 20
            }.Valid(10));

            Assert.IsFalse(new OAuth2TokenResponse
            {
                IssueDate = DateTime.Now.Subtract(TimeSpan.FromMinutes(10)),
                ExpiresIn = 300
            }.Valid());

            Assert.IsFalse(new OAuth2TokenResponse().Valid(10));
        }
    }
}
