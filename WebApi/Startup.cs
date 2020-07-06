using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

using Core;
using Infrastructure.Auth;
using Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using Infrastructure.Repositories.Interfaces;
using Infrastructure.Repositories;

namespace WebApi
{
    public class Startup
    {
        private readonly string _nameOfSpecificOrigins = "corsPolicy";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services, IWebHostEnvironment environment)
        {
            services.AddMvc(options => options.EnableEndpointRouting = false)
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            // dbContext
            services.AddDbContext<DbMainContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("FilmsDBConnection"),
                    b => b.MigrationsAssembly("WebApi")));
            //services.AddScoped<DbContext, DbMainContext>();

            // automapper
            services.AddAutoMapper(typeof(Startup));

            // cors
            services.AddCors(options =>
            {
                options.AddPolicy(name: _nameOfSpecificOrigins, builder =>
                builder.WithOrigins(Configuration.GetValue<string>("webClientPath"))
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials()
                            );
            });

            // repos
            services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));

            // managers

            // auth
            services.AddIdentity<Account, IdentityRole>(options =>
            {
                // IMPORTANT! after changing password validations options, change "password-confirmation-form.component" in client accordingly
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 8;
                options.User.RequireUniqueEmail = false;
                options.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+/";
            })
                .AddEntityFrameworkStores<DbMainContext>()
                .AddDefaultTokenProviders();
            services.AddHttpContextAccessor();
            services.AddIdentityServer()
                .AddApiAuthorization<Account, DbMainContext>();
            services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddIdentityServerJwt()
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = !environment.IsDevelopment();
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        // укзывает, будет ли валидироваться издатель при валидации токена
                        ValidateIssuer = true,
                        // строка, представляющая издателя
                        ValidIssuer = AuthOptions.ISSUER,

                        // будет ли валидироваться потребитель токена
                        ValidateAudience = true,
                        // установка потребителя токена
                        ValidAudience = AuthOptions.AUDIENCE,
                        // будет ли валидироваться время существования
                        ValidateLifetime = true,

                        // установка ключа безопасности
                        IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
                        // валидация ключа безопасности
                        ValidateIssuerSigningKey = true
                    };
                });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseIdentityServer();
            app.UseRouting();
            app.UseEndpoints(endps =>
            {
                endps.MapControllers();
            });

            app.UseMvc();

            //cors
            app.UseCors(_nameOfSpecificOrigins);
        }
    }
}
