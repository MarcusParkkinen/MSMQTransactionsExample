using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;

namespace MsmqApp
{
    public class Program
    {
        private static void Main(string[] args)
        {
            const string message = "Testing MSMQ";

            List<MessageQueue> queues = CreateQueues(".\\private$\\FirstQueue",
                ".\\private$\\SecondQueue",
                ".\\private$\\ThirdQueue");

            var sender = new Sender();
            var receiver = new Receiver();

            /*
             * Send to all the queues within a transaction
             */
            using (var transaction = new MessageQueueTransaction())
            {
                transaction.Begin();

                foreach (MessageQueue queue in queues)
                {
                    sender.Send(queue, message, transaction);
                }

                transaction.Commit();
            }

            /*
             * Read from all the queues
             */
            receiver.StartReading(queues);

            Console.ReadKey();
        }

        private static List<MessageQueue> CreateQueues(params string[] queuePaths)
        {
            return
                queuePaths.Select(
                    queuePath =>
                        MessageQueue.Exists(queuePath)
                            ? new MessageQueue(queuePath)
                            : MessageQueue.Create(queuePath, true)).ToList();
        }
    }
}