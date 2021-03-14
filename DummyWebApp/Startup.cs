namespace DummyWebApp
{
    using System;
    using System.IO;
    using System.Net.Mime;
    using System.Reflection;
    using System.Text;
    using System.Text.Json.Serialization;
    using BLL.Dtos.Auth;
    using BLL.Options;
    using BLL.Services;
    using BLL.Services.Abstraction;
    using Core.ResultConstants;
    using DAL;
    using DAL.Entities;
    using Extensions;
    using Filters;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.IdentityModel.Tokens;
    using Microsoft.OpenApi.Models;

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
                    builder.UseNpgsql(EnvironmentExtensions.GetValueOrThrow<string>("CONNECTION_STRING"))
                        .UseLoggerFactory(LoggerFactory.Create(loggingBuilder =>
                        {
                            if (provider.GetRequiredService<IWebHostEnvironment>().IsDevelopment())
                                loggingBuilder.AddConsole();
                        })))
                .AddScoped<IAuthService, AuthService>()
                .AddSingleton<IEmailService, EmailService>()
                .AddSingleton<IResetPasswordTokenProvider, ResetPasswordTokenProvider>()
                .AddIdentityCore<User>()
                .AddUserManager<UserManager<User>>()
                // .AddRoles<IdentityRole<int>>()
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Audience = _configuration["Token:Audience"];
                    options.RequireHttpsMetadata = bool.Parse(_configuration["Token:RequireHttpsMetadata"]);
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateActor = bool.Parse(_configuration["Token:ValidateActor"]),
                        ValidateAudience = bool.Parse(_configuration["Token:ValidateAudience"]),
                        ValidateLifetime = bool.Parse(_configuration["Token:ValidateLifetime"]),
                        ValidateIssuerSigningKey = bool.Parse(_configuration["Token:ValidateIssuerSigningKey"]),
                        ValidIssuer = _configuration["Token:Issuer"],
                        ValidAudience = _configuration["Token:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Token:Key"]))
                    };
                })
                .Services
                .Configure<IdentityOptions>(options =>
                {
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequiredLength = IdentityPasswordConstants.RequiredLength;
                    options.Password.RequiredUniqueChars = 0;
                    options.User.RequireUniqueEmail = true;
                })
                .Configure<EmailOptions>(_configuration.GetSection(nameof(EmailOptions)))
                .AddSwaggerGen(options =>
                {
                    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        In = ParameterLocation.Header,
                        Description = "Please insert JWT with Bearer into field",
                        Name = "Authorization",
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = JwtBearerDefaults.AuthenticationScheme,
                        BearerFormat = "JWT"
                    });
                    options.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = JwtBearerDefaults.AuthenticationScheme
                                }
                            },
                            Array.Empty<string>()
                        }
                    });
                })
                .AddSwaggerGen(options =>
                {
                    options.SchemaFilter<IgnoreReadOnlySchemaFilter>();
                    options.IncludeXmlComments(Path.Combine(
                        AppContext.BaseDirectory,
                        $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
                    options.IncludeXmlComments(Path.Combine(
                        AppContext.BaseDirectory,
                        $"{Assembly.GetAssembly(typeof(LoginUserDto))?.GetName().Name}.xml"));
                })
                .AddControllers(options =>
                {
                    options.Filters.Add<ErrorableResultFilterAttribute>();
                    options.Filters.Add(new ProducesAttribute(MediaTypeNames.Application.Json));
                })
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