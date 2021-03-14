namespace DummyWebApp
{
    using DAL;
    using Extensions;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddScoped<DbSeeder>()
                .AddDbContext<ApplicationDbContext>((provider, builder) =>
                    builder.UseNpgsql(EnvironmentExtensions.GetValueOrThrow<string>("CONNECTION_STRING"))
                        .UseLoggerFactory(LoggerFactory.Create(loggingBuilder =>
                        {
                            if (provider.GetRequiredService<IWebHostEnvironment>().IsDevelopment())
                                loggingBuilder.AddConsole();
                        })));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseEndpoints(_ =>
            {
            });
        }
    }
}