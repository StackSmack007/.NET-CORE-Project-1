using Microsoft.AspNetCore.Hosting;
[assembly: HostingStartup(typeof(Junjuria.App.Areas.Identity.IdentityHostingStartup))]
namespace Junjuria.App.Areas.Identity
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