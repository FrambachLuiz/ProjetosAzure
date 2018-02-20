using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Configuration;

namespace StorageQueueAzure
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var storageAccount =
                CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);

            var storageClient = storageAccount.CreateCloudQueueClient();

            var cloudQueue = storageClient.GetQueueReference("queuestoragelcruz");

            cloudQueue.CreateIfNotExists();

            for (int i = 1; i < 1000000; i++)
            {
                var cloudQueueMessage = new CloudQueueMessage("Funcionando " + i);
                cloudQueue.AddMessage(cloudQueueMessage);

                var retrieveMessage = cloudQueue.GetMessage();

                Console.WriteLine("Quantidade de linhas ja enviadas: " + retrieveMessage.AsString);

                cloudQueue.DeleteMessage(retrieveMessage);
            }

            // cloudQueue.DeleteMessage(retrieveMessage);
            //5 min 500 iteraçoes
            //30min  3000 iteraçoes
        }
    }
}