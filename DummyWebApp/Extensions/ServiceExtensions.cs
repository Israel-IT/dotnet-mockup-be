namespace DummyWebApp.Extensions
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using BLL.Dtos.Auth;
    using BLL.Options;
    using Core.ResultConstants;
    using DAL;
    using DAL.Entities;
    using Filters;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.IdentityModel.Tokens;
    using Microsoft.OpenApi.Models;

    public static class ServiceExtensions
    {
        public static IServiceCollection AddJwtBearerAuth(this IServiceCollection serviceCollection, IConfiguration configuration)
            => serviceCollection.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Audience = configuration["Token:Audience"];
                    options.RequireHttpsMetadata = bool.Parse(configuration["Token:RequireHttpsMetadata"]);
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateActor = bool.Parse(configuration["Token:ValidateActor"]),
                        ValidateAudience = bool.Parse(configuration["Token:ValidateAudience"]),
                        ValidateLifetime = bool.Parse(configuration["Token:ValidateLifetime"]),
                        ValidateIssuerSigningKey = bool.Parse(configuration["Token:ValidateIssuerSigningKey"]),
                        ValidIssuer = configuration["Token:Issuer"],
                        ValidAudience = configuration["Token:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Token:Key"]))
                    };
                })
                .Services
                .Configure<AccessTokenOptions>(options =>
                {
                    options.Audience = configuration["Token:Audience"];
                    options.Issuer = configuration["Token:Issuer"];
                    options.Key = configuration["Token:Key"];
                    options.ValidateActor = bool.Parse(configuration["Token:ValidateActor"]);
                    options.ValidateAudience = bool.Parse(configuration["Token:ValidateAudience"]);
                    options.ValidateLifetime = bool.Parse(configuration["Token:ValidateLifetime"]);
                    options.AccessTokenLifetime = int.Parse(configuration["Token:AccessTokenLifetime"]);
                    options.RefreshTokenLifetime = int.Parse(configuration["Token:RefreshTokenLifetime"]);
                    options.ValidateIssuerSigningKey = bool.Parse(configuration["Token:ValidateIssuerSigningKey"]);
                });

        public static IServiceCollection AddSwagger(this IServiceCollection serviceCollection)
            => serviceCollection.AddSwaggerGen(options =>
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
                });

        public static IServiceCollection AddIdentity(this IServiceCollection serviceCollection)
            => serviceCollection
                .AddIdentityCore<User>()
                .AddUserManager<UserManager<User>>()
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<ApplicationDbContext>()
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
                });
    }
}