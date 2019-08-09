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
using MicroRabbit.Transfer.Application.Interfaces;
using MicroRabbit.Transfer.Application.Services;
using MicroRabbit.Transfer.Domain.Interfaces;
using MicroRabbit.Transfer.Data.Repository;
using MicroRabbit.Transfer.Domain.Events;
using MicroRabbit.Transfer.Domain.EventHandles;
using MediatR;
using System.Collections.Generic;


namespace MicroRabbit.Infra.Ioc
{




    public static class DependencyContainer
    {
        public static void RegisterServices(this IServiceCollection services)
        {
          

            //  Application layer 
            services.AddTransient<IAccountRepository, AccountRepository>();
            services.AddTransient<IAccountService, AccountService>();

            services.AddTransient<ITransferService, TransferService>();
            services.AddTransient<ITransferRepository, TransferRepository>();

            //domain events
            services.AddTransient<IEventHandler<TransferCreatedEvent>, TransferEventHandler>();
            services.AddTransient<TransferEventHandler>();

              ResgistrAllEvents(services);
            services.AddSingleton<IEventBus, RabbitMqBus>(sp => {

                return new RabbitMqBus(sp.GetService<IMediator>(),sp);
            });

          
        }

        public static List<Type> GetAllEventHandlerType()
        {
            // get all types from all assemblies 
            var allTypesFromAssemb =  AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .ToList();
            // filter all by the event type interface 
            return allTypesFromAssemb .Where(x => x.GetInterfaces().Any(t => IsTypeImplementGenericInterface(t)))
                                       .ToList();
        }

        public static void ResgistrAllEvents(this IServiceCollection services)
        {           
            List<Type> handlerTypes = GetAllEventHandlerType();

             // register all event type handlers  
            foreach (Type type in handlerTypes)
            {
                AddHandler(services, type);
            }
        }

        private static void AddHandler(IServiceCollection services, Type type)
        {
            // get te interface inplemented by this type, in this case command, query , event 
            var getInterfaceType = type.GetInterfaces().Single(interfaceType => IsTypeImplementGenericInterface(interfaceType));
            services.AddTransient(getInterfaceType, (sp) =>
            {
                object obj = Activator.CreateInstance(type, InstantiateConstructorParam(type, services));
                return obj;

            });
        }

        private static object [] InstantiateConstructorParam(Type type , IServiceCollection services)
        {
            var parametersValues = new List<object>();
            var  ctor  = type.GetConstructors().First();
            var parameters = ctor.GetParameters();
           
            if (parameters.Count() > 0) {

                foreach (var parmeter in parameters)
                {
                    Type parameterType = parmeter.ParameterType;
                    if (!IsTypeImplementGenericInterface(parameterType))
                    {

                        var  _serviceProvider = services.BuildServiceProvider();
                        parametersValues.Add(_serviceProvider.GetService(parameterType));
                    }
                }
            }

            return parametersValues.ToArray();
        }

        private static bool IsTypeImplementGenericInterface(Type type)
        {
            if (!type.IsGenericType)
                return false;
            // Generic type defination stand for th IItrerface<> only the shallow for the interafce 
            Type typeDefinition = type.GetGenericTypeDefinition();

            return typeDefinition == typeof(IEventHandler<>) ;
        }

        public static Assembly[] GetAllCommandsAssemblies()
        {

            // IsAssignableFrom mean is x (some type) Implment interfae rype
            var assemlies  =AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
                 .Where(x => typeof(Command).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                 .Select(x => x.Assembly).ToArray();

            if (assemlies == null || !assemlies.Any())
            {
                throw new ArgumentNullException("Commands assembly name is missing... ");
            }

            return assemlies.GroupBy(x=> x.FullName).Select(x=>x.FirstOrDefault()).ToArray();
        }
        
    }
}
