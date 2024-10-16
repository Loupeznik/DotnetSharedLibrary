using DZarsky.CommonLibraries.AzureFunctions.Configuration;
using DZarsky.CommonLibraries.AzureFunctions.Extensions;
using DZarsky.CommonLibraries.AzureFunctions.General;
using DZarsky.CommonLibraries.AzureFunctions.Models.Auth;
using DZarsky.CommonLibraries.AzureFunctions.Security;
using DZarsky.CommonLibraries.AzureFunctions.Security.CosmosDB;
using DZarsky.CommonLibraries.AzureFunctions.Security.Zitadel;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;
using System.Text;

namespace DZarsky.CommonLibraries.AzureFunctions.Infrastructure;

public static class AzureFunctionsExtensions
{
    public static void AddCommonFunctionServices(this IServiceCollection services, IConfiguration configuration,
        AuthType? authType = AuthType.Standalone, bool? instrumentCosmosDb = true)
    {
        var cosmosConfig = new CosmosConfiguration();

        if (instrumentCosmosDb.GetValueOrDefault())
        {
            services.AddSingleton((s) =>
            {
                var endpoint = configuration.GetValueFromContainer<string>("CosmosDB.Endpoint");

                if (string.IsNullOrWhiteSpace(endpoint))
                {
                    throw new ArgumentException("CosmosDB endpoint was not set");
                }

                var authKey = configuration.GetValueFromContainer<string>("CosmosDB.AuthorizationKey");

                if (string.IsNullOrWhiteSpace(authKey))
                {
                    throw new ArgumentException("CosmosDB authorization key was not set");
                }

                var configurationBuilder = new CosmosClientBuilder(endpoint, authKey);

                return configurationBuilder
                    .Build();
            });

            cosmosConfig.DatabaseID = configuration.GetValueFromContainer<string>("CosmosDB.DatabaseID");
            cosmosConfig.UsersContainerID = configuration.GetValueFromContainer<string>("CosmosDB.UserContainerID");

            if (string.IsNullOrWhiteSpace(cosmosConfig.DatabaseID))
            {
                throw new ArgumentException("Could not set CosmosConfiguration - DatabaseID is missing");
            }

            services.AddSingleton(cosmosConfig);
        }

        if (authType == AuthType.Standalone)
        {
            if (!instrumentCosmosDb.GetValueOrDefault())
            {
                throw new ArgumentException("Cannot set standalone authentication without CosmosDB instrumentation");
            }

            if (string.IsNullOrWhiteSpace(cosmosConfig.UsersContainerID))
            {
                throw new ArgumentException("Could not set CosmosConfiguration - UsersContainerID is missing");
            }

            var authConfig = new AuthConfiguration
            {
                ArgonSecret = configuration.GetValueFromContainer<string>("ArgonSecret")
            };

            if (string.IsNullOrWhiteSpace(authConfig.ArgonSecret))
            {
                throw new ArgumentException("ArgonSecret was not set");
            }

            services.AddSingleton(authConfig);

            services.AddScoped<IAuthManager, CosmosAuthManager>();
            services.AddScoped<PasswordUtils>();
        }
        else if (authType == AuthType.Zitadel)
        {
            var zitadelConfig = new ZitadelConfiguration
            {
                ClientId = configuration.GetValueFromContainer<string>("Zitadel.ClientId"),
                ClientSecret = configuration.GetValueFromContainer<string>("Zitadel.ClientSecret"),
                Authority = configuration.GetValueFromContainer<string>("Zitadel.Authority")
            };

            if (string.IsNullOrWhiteSpace(zitadelConfig.ClientId) ||
                string.IsNullOrWhiteSpace(zitadelConfig.ClientSecret) ||
                string.IsNullOrWhiteSpace(zitadelConfig.Authority))
            {
                throw new ArgumentException("Could not set Zitadel configuration - required properties are missing");
            }

            services.AddHttpClient(HttpClients.ZitadelClient, client =>
            {
                client.BaseAddress = new Uri(zitadelConfig.Authority);
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Basic",
                        Convert.ToBase64String(
                            Encoding.ASCII.GetBytes($"{zitadelConfig.ClientId}:{zitadelConfig.ClientSecret}")));
            });

            services.AddScoped<IAuthManager, ZitadelAuthManager>();
        }
        else
        {
            throw new NotImplementedException("Failed to instrument authentication layer - unimplemented AuthType");
        }
    }
}
