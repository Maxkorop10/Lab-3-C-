using System;
using System.Collections.Generic;
using System.Threading;

namespace CSharp_task
{
    class Program
    {
        private int consumerCount = 0;
        private int producerCount = 0;

        static void Main(string[] args)
        {
            Program program = new Program();
            program.Starter(3, 10);

            Console.ReadKey();
        }

        private void Starter(int storageSize, int itemNumbers)
        {
            Semaphore access = new Semaphore(1, 1);
            int[] count = { 5, 5 };
            Semaphore full = new Semaphore(storageSize, storageSize);
            Semaphore empty = new Semaphore(0, storageSize);

            for (int i = 0; i < count[0]; i++)
            {
                Thread threadConsumer = new Thread(() => Consumer(itemNumbers, access, full, empty));
                threadConsumer.Start();
            }

            for (int i = 0; i < count[1]; i++)
            {
                Thread threadProducer = new Thread(() => Producer(itemNumbers, access, full, empty));
                threadProducer.Start();
            }
        }


        private readonly List<string> storage = new List<string>();

        private void Producer(int maxItem, Semaphore access, Semaphore full, Semaphore empty)
        {
            int currentProducer = Interlocked.Increment(ref producerCount);
            for (int i = 0; i < maxItem; i++)
            {
                full.WaitOne();
                access.WaitOne();

                storage.Add("item " + i);
                Console.WriteLine("Producer " + currentProducer + " added item: " + i);

                access.Release();
                empty.Release();
            }
        }

        private void Consumer(int maxItem, Semaphore access, Semaphore full, Semaphore empty)
        {
            int currentConsumer = Interlocked.Increment(ref consumerCount);
            for (int i = 0; i < maxItem; i++)
            {
                empty.WaitOne();
                Thread.Sleep(1000);
                access.WaitOne();

                string item = storage[0];
                storage.RemoveAt(0);

                full.Release();
                access.Release();

                Console.WriteLine("Consumer " + currentConsumer + " took " + item);
            }
        }
    }
}
