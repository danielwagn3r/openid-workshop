using System;
using Microsoft.AspNetCore.Authorization;

namespace CalculatorApi;
// HasScopeRequirement.cs
//
// From: https://auth0.com/docs/quickstart/backend/aspnet-core-webapi/01-authorization

public class HasScopeRequirement : IAuthorizationRequirement
{
    public HasScopeRequirement(string scope, string issuer)
    {
        Scope = scope ?? throw new ArgumentNullException(nameof(scope));
        Issuer = issuer ?? throw new ArgumentNullException(nameof(issuer));
    }

    public string Issuer { get; }
    public string Scope { get; }
}