using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Blazor.Hosting;
using Microsoft.Extensions.DependencyInjection;
using IdentityHistoryViewer.Service;
using BlazorWebAsmEasyAuth;
using Microsoft.AspNetCore.Components.Authorization;

namespace IdentityHistoryViewer
{
    public class Program
    {
#if DEBUG
        const string SITE_BASE_URI = "https://localhost:5001/";
        const string API_BASE_URI = "http://localhost:7071/";
#else
        const string SITE_BASE_URI = "https://identityhistory.z11.web.core.windows.net/";
        const string API_BASE_URI = "https://identityhistoryfunctionapp.azurewebsites.net/";
#endif
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddEasyAuth(new EasyAuthConfig()
            {
                BlazorWebsiteURL = SITE_BASE_URI,
                AzureFunctionAuthURL = API_BASE_URI,
            });

            builder.Services.AddSingleton(new IHAPIConfig()
            {
                BaseUri = API_BASE_URI,
            });
            builder.Services.AddTransient<IHAPI>();

            await builder.Build().RunAsync();
        }
    }
}
