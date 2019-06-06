using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using TestSPA.Middleware;
using TestSPA.Repositories;

namespace TestSPA
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
            services
                .AddMvc()
                .AddJsonOptions(opt =>
                {
                    opt.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    opt.SerializerSettings.TypeNameHandling = TypeNameHandling.Auto;
                });

            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                TypeNameHandling = TypeNameHandling.Auto
            };

            var connectionString = Configuration.GetConnectionString("TestSPA");

            services.AddScoped(provider => new ServersRepository(connectionString));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            var rewriteOptions = new RewriteOptions()
              // ToDo: split scripts, css and other files to subfolders. Ng Build cannot separate files
              //https://stackoverflow.com/questions/43283915/ng-build-move-scripts-to-subfolder?answertab=active#tab-top
              //.AddRewrite("(assets/.*)", "$1", true)
              //.AddRewrite("(scripts/.*)", "$1", true)
              //.AddRewrite("(styles/.*)", "$1", true)
              .AddRewrite(@"(.*\.(\w|\d)+$)", "$1", true)
              .AddRewrite("(api/.*)", "$1", true)
              .AddRewrite(@"(/.)*", "/", true);

            app.UseRewriter(rewriteOptions);
            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseMiddleware<ExceptionMiddleware>();

            app.UseMvc();
        }
    }
}
