using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MediatR;
using MicroRabbit.Banking.Data.Context;
using MicroRabbit.Banking.Domain.CommandHandlers;
using MicroRabbit.Infra.Ioc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MIcroRabbit.Banking.Api
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

            services.AddDbContext<BankingDbCocntext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("BankingDbConnection"))
            );


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);


            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info { Title = "Banking microservice", Version = "v1" });
            });

            var assemblyName = typeof(TransferCommandHandler).GetTypeInfo().Assembly.FullName;

            var assemblyComandsName = AppDomain.CurrentDomain.GetAssemblies()
                                     .Where(x => x.GetName()
                                     .Name == Configuration.GetValue<string>("CommandsAssmbly"))
                                     .FirstOrDefault();
            if (assemblyComandsName == null)
            {
                throw new ArgumentNullException("Commands assembly name is missing... ");
            }

            // services.AddMediatR(assemblyComandsName/*typeof(TransferCommandHandler).GetTypeInfo().Assembly*/);
            services.RegisterServices();
            services.AddMediatR(DependencyContainer.GetAllCommandsAssemblies());


        }
            // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
            public void Configure(IApplicationBuilder app, IHostingEnvironment env)
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

            app.UseSwagger();
            app.UseSwaggerUI(opt => {
                opt.SwaggerEndpoint("/swagger/v1/swagger.json", "My Banking microservice");
            });
            app.UseMvc();
        }
    }
}
