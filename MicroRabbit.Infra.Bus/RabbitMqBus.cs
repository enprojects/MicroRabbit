using MediatR;
using MicroRabbit.Banking.Domain.Events;
using MicroRabbit.Domain.Core.Bus;
using MicroRabbit.Domain.Core.Commands;
using MicroRabbit.Domain.Core.Events;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroRabbit.Infra.Bus
{
    public sealed class RabbitMqBus : IEventBus

    {

        private readonly IMediator _mediator;
        private readonly Dictionary<Type, List<Type>> _handlers;
        private readonly IServiceProvider _sp;
        public RabbitMqBus(IMediator mediator, IServiceProvider sp)
        {
            _mediator = mediator;
            _handlers = new Dictionary<Type, List<Type>>();          
            _sp = sp;
        }

        public Task SendCommand<T>(T command) where T : Command
        {
            return _mediator.Send(command);
        }


        public void Publish<T>(T @event) where T : Event
        {
            var factory = new ConnectionFactory() { HostName = "localHost" };
            using (var connection = factory.CreateConnection())
            using (var channnel = connection.CreateModel())
            {

                var eventName = @event.GetType().Name;
                channnel.QueueDeclare(eventName, false, false, false, null);
                var message = JsonConvert.SerializeObject(@event);

                var body = Encoding.UTF8.GetBytes(message);
                channnel.BasicPublish("", eventName, null, body);
            }
        }

        public void SubscribeAdvanced<TEvnt>() where TEvnt : Event
        {

            var eventType = typeof(TEvnt);
            if (!_handlers.ContainsKey(typeof(TEvnt)))
            {
                _handlers.Add(eventType, new List<Type>());
            }

            _handlers[eventType].Add(typeof(IEventHandler<TEvnt>));
            StartBasicConsume<TEvnt>();
        }

         
        private void StartBasicConsume<T>() where T : Event
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localHost",
                DispatchConsumersAsync = true
            };


            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            {
                var eventName = typeof(T).Name;
                channel.QueueDeclare(eventName, false, false, false, null);
                var consumer = new AsyncEventingBasicConsumer(channel);
                consumer.Received += Consumer_Received;
                channel.BasicConsume(eventName, true, consumer);
            }
        }

        private async Task Consumer_Received(object sender, BasicDeliverEventArgs e)
        {
            var eventName = e.RoutingKey;
            var body = e.Body;
            var message = Encoding.UTF8.GetString(body);
            try
            {
                await ProccessEvent(eventName, message);
            }
            catch (Exception ex)
            {


            }
        }

        private async Task ProccessEvent(string eventName, string message)
        {
            if (_handlers.Count > 0)
            {
                foreach (var keyPair in _handlers)
                {
                    if (keyPair.Key.Name == eventName)
                    {
                            foreach (var handlerType in keyPair.Value)
                            {
                            var data = JsonConvert.DeserializeObject(message, keyPair.Key);// Convert.ChangeType(JsonConvert.DeserializeObject(message, keyPair.Key), keyPair.Key);
                                dynamic handler = _sp.GetService(handlerType);
                                await (Task)handler.Handle((dynamic)data);
                            }
                    }
                }
            }

            await Task.CompletedTask;
        }



    }

}

