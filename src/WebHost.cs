using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace TinyBlog;

internal static class WebHost
{
    public static void CreateAndStart(string contentRootDirectory, string outputDirectory, int port)
    {
        var host = new WebHostBuilder()
            .UseKestrel()
            .UseContentRoot(contentRootDirectory)
            .UseWebRoot(outputDirectory)
            .UseUrls($"http://localhost:{port}")
            .UseStartup<Startup>()
            .Build();

        host.Start();
    }
}

internal class Startup
{
    public static void Configure(IApplicationBuilder app)
    {
        app.UseStaticFiles();
    }
}