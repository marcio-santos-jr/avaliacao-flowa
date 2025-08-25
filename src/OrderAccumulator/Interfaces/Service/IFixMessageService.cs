using QuickFix;

namespace OrderAccumulator.Interfaces.Service;

public interface IFixMessageService
{
    void Send(Message message, SessionID sessionID);
}
