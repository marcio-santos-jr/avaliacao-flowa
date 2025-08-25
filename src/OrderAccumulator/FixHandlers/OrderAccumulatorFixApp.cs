using QuickFix.Fields;
using QuickFix;
using OrderAccumulator.Models;     
using OrderAccumulator.Interfaces.Service;
using OrderAccumulator.Models.Enum;

namespace OrderAccumulator.FixHandlers
{
    public class OrderAccumulatorFixApp : IApplication
    {
        private readonly ILogger<OrderAccumulatorFixApp> _logger;
        private readonly IOrderService _orderService; 
        private readonly IFixMessageService _fixMessageService; 

        public OrderAccumulatorFixApp(ILogger<OrderAccumulatorFixApp> logger, IOrderService orderService, IFixMessageService fixMessageService)
        {
            _logger = logger;
            _orderService = orderService;
            _fixMessageService = fixMessageService;
        }

        public void OnCreate(SessionID sessionID) { _logger.LogInformation($"Sessão criada: {sessionID}"); }
        public void OnLogon(SessionID sessionID) { _logger.LogInformation($"Sessão logon: {sessionID}"); }
        public void OnLogout(SessionID sessionID) { _logger.LogInformation($"Sessão logout: {sessionID}"); }
        public void ToAdmin(Message message, SessionID sessionID) { _logger.LogInformation($"Para Admin ({sessionID}): {message}"); }
        public void ToApp(Message message, SessionID sessionID) { _logger.LogInformation($"Para App ({sessionID}): {message}"); }
        public void FromAdmin(Message message, SessionID sessionID) { _logger.LogInformation($"De Admin ({sessionID}): {message}"); }

        public void FromApp(Message message, SessionID sessionID)
        {
            try
            {
                MsgType msgType = new();
                message.Header.GetField(msgType);

                _logger.LogInformation($"Mensagem de aplicação recebida: {msgType.Value} de sessão {sessionID}");

                if (msgType.Value == QuickFix.FIX44.NewOrderSingle.MsgType)
                {
                    QuickFix.FIX44.NewOrderSingle newOrderFix = (QuickFix.FIX44.NewOrderSingle)message;
                    _ = ProcessNewOrderSingleAsync(newOrderFix, sessionID);
                }
                else
                {
                    _logger.LogWarning($"Mensagem de aplicação desconhecida ou não tratada: {msgType.Value} da sessão {sessionID}. " +
                                       $"Enviando MessageReject.");

                    var reject = new QuickFix.FIX44.Reject(new RefSeqNum(message.Header.GetField(new MsgSeqNum()).Value));
                    reject.Set(new Text($"Unsupported message type: {msgType.Value}"));
                    _fixMessageService.Send(reject, sessionID);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro inesperado ao processar mensagem de aplicação da sessão {sessionID}. Mensagem: {message}");
            }
        }

        private async Task ProcessNewOrderSingleAsync(QuickFix.FIX44.NewOrderSingle newOrderFix, SessionID sessionId)
        {
            try
            {
                var order = new Order
                {
                    ClOrdID = newOrderFix.ClOrdID.Value,
                    Symbol = newOrderFix.Symbol.Value,
                    Quantity = newOrderFix.OrderQty.Value,
                    Price = newOrderFix.Price.Value,
                    Side = newOrderFix.Side.Value == Side.BUY ? OrderSide.Buy : OrderSide.Sell
                };

                var result = await _orderService.ProcessOrderAsync(order);

                QuickFix.FIX44.ExecutionReport execReport;

                if (result.IsAccepted)
                {
                    execReport = CreateExecutionReport(result, ExecType.NEW, OrdStatus.NEW, "");
                    _logger.LogInformation($"Ordem {result.ClOrdID} aceita para {result.Symbol}. Lado: {result.Side}. Nova exposição: {result.NewProjectedExposure:N2}.");
                }
                else
                {
                    execReport = CreateExecutionReport(result, ExecType.REJECTED, OrdStatus.REJECTED, result.RejectionReason);
                    _logger.LogWarning($"Ordem {result.ClOrdID} rejeitada: {result.RejectionReason}");
                }

                _fixMessageService.Send(execReport, sessionId);
                _logger.LogInformation($"Mensagem ExecutionReport (ExecType: {execReport.ExecType.Value}, OrdStatus: {execReport.OrdStatus.Value}) para ClOrdID '{result.ClOrdID}' enviada para {sessionId}.");
            }
            catch (SessionNotFound snag)
            {
                _logger.LogError($"Sessão FIX não encontrada para enviar ExecutionReport: {snag.Message}. ClOrdID: {newOrderFix.ClOrdID.Value}.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro inesperado ao processar NewOrderSingle para ClOrdID '{newOrderFix.ClOrdID.Value}'.");
            }
        }

        private QuickFix.FIX44.ExecutionReport CreateExecutionReport(OrderProcessingResult result, char execType, char ordStatus, string textReason)
        {
            var report = new QuickFix.FIX44.ExecutionReport(
                new OrderID(Guid.NewGuid().ToString()), 
                new ExecID(Guid.NewGuid().ToString()), 
                new ExecType(execType),
                new OrdStatus(ordStatus),
                new Symbol(result.Symbol),
                new Side(result.Side == OrderSide.Buy ? Side.BUY : Side.SELL),
                new LeavesQty(result.Quantity),
                new CumQty(result.IsAccepted ? result.Quantity : 0), 
                new AvgPx(result.IsAccepted ? result.Price : 0) 
            );
            report.Set(new ClOrdID(result.ClOrdID));
            report.Set(new TransactTime(DateTime.UtcNow));
            report.Set(new LastQty(result.IsAccepted ? result.Quantity : 0)); 
            report.Set(new LastPx(result.IsAccepted ? result.Price : 0)); 

            if (!string.IsNullOrEmpty(textReason))
            {
                report.Set(new Text(textReason));
            }
            return report;
        }
    }
}