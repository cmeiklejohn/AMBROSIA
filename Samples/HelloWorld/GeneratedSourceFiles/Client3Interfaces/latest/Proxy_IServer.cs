
using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Ambrosia;
using static Ambrosia.StreamCommunicator;


namespace Server
{
    /// <summary>
    /// This class is the proxy that runs in the client's process and communicates with the local Ambrosia runtime.
    /// It runs within the client's process, so it is generated in the language that the client is using.
    /// It is returned from ImmortalFactory.CreateClient when a client requests a container that supports the interface IServerProxy.
    /// </summary>
    [System.Runtime.Serialization.DataContract]
    public class IServerProxy_Implementation : Immortal.InstanceProxy, IServerProxy
    {

        public IServerProxy_Implementation(string remoteAmbrosiaRuntime, bool attachNeeded)
            : base(remoteAmbrosiaRuntime, attachNeeded)
        {
        }

        async Task<Int32>
        IServerProxy.ReceiveMessageAsync(System.DateTime p_0)
        {
			return await ReceiveMessageAsync(p_0);
        }

        async Task<Int32>
        ReceiveMessageAsync(System.DateTime p_0)
        {
            SerializableTaskCompletionSource rpcTask;
            // Make call, wait for reply
            // Compute size of serialized arguments
            var totalArgSize = 0;

			var p_1 = default(Int32);
			int arg0Size = 0;
			byte[] arg0Bytes = null;

            // Argument 0
            arg0Bytes = Ambrosia.BinarySerializer.Serialize<System.DateTime>(p_0);
arg0Size = IntSize(arg0Bytes.Length) + arg0Bytes.Length;

            totalArgSize += arg0Size;

            var wp = this.StartRPC<Int32>(methodIdentifier: 1 /* method identifier for ReceiveMessage */, lengthOfSerializedArguments: totalArgSize, taskToWaitFor: out rpcTask);
			var asyncContext = new AsyncContext { SequenceNumber = Immortal.CurrentSequenceNumber };

            // Serialize arguments


            // Serialize arg0
            wp.curLength += wp.PageBytes.WriteInt(wp.curLength, arg0Bytes.Length);
Buffer.BlockCopy(arg0Bytes, 0, wp.PageBytes, wp.curLength, arg0Bytes.Length);
wp.curLength += arg0Bytes.Length;

            int taskId;
			lock (Immortal.DispatchTaskIdQueueLock)
            {
                while (!Immortal.DispatchTaskIdQueue.Data.TryDequeue(out taskId)) { }
            }

            ReleaseBufferAndSend();

			Immortal.StartDispatchLoop();

			var taskToWaitFor = Immortal.CallCache.Data[asyncContext.SequenceNumber].GetAwaitableTaskWithAdditionalInfoAsync();
            var currentResult = await taskToWaitFor;

			while (currentResult.AdditionalInfoType != ResultAdditionalInfoTypes.SetResult)
            {
                switch (currentResult.AdditionalInfoType)
                {
                    case ResultAdditionalInfoTypes.SaveContext:
                        await Immortal.SaveTaskContextAsync();
                        taskToWaitFor = Immortal.CallCache.Data[asyncContext.SequenceNumber].GetAwaitableTaskWithAdditionalInfoAsync();
                        break;
                    case ResultAdditionalInfoTypes.TakeCheckpoint:
                        var sequenceNumber = await Immortal.TakeTaskCheckpointAsync();
                        Immortal.StartDispatchLoop();
                        taskToWaitFor = Immortal.GetTaskToWaitForWithAdditionalInfoAsync(sequenceNumber);
                        break;
                }

                currentResult = await taskToWaitFor;
            }

            lock (Immortal.DispatchTaskIdQueueLock)
            {
                Immortal.DispatchTaskIdQueue.Data.Enqueue(taskId);
            }	
			return (Int32) currentResult.Result;
        }

        void IServerProxy.ReceiveMessageFork(System.DateTime p_0)
        {
            SerializableTaskCompletionSource rpcTask;

            // Compute size of serialized arguments
            var totalArgSize = 0;

            // Argument 0
			int arg0Size = 0;
			byte[] arg0Bytes = null;

            arg0Bytes = Ambrosia.BinarySerializer.Serialize<System.DateTime>(p_0);
arg0Size = IntSize(arg0Bytes.Length) + arg0Bytes.Length;

            totalArgSize += arg0Size;

            var wp = this.StartRPC<Int32>(1 /* method identifier for ReceiveMessage */, totalArgSize, out rpcTask, RpcTypes.RpcType.FireAndForget);

            // Serialize arguments


            // Serialize arg0
            wp.curLength += wp.PageBytes.WriteInt(wp.curLength, arg0Bytes.Length);
Buffer.BlockCopy(arg0Bytes, 0, wp.PageBytes, wp.curLength, arg0Bytes.Length);
wp.curLength += arg0Bytes.Length;


            this.ReleaseBufferAndSend();
            return;
        }

        private Int32
        ReceiveMessage_ReturnValue(byte[] buffer, int cursor)
        {
            // deserialize return value
            var returnValue_ValueLength = buffer.ReadBufferedInt(cursor);
cursor += IntSize(returnValue_ValueLength);
var returnValue_ValueBuffer = new byte[returnValue_ValueLength];
Buffer.BlockCopy(buffer, cursor, returnValue_ValueBuffer, 0, returnValue_ValueLength);
cursor += returnValue_ValueLength;
var returnValue = Ambrosia.BinarySerializer.Deserialize<System.Int32>(returnValue_ValueBuffer);

            return returnValue;
        }
    }
}