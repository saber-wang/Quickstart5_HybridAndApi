using CacheManager.Core.Logging;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Cache.CacheManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ocelot.Provider.Polly;
using Ocelot.Administration;
using Microsoft.AspNetCore.DataProtection;

namespace Gateway
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration,IHostingEnvironment env)
        {
            Configuration = configuration;
        }

        

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDataProtection(options => options.ApplicationDiscriminator = "00000").SetApplicationName("00000");

            services.AddCors();

            services.AddOcelot()
            .AddCacheManager(x =>
            {
                x.WithDictionaryHandle();
            })
            .AddPolly()
            //.AddAdministration("/administration", options)
            ;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCors(builder =>
            {
                builder.WithOrigins("*");
                builder.AllowAnyHeader();
                builder.AllowAnyMethod();
            });
            app.UseOcelot().Wait();
        }
    }
}
