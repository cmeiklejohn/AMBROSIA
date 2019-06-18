using Ambrosia;
using Microsoft.VisualStudio.Threading;
using Server;
using Client1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Server
{
    class Program
    {
        [DataContract]
        sealed class Server : Immortal<IServerProxy>, IServer
        {
            [DataMember]
            private string _clientName;

            [DataMember]
            private IClient1Proxy _client;

            [DataMember]
            private bool _shouldSync = true;

            [DataMember]
            private bool _shouldSyncFullState = false;

            [DataMember]
            private int _numRequestsProcessed = 0;

            [DataMember]
            private Dictionary<int, int> _counters = new Dictionary<int, int>();

            public Server(String clientName)
            {
                _clientName = clientName;
            }

            public async Task StartRequestAsync(DateTime sent, int random)
            {
                Console.WriteLine("Processing request...");

                // Update counter.
                int container;

                if (_counters.TryGetValue(random, out container)) {
                    // Remove old.
                    _counters.Remove(random);

                    // Add new.
                    _counters.Add(random, container + 1);
                }
                else
                {
                    _counters.Add(random, 1);
                }

                Console.WriteLine("Counter incremented.");

                if(_shouldSync)
                {
                    if(_shouldSyncFullState) {
                        var serialized = ObjectToByteArray(_counters);
                        Console.WriteLine("Serialized object size: " + serialized.Length);
                        thisProxy.RecordStateFork(sent, serialized);
                    }
                    else
                    {
                        if(_counters.TryGetValue(random, out container))
                        {
                            var serialized = ObjectToByteArray(container);
                            Console.WriteLine("Serialized value object size: " + serialized.Length);
                            thisProxy.RecordStateFork(sent, serialized);
                        }                      
                    }
                }
                else
                {
                    // Respond to the client regarding the request.
                    _client.FinishRequestFork(sent);

                    // Update counter.
                    _numRequestsProcessed++;

                    if (_numRequestsProcessed == 10000)
                    {
                        // Terminate the client.
                        Program.finishedTokenQ.Enqueue(0);
                    }
                }                
            }

            public static byte[] ObjectToByteArray(Object obj)
            {
                BinaryFormatter bf = new BinaryFormatter();
                using (var ms = new MemoryStream())
                {
                    bf.Serialize(ms, obj);
                    return ms.ToArray();
                }
            }

            public async Task RecordStateAsync(DateTime sent, byte[] obj)
            {
                Console.WriteLine("Checkpoint!");

                // Respond to the client regarding the request.
                _client.FinishRequestFork(sent);

                // Update counter.
                _numRequestsProcessed++;

                if (_numRequestsProcessed == 10000)
                {
                    // Terminate the client.
                    Program.finishedTokenQ.Enqueue(0);
                }
            }

            protected override async Task<bool> OnFirstStart()
            {
                _client = GetProxy<IClient1Proxy>(_clientName);

                for(int i = 0; i < 10000; i++)
                {
                    _counters.Add(i, 0);
                }

                return true;
            }
        }

        public static AsyncQueue<int> finishedTokenQ;

        static void Main(string[] args)
        {
            finishedTokenQ = new AsyncQueue<int>();

            int receivePort = 2001;
            int sendPort = 2000;
            string serviceName = "server";
            string clientInstanceName = "client";

            if (args.Length >= 1)
            {
                receivePort = int.Parse(args[0]);
            }
            if (args.Length >= 2)
            {
                sendPort = int.Parse(args[1]);
            }
            if (args.Length == 3)
            {
                serviceName = args[2];
            }
            
            using (var c = AmbrosiaFactory.Deploy<IServer>(serviceName, new Server(clientInstanceName), receivePort, sendPort))
            {
                finishedTokenQ.DequeueAsync().Wait();
            }
        }
    }
}
