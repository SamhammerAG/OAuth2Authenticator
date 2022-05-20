using System;
using NUnit.Framework;

namespace OAuth2Authenticator.Tests
{
    public class OAuth2TokenValidatorTest : BaseUnitTest
    {
        [Test]
        public void ValidateExpiry()
        {
            Assert.IsTrue(OAuth2TokenValidator.ValidateExpiry(DateTime.Now, 10));
            Assert.IsTrue(OAuth2TokenValidator.ValidateExpiry(DateTime.Now, 20, 10));
            Assert.IsFalse(OAuth2TokenValidator.ValidateExpiry(new DateTime(), 10));
        }
    }
}