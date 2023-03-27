using System;
using System.Collections.Generic;
using Lykke.Cqrs;
using Lykke.RabbitMqBroker;
using Microsoft.Extensions.Logging;

namespace Lykke.Snow.Notifications
{
    public class StartupManager
    {
        private readonly ICqrsEngine _cqrsEngine;
        private readonly IEnumerable<IStartStop> _rabbitMqStartables;
        private readonly ILogger<StartupManager> _logger;

        public StartupManager(ICqrsEngine cqrsEngine,
            IEnumerable<IStartStop> rabbitMqStartables,
            ILogger<StartupManager> logger)
        {
            _cqrsEngine = cqrsEngine;
            _rabbitMqStartables = rabbitMqStartables;
            _logger = logger;
        }

        internal void Start()
        {
            _cqrsEngine.StartSubscribers();
            
            foreach(var component in _rabbitMqStartables)
            {
                StartComponent(component);
            }
            
        }

        private void StartComponent(IStartStop component)
        {
            try 
            {
                component.Start();
                
                _logger.LogInformation("Started {Component} successfully.", component.GetType().Name);
            }
            catch(Exception e)
            {
                _logger.LogError(e, "Couldn't start the component {Component}", component.GetType().Name);

                throw;
            }
        }
    }
}
