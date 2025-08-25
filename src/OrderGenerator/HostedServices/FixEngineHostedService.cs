using QuickFix;
using QuickFix.Logger;
using QuickFix.Store;
using QuickFix.Transport;

namespace OrderGenerator.HostedServices
{
    public class FixEngineHostedService : IHostedService
    {
        private readonly IApplication _application;
        private readonly SessionSettings _settings;
        private readonly IMessageStoreFactory _storeFactory;
        private readonly ILogFactory _logFactory;
        private readonly ILogger<FixEngineHostedService> _logger;

        private readonly IInitiator _initiator;

        public FixEngineHostedService(
            IApplication application,
            SessionSettings settings,
            IMessageStoreFactory storeFactory,
            ILogFactory logFactory,
            ILogger<FixEngineHostedService> logger)
        {
            _application = application;
            _settings = settings;
            _storeFactory = storeFactory;
            _logFactory = logFactory;
            _logger = logger;
            _initiator = new SocketInitiator(_application, _storeFactory, _settings, _logFactory);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Iniciando o motor FIX...");
            _initiator.Start();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Parando o motor FIX...");
            _initiator?.Stop();
            _logger.LogInformation("Motor FIX parado.");
            return Task.CompletedTask;
        }
    }
}
