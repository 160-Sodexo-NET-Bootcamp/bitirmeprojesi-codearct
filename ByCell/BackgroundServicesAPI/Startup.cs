using BackgroundServicesAPI.BackgroundJobs.Jobs;
using Business.Abstract;
using Business.Concrete;
using Core.IoC;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackgroundServicesAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();


            //Database
            services.AddDbContext<ByCellDbContext>(
                options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));


            //Hangfire
            services.AddHangfire(config => config.UseSqlServerStorage(Configuration["ConnectionStrings:HangfireConnection"]));
            services.AddHangfireServer(option =>
            {
                option.SchedulePollingInterval = TimeSpan.FromSeconds(2);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            //Hangfire => endpoint host:port/bycellhangfire 
            app.UseHangfireDashboard("/bycellhangfire", new DashboardOptions
            {
                DashboardTitle = "ByCell Hangfire DashBoard"
            });

            /*app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });*/

            RecurringJobs.SendMailFromDatabase();

        }
    }
}
