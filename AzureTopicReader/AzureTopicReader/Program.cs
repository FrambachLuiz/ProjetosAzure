using Microsoft.ServiceBus.Messaging;
using System;
using System.Configuration;

namespace AzureTopicReader
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var connectionString =
                ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"];

            while (true)
            {
                ReadMessages(connectionString);
            }
        }

        private static void ReadMessages(string connectionString)
        {
            var topiClient = SubscriptionClient.CreateFromConnectionString(connectionString, "topictest", "subscriber1");

            //ReadingMessages
            while (true)
            {
                var menssage = topiClient.Receive();
                topiClient.Complete(menssage.LockToken);

                Console.WriteLine(menssage.ContentType + " recebida com sucesso!");
            }
        }
    }
}