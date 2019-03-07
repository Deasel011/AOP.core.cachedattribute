using AOP.Netcore.Caching;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TestWebApp
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddMemoryCache();
            services.AddTransient<Values>();

            services.AddTransient(typeof(IValues), (provider) =>
            {
                return new Cached<IValues>(provider.GetRequiredService(typeof(IMemoryCache)), provider.GetRequiredService<Values>());
            });



            //services.AddTransient<IValues>((provider) =>
            //{
            //    return new Cached<Values>(provider.GetRequiredService<IMemoryCache>(), typeof(IValues)); });



            //services.AddCaching<Values>(typeof(IValues));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
