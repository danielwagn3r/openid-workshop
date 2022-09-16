using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace RessourceOwnerPasswordClient;

public class Program
{
    private static IConfiguration _configuration;

    private static async Task Main(string[] args)
    {
        var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", true, true)
            .AddJsonFile($"appsettings.{environment}.json", true, true)
            .AddEnvironmentVariables();

        _configuration = builder.Build();

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .ReadFrom.Configuration(_configuration)
            .CreateLogger();

        var domain = $"https://{_configuration["Auth0:Domain"]}/";

        Log.Debug("Domain: {domain}", domain);

        var disco = await GetDiscoveryResponse(domain);

        var tokenResponse = await GetTokenResponse(disco);

        var refreshToken = tokenResponse.RefreshToken;

        await CallApi(tokenResponse.AccessToken);

        tokenResponse = await RefreshTokenResponse(disco, refreshToken);

        Log.CloseAndFlush();
    }

    private static async Task<DiscoveryDocumentResponse> GetDiscoveryResponse(string domain)
    {
        // discover endpoints from metadata
        var cache = new DiscoveryCache(domain);

        var disco = await cache.GetAsync();
        if (disco.IsError)
        {
            Log.Error("Discovery error: {Error}", disco.Error);
            throw new Exception(disco.Error);
        }

        Log.Information("Issuer: {Issuer}", disco.Issuer);

        return disco;
    }

    private static async Task<TokenResponse> GetTokenResponse(DiscoveryDocumentResponse disco)
    {
        var client = new HttpClient();

        // request token
        var response = await client.RequestPasswordTokenAsync(new PasswordTokenRequest
        {
            Address = disco.TokenEndpoint,

            ClientId = _configuration["Auth0:ClientId"],
            ClientSecret = _configuration["Auth0:ClientSecret"],

            UserName = _configuration["Auth0:Username"],
            Password = _configuration["Auth0:Password"],

            Scope = "openid email profile offline_access calc:double",

            Parameters =
            {
                {"audience", _configuration["CalculatorApi:ApiIdentifier"]}
            }
        });

        if (response.IsError)
        {
            Log.Error("Token error: {Error}\n{Description}", response.Error, response.ErrorDescription);
            throw new Exception(response.Error);
        }

        Log.Information("Access Token: {AccessToken}", response.AccessToken);
        Log.Information("Identity Token: {IdentityToken}", response.IdentityToken);
        Log.Information("Refresh Token: {RefreshToken}", response.RefreshToken);

        return response;
    }

    private static async Task<TokenResponse> RefreshTokenResponse(DiscoveryDocumentResponse disco, string refreshToken)
    {
        var client = new HttpClient();

        // request token
        var response = await client.RequestRefreshTokenAsync(new RefreshTokenRequest
        {
            Address = disco.TokenEndpoint,

            ClientId = _configuration["Auth0:ClientId"],
            ClientSecret = _configuration["Auth0:ClientSecret"],

            RefreshToken = refreshToken
        });

        if (response.IsError)
        {
            Log.Error("Token error: {Error}\n{Description}", response.Error, response.ErrorDescription);
            throw new Exception(response.Error);
        }

        Log.Information("Access Token: {AccessToken}", response.AccessToken);
        Log.Information("Identity Token: {IdentityToken}", response.IdentityToken);
        Log.Information("Refresh Token: {RefreshToken}", response.RefreshToken);

        return response;
    }

    private static async Task CallApi(string token)
    {
        var client = new HttpClient
        {
            BaseAddress = new Uri(_configuration["CalculatorApi:Address"])
        };

        client.SetBearerToken(token);

        var response = await client.GetStringAsync("/api/double/3");

        Log.Information("Response {response}", response);
    }
}