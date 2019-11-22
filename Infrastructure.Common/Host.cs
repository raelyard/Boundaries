using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;

namespace Podcasting.Infrastructure.Common
{
    public class Host
    {
        private static readonly ILog Log = LogManager.GetLogger<Host>();

        private const string CommandsExpression = @"Podcasting\.(.*)\.Commands.*";
        private const string EventsExpression = @"Podcasting\.Public\.Events\.([A-Za-z]+)(?:\..*)?";
        
        private IEndpointInstance _endpoint;

        public string EndpointName { get; }

        public Host()
        {
            EndpointName = Assembly.GetEntryAssembly().GetName().Name;
        }

        public async Task<IEndpointInstance> Start(params Assembly[] messageAssemblies)
        {
            try
            {
                Log.Info($"Starting endpoint: {EndpointName}");
                var endpointConfiguration = new EndpointConfiguration(EndpointName);

                var transport = endpointConfiguration.UseTransport<LearningTransport>();
                var messageTypesAndDestinations = GetEndpointsForMessages(transport, messageAssemblies);
                ConfigureEnpointFundamentals(endpointConfiguration);
                ConfigureMessageConventions(endpointConfiguration, messageTypesAndDestinations);
                ConfigureRouting(transport, messageTypesAndDestinations);
                endpointConfiguration.EnableInstallers();

                _endpoint = await Endpoint.Start(endpointConfiguration);
                return _endpoint;
            }
            catch (Exception ex)
            {
                FailFast("Failed to start.", ex);
            }
            return default;
        }

        private void ConfigureEnpointFundamentals(EndpointConfiguration endpointConfiguration)
        {
            endpointConfiguration.DefineCriticalErrorAction(OnCriticalError);
            endpointConfiguration.UseSerialization<NewtonsoftSerializer>();
            endpointConfiguration.UsePersistence<LearningPersistence>();
            endpointConfiguration.LicensePath(Environment.GetEnvironmentVariable("NServiceBusLicencePath"));
        }

        private IEnumerable<KeyValuePair<string, Type>> GetEndpointsForMessages(
            TransportExtensions<LearningTransport> transport,
            params Assembly[] messageAssemblies)
        {
            return messageAssemblies
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => !string.IsNullOrWhiteSpace(type.Namespace))
                .Where(type => Regex.IsMatch(type.Namespace, CommandsExpression) || Regex.IsMatch(type.Namespace, EventsExpression))
                .ToDictionary(
                    type =>
                    {
                        var regex = new Regex(CommandsExpression);
                        var match = regex.Match(type.Namespace);
                        if (!match.Success)
                        {
                            regex = new Regex(EventsExpression);
                            match = regex.Match(type.Namespace);
                            if (!match.Success)
                            {
                                return Guid.NewGuid().ToString();
                            }
                        }
                        return $"Podcasting.{match.Groups[1].Captures[0].Value}.Processor";
                    },
                    type => type
                );
        }

        private void ConfigureMessageConventions(EndpointConfiguration endpointConfiguration,
            IEnumerable<KeyValuePair<string, Type>> messageTypesAndDestinations)
        {
            var conventions = endpointConfiguration.Conventions();

            conventions.DefiningCommandsAs(type => Regex.IsMatch(type.Namespace, CommandsExpression));
            conventions.DefiningEventsAs(type => Regex.IsMatch(type.Namespace, EventsExpression));
        }

        private void ConfigureRouting(TransportExtensions<LearningTransport> transport, IEnumerable<KeyValuePair<string, Type>> messageTypesAndDestinations)
        {
            var routing = transport.Routing();

            foreach (var messageTypeAndDestination in messageTypesAndDestinations)
            {
                Log.Debug($"Routing message type {messageTypeAndDestination.Value} to endpoint {messageTypeAndDestination.Key}");
                routing.RouteToEndpoint(messageTypeAndDestination.Value, messageTypeAndDestination.Key);
            }
        }

        public async Task Stop()
        {
            try
            {
                await _endpoint?.Stop();
            }
            catch (Exception ex)
            {
                FailFast("Failed to stop correctly.", ex);
            }
        }

        private async Task OnCriticalError(ICriticalErrorContext context)
        {
            try
            {
                await context.Stop();
            }
            finally
            {
                FailFast($"Critical error, shutting down: {context.Error}", context.Exception);
            }
        }

        private void FailFast(string message, Exception exception)
        {
            try
            {
                Log.Fatal(message, exception);
            }
            finally
            {
                Environment.FailFast(message, exception);
            }
        }
    }
}
