using AutoMapper;
using Business.Abstract;
using Business.Concrete;
using Business.Mapper.AutoMapper;
using Core.IoC;
using Core.Logging.Middleware;
using Core.Security.JWT;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();

            //Authentication
            var tokenConfig = Configuration.GetSection("TokenConfig").Get<TokenConfig>();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidIssuer = tokenConfig.Issuer,
                        ValidAudience = tokenConfig.Audience,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenConfig.SecurityKey)),
                        ClockSkew = TimeSpan.Zero
                    };
                });

            //Database
            services.AddDbContext<ByCellDbContext>(
                options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            //Merkezi dependency resolver
            services.AddDependencyResolvers(new ICoreModule[] {
                new CoreModule()
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ByCell v1"));
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            //Merkezi exception console log middleware
            app.UseMiddleware<CustomExceptionMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });           
        }
    }
}
