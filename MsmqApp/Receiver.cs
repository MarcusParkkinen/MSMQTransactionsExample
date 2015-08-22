using System;
using System.Collections.Generic;
using System.Messaging;

namespace MsmqApp
{
    public class Receiver
    {
        public void StartReading(List<MessageQueue> messageQueues)
        {
            foreach (MessageQueue messageQueue in messageQueues)
            {
                messageQueue.PeekCompleted += MessageAvailable;
                messageQueue.BeginPeek();
            }
        }

        private void MessageAvailable(object source, PeekCompletedEventArgs args)
        {
            var messageQueue = (MessageQueue) source;

            try
            {
                ReadMessage(messageQueue);
            }
            finally
            {
                messageQueue.Close();
            }

            // Continue reading from the queue
            messageQueue.BeginPeek();
        }

        private void ReadMessage(MessageQueue messageQueue)
        {
            // We interpret the queue messages as strings
            messageQueue.Formatter =
                new XmlMessageFormatter(new[] {typeof (string)});

            using (var transaction = new MessageQueueTransaction())
            {
                transaction.Begin();

                var message = messageQueue.Receive(transaction);
                var messageContents = message == null ?
                    string.Empty : message.Body.ToString();

                Console.WriteLine("Received message \"{0}\" from queue \"{1}\"",
                    messageContents, messageQueue.QueueName);

                transaction.Commit();
            }
        }
    }
}
