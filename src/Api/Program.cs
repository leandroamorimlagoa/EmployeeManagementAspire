using System.Text;
using Api.Filters;
using Application.Models.Employees;
using Application.Services.Authentications;
using Application.Services.Employees;
using Domain.Authentications;
using Domain.Core;
using Domain.Employees;
using Domain.Notifications;
using Infrastructure.Repositories.Core;
using Infrastructure.Repositories.Employees;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;

namespace Api
{
    public class Program
    {
        private static string corsPolicy = "CorsPolicy";

        public static void Main(string[] args)
        {
            var audience = "https://{--app-audience--}";
            var issuer = "https://{--app-issuer--}";

            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddScoped(x => new AuthenticatedUser());
            builder.Services.AddScoped(x => new NotificationContext());
            var key = Encoding.UTF8.GetBytes("CHAVE_SECRETA_SUPER_SECRETA_LONGA_E_SEGURA_32_CHARSAKI");

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                            .AddEntityFrameworkStores<ApplicationDbContext>()
                            .AddDefaultTokenProviders();

            builder.Services.AddAuthentication(options =>
                            {
                                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                            })
                            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme,
                                            options =>
                                            {
                                                options.RequireHttpsMetadata = false;
                                                options.SaveToken = true;
                                                options.TokenValidationParameters = new TokenValidationParameters
                                                {
                                                    ValidateIssuerSigningKey = true,
                                                    IssuerSigningKey = new SymmetricSecurityKey(key),
                                                    ValidateIssuer = false,
                                                    ValidateAudience = false,
                                                    ClockSkew = TimeSpan.Zero
                                                };
                                            });

            builder.Services.AddAuthorization();

            builder.Services.AddControllers(options =>
            {
                options.Filters.Add<NotificationFilter>();
                options.Filters.Add<AuthenticationFilter>();
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            });

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Employee Management API",
                    Version = "v1",
                    Description = "API de gerenciamento de funcionários com autenticação JWT e ASP.NET Identity."
                });
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Insira o token JWT no campo abaixo. \nExemplo: Bearer {seu_token_aqui}"
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                        new string[] { }
                    }
                });
            });

            builder.AddSeqEndpoint(connectionName: "seq");

            var connectionString = builder.Configuration["Aspire:Npgsql:connectionString"];
            if (builder.Environment.EnvironmentName == "IntegrationTests")
            {
                builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("IntegrationTestDb"));
            }
            else
            {
                builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));
            }

            builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();


            var seqUrl = builder.Configuration["Serilog:WriteTo:0:Args:serverUrl"].ToString();
            builder.Host.UseSerilog((context, config) =>
            {
                config
                    .WriteTo.Console()
                    .WriteTo.Seq(seqUrl ?? "http://localhost:5341");
            });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(corsPolicy,
                    builder => builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
            });

            var app = builder.Build();

            app.UseSerilogRequestLogging();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors(corsPolicy);
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.Logger.LogInformation("API EmployeeManagementAspire started successfully!");

            try
            {
                using (var scope = app.Services.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                    db.Database.Migrate();
                    // add director
                    var director = new ApplicationUser
                    {
                        FirstName = "Admin FirstName",
                        LastName = " LastName",
                        Email = "admin@admin.com",
                        UserName = "admin@admin.com",
                        EmailConfirmed = true,
                        Status = EmployeeStatus.Active,
                        BirthDate = DateTime.UtcNow.AddYears(-19),
                        DocNumber = "AA3456789",
                        DocType = DocType.Passport,
                        Phones = new List<UserPhone>
                        {
                            new UserPhone { PhoneNumber = "123456789" },
                            new UserPhone { PhoneNumber = "987654321" }
                        },
                    };

                    if (userManager.FindByEmailAsync(director.Email).Result == null)
                    {
                        var result = userManager.CreateAsync(director, "Admin@123!").Result;
                        if (result.Succeeded)
                        {
                            if (!roleManager.RoleExistsAsync(Role.Director.ToString()).Result)
                            {
                                result = roleManager.CreateAsync(new IdentityRole(Role.Director.ToString())).Result;
                            }
                            userManager.AddToRoleAsync(director, Role.Director.ToString()).Wait();
                        }
                    }
                }
            }
            catch (Exception err)
            {
            }

            app.Run();

            Log.CloseAndFlush();
        }
    }
}

public partial class Program { }