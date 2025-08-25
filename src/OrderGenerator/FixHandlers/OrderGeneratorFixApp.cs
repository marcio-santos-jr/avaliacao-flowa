using OrderGenerator.Interfaces.Repositories;
using OrderGenerator.Models.Enum;
using QuickFix;
using QuickFix.Fields;

namespace OrderGenerator.FixHandlers
{
    public class OrderGeneratorFixApp : MessageCracker, IApplication
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<OrderGeneratorFixApp> _logger;
        private Session? _session = null;
        public OrderGeneratorFixApp(IOrderRepository orderRepository, ILogger<OrderGeneratorFixApp> logger)
        {
            _orderRepository = orderRepository;
            _logger = logger;
        }

        public void OnCreate(SessionID sessionId)
        {
            _session = Session.LookupSession(sessionId);
            if (_session is null)
                throw new ApplicationException("Somehow session is not found");
        }
        public void OnLogon(SessionID sessionId) { _logger.LogInformation($"Sessão logon: {sessionId}"); }
        public void OnLogout(SessionID sessionId) { _logger.LogInformation($"Sessão logout: {sessionId}"); }
        public void ToAdmin(Message message, SessionID sessionId) { _logger.LogInformation($"To Admin ({sessionId}): {message}"); }
        public void ToApp(Message message, SessionID sessionId)
        {
            _logger.LogInformation($"To App ({sessionId}): {message}");
            try
            {
                bool possDupFlag = false;
                if (message.Header.IsSetField(Tags.PossDupFlag))
                {
                    possDupFlag = message.Header.GetBoolean(Tags.PossDupFlag);
                }
                if (possDupFlag)
                    throw new DoNotSend();
            }
            catch (FieldNotFoundException)
            { }

            Console.WriteLine();
            Console.WriteLine("OUT: " + message.ConstructString());
        }
        public void FromAdmin(Message message, SessionID sessionId) { _logger.LogInformation($"De Admin ({sessionId}): {message}"); }

        public void FromApp(Message message, SessionID sessionId)
        {
            try
            {
                Crack(message, sessionId);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Cracker exception");
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
            }
        }

        public bool SendMessage(Message m)
        {
            if (_session is not null)
               return _session.Send(m);
            else
            {
                Console.WriteLine("Can't send message: session not created.");
                return false;
            }
        }

        #region MessageCracker handlers
        public void OnMessage(QuickFix.FIX44.ExecutionReport execReport, SessionID s)
        {
            OrderStatus newStatus;
            string rejectionReason = null;

            if (execReport.ExecType.Value == ExecType.NEW) 
            {
                newStatus = OrderStatus.Accepted;
            }
            else if (execReport.ExecType.Value == ExecType.REJECTED) 
            {
                newStatus = OrderStatus.Rejected;
                if (execReport.IsSetText())
                {
                    rejectionReason = execReport.Text.Value;
                }
            }
            else
            {
                newStatus = OrderStatus.Pending;
            }

            _orderRepository.UpdateOrderStatus(execReport.ClOrdID.Value, newStatus, rejectionReason).Wait();
        }
        #endregion
    }
}
