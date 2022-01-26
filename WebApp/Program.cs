using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyApp.ApplicationLogic;
using MyApp.Repository;
using MyApp.Repository.ApiClient;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace WebApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            //builder.Services.AddOptions();
            //builder.Services.AddAuthorizationCore();
            //builder.Services.AddSingleton<AuthenticationStateProvider, CustomTokenAuthenticationStateProvider>();
            //builder.Services.AddSingleton<AuthenticationStateProvider, JwtTokenAuthenticationStateProvider>();

            builder.Services.AddSingleton<ITokenRepository, TokenRepository>();

            builder.Services.AddSingleton(sp => sp.GetRequiredService<IHttpClientFactory>()
                .CreateClient("WebApi"));

            builder.Services.AddHttpClient(
                "WebApi",
                client => client.BaseAddress = new Uri("https://localhost:44314"))
                .AddHttpMessageHandler<AuthorizationMessageHandler>();

            builder.Services.AddTransient<AuthorizationMessageHandler>(sp =>
            {
                var provider = sp.GetRequiredService<IAccessTokenProvider>();
                var navigationManager = sp.GetRequiredService<NavigationManager>();

                var handler = new AuthorizationMessageHandler(provider, navigationManager);
                handler.ConfigureHandler(authorizedUrls: new []{ "https://localhost:44314/" });
                return handler;
            });

            builder.Services.AddSingleton<IWebApiExecuter, WebApiExecuter>();

            //builder.Services.AddTransient<IAuthenticationRepository, AuthenticationRepository>();
            //builder.Services.AddTransient<IAuthenticationUseCases, AuthenticationUseCases>();
            builder.Services.AddTransient<IProjectsScreenUseCases, ProjectsScreenUseCases>();
            builder.Services.AddTransient<IProjectRepository, ProjectRepository>();
            builder.Services.AddTransient<ITicketRepository, TicketRepository>();
            builder.Services.AddTransient<ITicketsScreenUseCases, TicketsScreenUseCases>();
            builder.Services.AddTransient<ITicketScreenUseCases, TicketScreenUseCases>();

            builder.Services.AddOidcAuthentication(options =>
            {
                builder.Configuration.Bind("local", options.ProviderOptions);

                options.ProviderOptions.DefaultScopes.Add("WebAPI");
                options.ProviderOptions.DefaultScopes.Add("profile");
                options.ProviderOptions.DefaultScopes.Add("email");
                options.ProviderOptions.DefaultScopes.Add(ClaimTypes.Role);
            });

            //builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            await builder.Build().RunAsync();
        }
    }
}
