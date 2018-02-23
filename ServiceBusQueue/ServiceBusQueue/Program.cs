using Microsoft.ServiceBus.Messaging;
using System;
using System.Configuration;

namespace ServiceBusQueue
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var connectionString =
                ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"];

            ServiceBusSendToTopic(connectionString);

            Console.Read();
        }

        public static void ServiceBusSendToQueue(string connectionString)
        {
            var queueClient = QueueClient.CreateFromConnectionString(connectionString, "testequeue"); ;

            var i = 1;

            //Sending Messages
            while (true)
            {
                var menssage = new BrokeredMessage();

                menssage.ContentType = "mensagem " + i++;

                queueClient.Send(menssage);

                Console.WriteLine(menssage.ContentType + " enviada com sucesso!");
            }
        }

        public static void ServiceBusSendToTopic(string connectionString)
        {
            var topicClient = TopicClient.CreateFromConnectionString(connectionString, "topictest");

            var i = 1;

            //Sending Messages
            while (true)
            {
                var menssage = new BrokeredMessage();

                menssage.ContentType = "mensagem " + i++;

                topicClient.Send(menssage);

                Console.WriteLine(menssage.ContentType + " enviada com sucesso!");
            }
        }
    }
}