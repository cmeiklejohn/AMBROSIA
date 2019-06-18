
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ambrosia;
using static Ambrosia.StreamCommunicator;
using LocalAmbrosiaRuntime;

namespace Server
{
    /// <summary>
    /// This class runs in the process of the object that implements the interface IServer
    /// and communicates with the local Ambrosia runtime.
    /// It is instantiated in ImmortalFactory.CreateServer when a bootstrapper registers a container
    /// that supports the interface IServer.
    /// </summary>
    class IServer_Dispatcher_Implementation : Immortal.Dispatcher
    {
        private readonly IServer instance;
		private readonly ExceptionSerializer exceptionSerializer = new ExceptionSerializer(new List<Type>());

        public IServer_Dispatcher_Implementation(Immortal z, ImmortalSerializerBase myImmortalSerializer, string serviceName, int receivePort, int sendPort, bool setupConnections)
            : base(z, myImmortalSerializer, serviceName, receivePort, sendPort, setupConnections)
        {
            this.instance = (IServer) z;
        }

        public  IServer_Dispatcher_Implementation(Immortal z, ImmortalSerializerBase myImmortalSerializer, string localAmbrosiaRuntime, Type newInterface, Type newImmortalType, int receivePort, int sendPort)
            : base(z, myImmortalSerializer, localAmbrosiaRuntime, newInterface, newImmortalType, receivePort, sendPort)
        {
            this.instance = (IServer) z;
        }

        public override async Task<bool> DispatchToMethod(int methodId, RpcTypes.RpcType rpcType, string senderOfRPC, long sequenceNumber, byte[] buffer, int cursor)
        {
            switch (methodId)
            {
                case 0:
                    // Entry point
                    await this.EntryPoint();
                    break;
                case 1:
                    // StartRequestAsync
                    {
                        // deserialize arguments

            // arg0: System.DateTime
            var p_0_ValueLength = buffer.ReadBufferedInt(cursor);
cursor += IntSize(p_0_ValueLength);
var p_0_ValueBuffer = new byte[p_0_ValueLength];
Buffer.BlockCopy(buffer, cursor, p_0_ValueBuffer, 0, p_0_ValueLength);
cursor += p_0_ValueLength;
var p_0 = Ambrosia.BinarySerializer.Deserialize<System.DateTime>(p_0_ValueBuffer);


            // arg1: System.Int32
            var p_1_ValueLength = buffer.ReadBufferedInt(cursor);
cursor += IntSize(p_1_ValueLength);
var p_1_ValueBuffer = new byte[p_1_ValueLength];
Buffer.BlockCopy(buffer, cursor, p_1_ValueBuffer, 0, p_1_ValueLength);
cursor += p_1_ValueLength;
var p_1 = Ambrosia.BinarySerializer.Deserialize<System.Int32>(p_1_ValueBuffer);

                        // call the method
						byte[] argExBytes = null;
						int argExSize = 0;
						Exception currEx = null;
						int arg2Size = 0;
						byte[] arg2Bytes = null;

						try 
						{
								await this.instance.StartRequestAsync(p_0,p_1);
						}
						catch (Exception ex)
						{
							currEx = ex;
						}

                        if (!rpcType.IsFireAndForget())
                        {
                            // serialize result and send it back (there isn't one)
                            arg2Size = 0;
                            var wp = this.StartRPC_ReturnValue(senderOfRPC, sequenceNumber, currEx == null ? arg2Size : argExSize, currEx == null ? ReturnValueTypes.EmptyReturnValue : ReturnValueTypes.Exception);

                            this.ReleaseBufferAndSend();
                        }
                    }
                    break;
                case 2:
                    // RecordStateAsync
                    {
                        // deserialize arguments

            // arg0: System.DateTime
            var p_0_ValueLength = buffer.ReadBufferedInt(cursor);
cursor += IntSize(p_0_ValueLength);
var p_0_ValueBuffer = new byte[p_0_ValueLength];
Buffer.BlockCopy(buffer, cursor, p_0_ValueBuffer, 0, p_0_ValueLength);
cursor += p_0_ValueLength;
var p_0 = Ambrosia.BinarySerializer.Deserialize<System.DateTime>(p_0_ValueBuffer);


            // arg1: System.Byte[]
            var p_1_ValueLength = buffer.ReadBufferedInt(cursor);
cursor += IntSize(p_1_ValueLength);
var p_1_ValueBuffer = new byte[p_1_ValueLength];
Buffer.BlockCopy(buffer, cursor, p_1_ValueBuffer, 0, p_1_ValueLength);
cursor += p_1_ValueLength;
var p_1 = p_1_ValueBuffer;

                        // call the method
						byte[] argExBytes = null;
						int argExSize = 0;
						Exception currEx = null;
						int arg2Size = 0;
						byte[] arg2Bytes = null;

						try 
						{
								await this.instance.RecordStateAsync(p_0,p_1);
						}
						catch (Exception ex)
						{
							currEx = ex;
						}

                        if (!rpcType.IsFireAndForget())
                        {
                            // serialize result and send it back (there isn't one)
                            arg2Size = 0;
                            var wp = this.StartRPC_ReturnValue(senderOfRPC, sequenceNumber, currEx == null ? arg2Size : argExSize, currEx == null ? ReturnValueTypes.EmptyReturnValue : ReturnValueTypes.Exception);

                            this.ReleaseBufferAndSend();
                        }
                    }
                    break;
            }

            return true;
        }
    }
}