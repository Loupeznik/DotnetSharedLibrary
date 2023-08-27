using DZarsky.CommonLibraries.AzureFunctions.Configuration;
using DZarsky.CommonLibraries.AzureFunctions.Extensions;
using DZarsky.CommonLibraries.AzureFunctions.Models.Auth;
using DZarsky.CommonLibraries.AzureFunctions.Security;
using DZarsky.CommonLibraries.AzureFunctions.Security.CosmosDB;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Zitadel.Extensions;

namespace DZarsky.CommonLibraries.AzureFunctions.Infrastructure
{
    public static class AzureFunctionsExtensions
    {
        public static IFunctionsHostBuilder AddCommonFunctionServices(this IFunctionsHostBuilder builder, IConfiguration configuration, AuthType? authType = AuthType.Standalone)
        {
            builder.Services.AddSingleton((s) =>
            {
                var endpoint = configuration.GetValueFromContainer<string>("CosmosDB.Endpoint");

                if (string.IsNullOrWhiteSpace(endpoint))
                {
                    throw new ArgumentException("CosmosDB endpoint was not set");
                }

                string authKey = configuration.GetValueFromContainer<string>("CosmosDB.AuthorizationKey");

                if (string.IsNullOrWhiteSpace(authKey))
                {
                    throw new ArgumentException("CosmosDB authorization key was not set");
                }

                var configurationBuilder = new CosmosClientBuilder(endpoint, authKey);

                return configurationBuilder
                        .Build();
            });

            var cosmosConfig = new CosmosConfiguration
            {
                DatabaseID = configuration.GetValueFromContainer<string>("CosmosDB.DatabaseID"),
                UsersContainerID = configuration.GetValueFromContainer<string>("CosmosDB.UserContainerID")
            };

            if (authType == AuthType.Standalone)
            {
                if (string.IsNullOrWhiteSpace(cosmosConfig.DatabaseID) || string.IsNullOrWhiteSpace(cosmosConfig.UsersContainerID))
                {
                    throw new ArgumentException("Could not set CosmosConfiguration - DatabaseID or UsersContainerID is missing");
                }

                var authConfig = new AuthConfiguration
                {
                    ArgonSecret = configuration.GetValueFromContainer<string>("ArgonSecret")
                };

                if (string.IsNullOrWhiteSpace(authConfig.ArgonSecret))
                {
                    throw new ArgumentException("ArgonSecret was not set");
                }

                builder.Services.AddSingleton(cosmosConfig);
                builder.Services.AddSingleton(authConfig);

                builder.Services.AddScoped<CosmosAuthManager>();
                builder.Services.AddScoped<PasswordUtils>();
            }
            else if (authType == AuthType.Zitadel)
            {
                if (string.IsNullOrWhiteSpace(cosmosConfig.DatabaseID))
                {
                    throw new ArgumentException("Could not set CosmosConfiguration - DatabaseID is missing");
                }

                builder.Services.AddSingleton(cosmosConfig);

                var zitadelConfig = new ZitadelConfiguration
                {
                    Authority = configuration.GetValueFromContainer<string>("Identity.Authority"),
                    JwtProfile = configuration.GetValueFromContainer<string>("Identity.JwtProfile"),
                    DebugRoles = configuration.GetValueFromContainer<IList<string>>("Identity.DebugRoles")
                };

                if (string.IsNullOrWhiteSpace(zitadelConfig.Authority) || string.IsNullOrWhiteSpace(zitadelConfig.JwtProfile))
                {
#if DEBUG
                    builder.Services
                        .AddAuthorization()
                        .AddAuthentication()
                        .AddZitadelFake(x =>
                        {
                            x.Roles = zitadelConfig.DebugRoles.ToArray();
                            x.FakeZitadelId = "functions-dev";
                        });

#else
                    builder.Services
                        .AddAuthorization()
                        .AddAuthentication()
                        .AddZitadelIntrospection(x =>
                        {
                            x.JwtProfile = Application.LoadFromJsonString(zitadelConfig.JwtProfile);
                            x.Authority = zitadelConfig.Authority;
                        });
#endif
                }
            }
            else
            {
                throw new ArgumentException("Failed to instrument authentication layer - unsupported AuthType");
            }

            return builder;
        }
    }
}
