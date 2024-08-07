using System;
using AutoFixture;

namespace OAuth2Authenticator.Tests;

public abstract class BaseUnitTest
{
    protected T FillObject<T>()
    {
        return new Fixture().Create<T>();
    }

    protected string GetRandomString() => Guid.NewGuid().ToString();
}
