using Sandboxable.Microsoft.WindowsAzure.Storage;
using Sandboxable.Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;

namespace StorageQueueAzure
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var storageAccount =
                CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);

            var storageClient = storageAccount.CreateCloudQueueClient();

            var cloudQueue = storageClient.GetQueueReference("stresstest");

            cloudQueue.CreateIfNotExists();

            Console.WriteLine("Starting...");

            while (true)
            {
                var taskFactory = new TaskFactory();
                var tasks = new List<Task>();

                cloudQueue.FetchAttributes();
                var total = cloudQueue.ApproximateMessageCount;

                while (total > 100) // threshold
                {
                    tasks.Add(taskFactory.StartNew(() =>
                    {
                        var messages = Task.Run(() => cloudQueue.GetMessagesAsync(32)).Result;

                        Task.Run(() => DeleteAllMessages(cloudQueue, messages));
                    }));

                    cloudQueue.FetchAttributes();
                    total = cloudQueue.ApproximateMessageCount;
                }

                Console.WriteLine("Total of tasks: " + tasks.Count);

                Task.WaitAll(tasks.ToArray());

                Thread.Sleep(100);
            }
        }

        private static void DeleteAllMessages(CloudQueue queue, IEnumerable<CloudQueueMessage> allQueueMessages)
        {
            foreach (var msg in allQueueMessages)
            {
                queue.DeleteMessageAsync(msg);
            }
        }
    }
}