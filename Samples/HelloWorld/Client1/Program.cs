using Ambrosia;
using Client1;
using Server;
using Microsoft.VisualStudio.Threading;
using System;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace Client1
{
    [DataContract]
    class Client1 : Immortal<IClient1Proxy>, IClient1
    {
        [DataMember]
        private string _serverName;

        [DataMember]
        private IServerProxy _server;

        [DataMember]
        private int _numRequestsSent = 0;

        [DataMember]
        private Random _random;

        [DataMember]
        private List<double> _observations = new List<double>();

        public Client1(string serverName)
        {
            _serverName = serverName;
        }

        public async Task FinishRequestAsync(DateTime issued)
        {       
            // Get time when the request was finished.
            var now = DateTime.UtcNow;
            var difference = now - issued;

            Console.WriteLine("Request finished, time: " + difference);

            // Generate random number between 1 and 10000.
            var random = _random.Next(1, 10000);

            // Log observation.
            _observations.Add(difference.TotalMilliseconds);

            if(_numRequestsSent < 10000)
            {
                Console.WriteLine("Issuing next request: " + (_numRequestsSent + 1));

                // Issue next request.
                _server.StartRequestFork(DateTime.UtcNow, random);

                // Update counter.
                _numRequestsSent++;
            }
            else
            {
                // Print out statistics.
                var average = _observations.Take(10000).Average();
                Console.WriteLine("Average (ms): " + average);

                // Terminate the client.
                Program.finishedTokenQ.Enqueue(0);
            }
        }

        protected override async Task<bool> OnFirstStart()
        {
            // Get the server proxy.
            _server = GetProxy<IServerProxy>(_serverName);

            // Initialize random number generator.
            _random = new Random();

            // Generate random number between 1 and 10000.
            var random = _random.Next(1, 10000);

            Console.WriteLine("Issuing FIRST request.");

            // Start the initial request.
            _server.StartRequestFork(DateTime.UtcNow, random);

            // Update counter.
            _numRequestsSent++;
            
            return true;
        }
    }
    class Program
    {
        public static AsyncQueue<int> finishedTokenQ;

        static void Main(string[] args)
        {
            finishedTokenQ = new AsyncQueue<int>();

            int receivePort = 1001;
            int sendPort = 1000;
            string clientInstanceName = "client";
            string serverInstanceName = "server";

            if (args.Length >= 1)
            {
                clientInstanceName = args[0];
            }

            if (args.Length == 2)
            {
                serverInstanceName = args[1];
            }

            using (var c = AmbrosiaFactory.Deploy<IClient1>(clientInstanceName, new Client1(serverInstanceName), receivePort, sendPort))
            {
                finishedTokenQ.DequeueAsync().Wait();
            }
        }
    }
}
