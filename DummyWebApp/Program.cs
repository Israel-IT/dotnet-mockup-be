namespace DummyWebApp
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using DAL;
    using DotNetEnv;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            using var scope = host.Services.CreateScope();
            var envName = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>().EnvironmentName;

            Env.Load(Path.Combine(Environment.CurrentDirectory, $"config.{envName}.env"));
            await scope.ServiceProvider.GetRequiredService<DbSeeder>().SeedAsync();

            await host.RunAsync();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
            => Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
    }
}