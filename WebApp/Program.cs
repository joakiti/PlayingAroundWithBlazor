using Microsoft.AspNetCore.Components.Authorization;
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

            builder.Services.AddOptions();
            builder.Services.AddAuthorizationCore();
            //builder.Services.AddSingleton<AuthenticationStateProvider, CustomTokenAuthenticationStateProvider>();
            builder.Services.AddSingleton<AuthenticationStateProvider, JwtTokenAuthenticationStateProvider>();

            builder.Services.AddSingleton<ITokenRepository, TokenRepository>();
            builder.Services.AddSingleton<IWebApiExecuter>(sp => 
                new WebApiExecuter(
                    "https://localhost:44314", 
                    new HttpClient(),
                    sp.GetRequiredService<ITokenRepository>()));

            builder.Services.AddTransient<IAuthenticationRepository, AuthenticationRepository>();
            builder.Services.AddTransient<IAuthenticationUseCases, AuthenticationUseCases>();
            builder.Services.AddTransient<IProjectsScreenUseCases, ProjectsScreenUseCases>();
            builder.Services.AddTransient<IProjectRepository, ProjectRepository>();
            builder.Services.AddTransient<ITicketRepository, TicketRepository>();
            builder.Services.AddTransient<ITicketsScreenUseCases, TicketsScreenUseCases>();
            builder.Services.AddTransient<ITicketScreenUseCases, TicketScreenUseCases>();

            //builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            await builder.Build().RunAsync();
        }
    }
}
