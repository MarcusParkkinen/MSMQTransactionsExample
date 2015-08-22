using System.Messaging;

namespace MsmqApp
{
    public class Sender
    {
        public void Send(MessageQueue messageQueue, string message, MessageQueueTransaction transaction)
        {
            try
            {
                var msg = new Message
                {
                    UseAuthentication = false,
                    Recoverable = true,
                    Body = message
                };

                messageQueue.Send(msg, transaction);
            }
            finally
            {
                messageQueue.Close();
            }
        }
    }
}
