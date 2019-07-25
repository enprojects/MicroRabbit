using MicroRabbit.Banking.Application.Interfaces;
using MicroRabbit.Banking.Application.Services;
using MicroRabbit.Banking.Data.Repository;
using MicroRabbit.Banking.Domain.Interfaces;
using MicroRabbit.Domain.Core.Bus;
using MicroRabbit.Infra.Bus;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;

using MicroRabbit.Domain.Core.Commands;

namespace MicroRabbit.Infra.Ioc
{




    public static class DependencyContainer
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            services.AddTransient<IEventBus, RabbitMqBus>();

            //  Application layer 
            services.AddTransient<IAccountRepository, AccountRepository>();
            services.AddTransient<IAccountService, AccountService>();
        }

        public static Assembly[] GetAllCommandsAssemblies()
        {

            var assemlies  =AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
                 .Where(x => typeof(Command).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                 .Select(x => x.Assembly).ToArray();

            if (assemlies == null || !assemlies.Any())
            {
                throw new ArgumentNullException("Commands assembly name is missing... ");
            }

            return assemlies;
        }
        
    }
}
