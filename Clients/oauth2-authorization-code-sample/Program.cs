using Serilog;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());

    builder.Services.AddRazorPages();

    JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

    builder.Services
        .AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = "cookie";
            options.DefaultSignInScheme = "cookie";
            options.DefaultChallengeScheme = "oidc";
        })
        .AddCookie("cookie", options =>
        {
            options.Cookie.HttpOnly = true;
            //options.Cookie.SameSite = SameSiteMode.Strict;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.Cookie.HttpOnly = true;

            // add an instance of the patched manager to the options:
            options.CookieManager = new ChunkingCookieManager();
        }
        )
        .AddOpenIdConnect("oidc", options =>
        {
            // Set the authority to your Auth0 domain
            options.Authority = builder.Configuration["Sts:Authority"];

            // Configure the Auth0 Client ID and Client Secret
            options.ClientId = builder.Configuration["Sts:ClientId"];
            options.ClientSecret = builder.Configuration["Sts:ClientSecret"];

            // Configure resource
            options.Resource = builder.Configuration["Api:Audience"];

            options.Events = new OpenIdConnectEvents
            {
                // Necessary to tell Auth0 we don't want an opaque access token
                OnRedirectToIdentityProvider = context =>
                {
                    context.ProtocolMessage.SetParameter("audience", "http://calculator-api");
                    return Task.CompletedTask;
                },

                // Necessary to enable RP-initiated logout (Auth0 Enterprise feature)
                OnRedirectToIdentityProviderForSignOut = (context) =>
                {
                    var logoutUri = $"{builder.Configuration["Sts:Authority"]}/oidc/logout?client_id={builder.Configuration["Sts:ClientId"]}";

                    var postLogoutUri = context.Properties.RedirectUri;
                    if (!string.IsNullOrEmpty(postLogoutUri))
                    {
                        if (postLogoutUri.StartsWith("/"))
                        {
                            // transform to absolute
                            var request = context.Request;
                            postLogoutUri = request.Scheme + "://" + request.Host + request.PathBase + postLogoutUri;
                        }
                        logoutUri += $"&returnTo={Uri.EscapeDataString(postLogoutUri)}";
                    }

                    context.Response.Redirect(logoutUri);
                    context.HandleResponse();

                    return Task.CompletedTask;
                }
            };

            // Set response type to code
            options.ResponseType = "code";
            //options.UsePkce = true;

            options.NonceCookie.SecurePolicy = CookieSecurePolicy.Always;
            options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;

            options.Scope.Clear();
            options.Scope.Add("openid");
            options.Scope.Add("email");
            options.Scope.Add("profile");
            options.Scope.Add("calc:double");
            options.Scope.Add("offline_access");
            options.GetClaimsFromUserInfoEndpoint = true;

            options.SaveTokens = true;

            // Default will work
            //options.CallbackPath = "/signin-auth0";

            // Set the correct name claim type
            options.TokenValidationParameters = new TokenValidationParameters
            {
                NameClaimType = "name"
            };
        });

    var app = builder.Build();

    // Configure Serilog request logging.
    app.UseSerilogRequestLogging();

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapRazorPages().RequireAuthorization();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "An unhandled exception occured during bootstrapping");
}
finally
{
    Log.CloseAndFlush();
}
