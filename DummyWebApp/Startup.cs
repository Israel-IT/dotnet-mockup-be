namespace DummyWebApp
{
    using System.Net.Mime;
    using System.Reflection;
    using System.Text.Json.Serialization;
    using BLL.Options;
    using BLL.Services;
    using BLL.Services.Abstraction;
    using DAL;
    using Extensions;
    using Filters;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using SharedResources;

    public class Startup
    {
        private readonly IConfiguration _configuration;

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddScoped<DbSeeder>()
                .AddDbContext<ApplicationDbContext>((provider, builder) =>
                    builder
                        .UseNpgsql(EnvironmentExtensions.GetValueOrThrow<string>("CONNECTION_STRING"))
                        .UseLoggerFactory(LoggerFactory.Create(loggingBuilder =>
                        {
                            if (provider.GetRequiredService<IWebHostEnvironment>().IsDevelopment())
                                loggingBuilder.AddConsole();
                        })))
                .AddScoped<IAuthService, AuthService>()
                .AddSingleton<IEmailService, EmailService>()
                .AddSingleton<IResetPasswordTokenProvider, ResetPasswordTokenProvider>()
                .AddJwtBearerAuth(_configuration)
                .AddIdentity()
                .Configure<EmailOptions>(_configuration.GetSection(nameof(EmailOptions)))
                .AddSwagger()
                .AddLocalization(options => options.ResourcesPath = "Resources")
                .AddControllers(options =>
                {
                    options.Filters.Add<ErrorableResultFilterAttribute>();
                    options.Filters.Add(new ProducesAttribute(MediaTypeNames.Application.Json));
                })
                .AddDataAnnotationsLocalization(options => options.DataAnnotationLocalizerProvider = (_, factory) => factory.Create(typeof(SharedResource)))
                .AddJsonOptions(options => options
                    .JsonSerializerOptions
                    .Converters
                    .Add(new JsonStringEnumConverter()));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRequestLocalization(options => options
                .AddSupportedCultures("en", "uk")
                .SetDefaultCulture("en")
                .AddSupportedUICultures("en", "uk"));
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", Assembly.GetAssembly(GetType())?.GetName().Name);
            });
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoint =>
            {
                endpoint.MapControllers();
            });
        }
    }
}