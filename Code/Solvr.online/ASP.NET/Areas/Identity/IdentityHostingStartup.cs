using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(ASP.NET.Areas.Identity.IdentityHostingStartup))]
namespace ASP.NET.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
            });
        }
    }
}