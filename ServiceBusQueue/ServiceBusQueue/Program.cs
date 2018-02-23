using Microsoft.ServiceBus.Messaging;
using System;
using System.Configuration;

namespace ServiceBusQueue
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var serviceConnectionString =
                ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"];

            var queueClient = QueueClient.CreateFromConnectionString(serviceConnectionString); ;

            var i = 1;

            //Sending Messages
            while (true)
            {
                var menssage = new BrokeredMessage();

                menssage.ContentType = "mensagem " + i++;

                queueClient.Send(menssage);

                Console.WriteLine(menssage.ContentType + " enviada com sucesso!");
            }

            Console.Read();
        }
    }
}