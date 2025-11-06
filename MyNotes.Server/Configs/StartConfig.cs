using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MyNotes.Server.Common;
using MyNotes.Server.Data.Implementations;
using MyNotes.Server.Data.Interfaces;
using MyNotes.Server.Services.Implementations;
using MyNotes.Server.Services.Interfaces;
using System.Security.Claims;
using System.Text;

namespace MyNotes.Server.Configs
{
    public static class StartConfig
    {
        public static void ConfigureRepositories(this IServiceCollection services)
        {
            // --- Repository Registration ---
            services.AddScoped<IUserRepository, UserRepository>();
        }

        public static void ConfigureServices(this IServiceCollection services)
        {
            // --- Service Registration ---
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IJwtService, JwtService>();

            //services.AddHttpClient<IUserService, UserService>();
            services.AddHttpContextAccessor();

        }

        public static void ConfigureValidators(this IServiceCollection services)
        {
            // --- Validator Registration ---
            //services.AddValidatorsFromAssemblyContaining<UserValidator>();
        }

        public static void ConfigureCors(this IServiceCollection services)
        {
            // --- Cross-origin resource sharing ---
            var AllowAngularApp = "AllowAngularApp";

            services.AddCors(options =>
            {
                options.AddPolicy(name: AllowAngularApp,
                                  policy =>
                                  {
                                      policy.WithOrigins(
                                          "https://localhost:4200",
                                          "http://localhost:4200",
                                          "https://127.0.0.1:4200",
                                          "http://127.0.0.1:4200")
                                      .AllowAnyHeader()
                                      .AllowAnyMethod();
                                  });
            });
        }

        public static void ConfigureSwaggerAuth(this IServiceCollection services)
        {
            services.AddSwaggerGen(swagger =>
            {
                //This is to generate the Default UI of Swagger Documentation
                swagger.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "JWT Token Authentication API",
                    Description = ".NET 8 Web API"
                });
                // To Enable authorization using Swagger (JWT)
                swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. " +
                    "\r\n\r\n Enter 'Bearer' [space] and then your token in the text input below." +
                    "\r\n\r\nExample: \"Bearer 12345abcdef\"",
                });
                swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });
        }

        public static void ConfigureAuthentication(this IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false; // set true in PROD
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(AppParameters.AppSettings.GoogleAuth.ClientSecret)
                    ),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = AppParameters.AppSettings.GoogleAuth.Issuer,
                    ValidAudience = AppParameters.AppSettings.GoogleAuth.ClientId,
                    ClockSkew = TimeSpan.Zero
                };

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        var email = context.Principal?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                        if (!string.IsNullOrEmpty(email))
                        {
                            var userService = context.HttpContext.RequestServices.GetRequiredService<IUserService>();
                            var user = await userService.GetByEmailAsync(email);

                            context.HttpContext.Items["User"] = user;
                        }
                    }
                };
            });
        }
    }
}
