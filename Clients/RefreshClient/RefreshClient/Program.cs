using IdentityModel;
using IdentityModel.Client;
using RefreshClient;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        // Configure OAuth access Token management
        services.AddAccessTokenManagement(options =>
        {
            options.Client.Clients.Add("sts", new ClientCredentialsTokenRequest
            {
                Address = hostContext.Configuration["Sts:TokenEndpoint"],
                ClientId = hostContext.Configuration["Sts:ClientId"],
                ClientSecret = hostContext.Configuration["Sts:ClientSecret"],
                GrantType = OidcConstants.GrantTypes.ClientCredentials,
                AuthorizationHeaderStyle = BasicAuthenticationHeaderStyle.Rfc6749,
                ClientCredentialStyle = ClientCredentialStyle.AuthorizationHeader,
                Scope = "calc:double",
                Parameters = new Parameters()
                {
                    new KeyValuePair<string, string>("audience", hostContext.Configuration["Api:Audience"])
                }
            });
        });

        // Configure http client
        services.AddHttpClient("client",
                client => { client.BaseAddress = new Uri(hostContext.Configuration["Api:BaseAddress"]); })
            .AddClientAccessTokenHandler("sts");

        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();