using OrderAccumulator.Interfaces.Service;
using QuickFix;

namespace OrderAccumulator.Services
{
    public class FixMessageService : IFixMessageService
    {
        public void Send(Message message, SessionID sessionID)
        {
            Session.SendToTarget(message, sessionID);
        }
    }
}
