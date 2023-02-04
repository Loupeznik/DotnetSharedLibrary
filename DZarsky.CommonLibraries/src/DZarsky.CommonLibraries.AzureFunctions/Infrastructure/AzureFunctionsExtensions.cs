using DZarsky.CommonLibraries.AzureFunctions.Configuration;
using DZarsky.CommonLibraries.AzureFunctions.Extensions;
using DZarsky.CommonLibraries.AzureFunctions.Security;
using DZarsky.CommonLibraries.AzureFunctions.Security.CosmosDB;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DZarsky.CommonLibraries.AzureFunctions.Infrastructure
{
    public static class AzureFunctionsExtensions
    {
        public static IFunctionsHostBuilder AddCommonFunctionServices(this IFunctionsHostBuilder builder, IConfiguration configuration)
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

            return builder;
        }
    }
}
