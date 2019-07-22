using MediatR;
using MicroRabbit.Domain.Core.Bus;
using MicroRabbit.Domain.Core.Commands;
using MicroRabbit.Domain.Core.Events;
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
        private readonly Dictionary<string, List<Type>> _handlers;
        private readonly List<Type> _eventType;

        public RabbitMqBus(IMediator mediator)
        {
            _mediator = mediator;
            _handlers = new Dictionary<string, List<Type>>();
            _eventType = new List<Type>();
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

        public void Subscribe<T, TH>()
            where T : Event
            where TH : IEventHandler<T>
        {
            var eventName = typeof(T).Name;
            var handlerType = typeof(TH);

            if (!_eventType.Contains(typeof(T)))
            {
                _eventType.Add(typeof(T));
            }

            if (!_handlers.ContainsKey(eventName))
            {
                _handlers.Add(eventName, new List<Type>());
            }

            if (_handlers[eventName].Any(s => s.GetType() == handlerType))
            {
                throw new ArgumentException($"Handler Type {handlerType.Name} already registed ");
            }

            _handlers[eventName].Add(handlerType);

            StartBasicConsume<T>();
        }

        private void StartBasicConsume<T>() where T : Event
        {
            var factory = new ConnectionFactory() { HostName = "localHost",
                DispatchConsumersAsync = true };


            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                var eventName = typeof(T).Name;
                channel.QueueDeclare(eventName, false, false, false, null);
                var consumer = new AsyncEventingBasicConsumer(channel);
                consumer.Received += Consumer_Received;
                channel.BasicConsume(eventName, true, consumer);
            }
        }
        
        private async  Task Consumer_Received(object sender, BasicDeliverEventArgs @event)
        {
            var eventName = @event.RoutingKey;
            var body = @event.Body;
            var message = Encoding.UTF8.GetString(body);
            try
            {
                await ProccessEvent(eventName, message);
            }
            catch (Exception)
            {

                 
            }
        }

        private async Task ProccessEvent(string eventName, string message)
        {
            if (_handlers.ContainsKey(eventName))
            {
                var subscriptions = _handlers[eventName];

                foreach (var subscription in subscriptions)
                {
                    var handler = Activator.CreateInstance(subscription);
                    if (handler != null) {

                        var eventType = _eventType.SingleOrDefault(t => t.Name == eventName);
                        var @event = JsonConvert.DeserializeObject(message, eventType);
                        var concreteType = typeof(IEventHandler<>).MakeGenericType(eventType);
                        await (Task)concreteType.GetMethod("Handle").Invoke(handler, new object[] { @event });

                        //dynamic cType = typeof(IEventHandler<>).MakeGenericType(eventType);
                        //cType.Handle(@event);
                    }
                }

            }
        }
    }
}
