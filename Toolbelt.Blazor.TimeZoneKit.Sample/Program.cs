using System.Threading.Tasks;
using Microsoft.AspNetCore.Blazor.Hosting;
using Toolbelt.Blazor.Extensions.DependencyInjection;

namespace Toolbelt.Blazor.TimeZoneKit.Sample
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            await builder
                .Build()
                .UseLocalTimeZone()
                .RunAsync();
        }
    }
}
