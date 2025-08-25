using OrderGenerator.Models;
using QuickFix.Fields;
using QuickFix.FIX44;
using QuickFix;
using OrderGenerator.Models.Enum;
using OrderGenerator.Interfaces.Services;

namespace OrderGenerator.Services
{
    public class FixMessageService : IFixMessageService
    {
        private readonly SessionID? _fixSessionId;
        private readonly ILogger<FixMessageService> _logger;


        public FixMessageService(ILogger<FixMessageService> logger, SessionSettings sessionSettings)
        {
            _fixSessionId = sessionSettings.GetSessions().FirstOrDefault() ?? throw new ArgumentNullException(nameof(_fixSessionId));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _logger.LogInformation($"QuickFixMessageSender inicializado para SessionID FIX: {_fixSessionId}");
        }

        public async Task SendNewOrderSingleAsync(Order order)
        {
            NewOrderSingle orderSingle = new(
                new ClOrdID(order.ClOrdID),
                new Symbol(order.Symbol),
                new Side(order.Side == OrderSide.Buy ? Side.BUY : Side.SELL),
                new TransactTime(order.OrderTime),
                new OrdType(OrdType.MARKET)
            );
            orderSingle.Set(new OrderQty(order.Quantity));
            orderSingle.Set(new Price(order.Price));

            Session? fixSession = Session.LookupSession(_fixSessionId!);

            if (fixSession == null || !fixSession.IsLoggedOn)
            {
                var errorMessage = $"Sessão FIX '{_fixSessionId}' não está ativa ou logada. Não foi possível enviar a mensagem para ClOrdID '{order.ClOrdID}'.";
                _logger.LogError(errorMessage);
                throw new SessionNotFound(errorMessage);
            }

            try
            {
                fixSession.Send(orderSingle);
                _logger.LogInformation($"Mensagem NewOrderSingle para ClOrdID '{order.ClOrdID}' enviada com sucesso para sessão '{_fixSessionId}'.");
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro inesperado ao tentar enviar mensagem FIX para ClOrdID '{order.ClOrdID}'.");
                throw;
            }
        }
    }
}
