using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
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

            var cloudQueue = storageClient.GetQueueReference("queuestoragelcruz");

            cloudQueue.CreateIfNotExists();

            cloudQueue.FetchAttributes();

            var option = new ParallelOptions
            {
                MaxDegreeOfParallelism = 5
            };

            #region solution 1

            var allMessages = new List<Guid>();

            while (true)
            {
                var teste = new TaskFactory();

                var taskList = new List<Task>();

                var batch = new List<CloudQueueMessage>();

                var watch = Stopwatch.StartNew();

                Parallel.For(1, 9, option, i =>
                {
                    taskList.Add(teste.StartNew(() =>
                    {
                        var messages = Task.Run(() => cloudQueue.GetMessagesAsync(32)).Result;

                        batch.AddRange(messages);
                    }));
                });

                Task.WaitAll(taskList.ToArray());

                #endregion solution 1

                Console.WriteLine(Environment.NewLine);

                Console.WriteLine("Count: " + batch.Count);

                Console.WriteLine("TempoDecorrido: " + watch.Elapsed);

                foreach (var msg in batch)
                {
                    var currentId = Guid.Parse(msg.Id);

                    if (allMessages.Any(x => x == currentId))
                    {
                        Console.WriteLine("DUPLICTY !!!");
                        Console.ReadKey();
                    }

                    allMessages.Add(currentId);

                    Console.WriteLine(msg.Id);
                }

                Thread.Sleep(100);
            }

            Console.Read();
        }

        private static void InsertInQueue(CloudQueue queue)
        {
            var cloudQueueMessage = new CloudQueueMessage("{\"PaymentId\":\"492d1012-52b4-4e11-99bc-579636fcf57e\",\"ChangeType\":1} ");
            queue.AddMessage(cloudQueueMessage);

            Console.WriteLine("Adicionando: " + cloudQueueMessage);
        }

        private static void InsertParallel(CloudQueue queue)
        {
            var option = new ParallelOptions
            {
                MaxDegreeOfParallelism = 5
            };

            Parallel.For(1, 1500000, option, i =>
            {
                var cloudQueueMessage = new CloudQueueMessage("{\"PaymentId\":\"492d1012-52b4-4e11-99bc-579636fcf57e\",\"ChangeType\":1} numero: " + i);
                queue.AddMessage(cloudQueueMessage);

                Console.WriteLine("Adicionando: " + cloudQueueMessage);
            });
        }

        private static CloudQueueMessage GetMessage(CloudQueue queue)
        {
            return queue.GetMessage(); //Message have 30 sec lifetime ,  and go back to queue
        }

        private static CloudQueueMessage GetAndDeleteMessage(CloudQueue queue)
        {
            var message = queue.GetMessage();

            queue.DeleteMessage(message);

            return message;
        }

        private static IEnumerable<CloudQueueMessage> GetMessages(CloudQueue queue)
        {
            var messages = queue.GetMessages(20);

            foreach (var message in messages)
            {
                queue.DeleteMessage(message);
            }

            return messages;
        }

    }
}