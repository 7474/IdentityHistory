using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Blazor.Hosting;
using Microsoft.Extensions.DependencyInjection;
using IdentityHistoryViewer.Service;

namespace IdentityHistoryViewer
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");
            builder.Services.AddSingleton(new IHAPIConfig()
            {
                BaseUri = "http://localhost:7071/",
            });
            builder.Services.AddTransient<IHAPI>();

            await builder.Build().RunAsync();
        }
    }
}
