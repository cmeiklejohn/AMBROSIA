
using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Ambrosia;
using static Ambrosia.StreamCommunicator;


namespace Client1
{
    /// <summary>
    /// This class is the proxy that runs in the client's process and communicates with the local Ambrosia runtime.
    /// It runs within the client's process, so it is generated in the language that the client is using.
    /// It is returned from ImmortalFactory.CreateClient when a client requests a container that supports the interface IClient1Proxy.
    /// </summary>
    [System.Runtime.Serialization.DataContract]
    public class IClient1Proxy_Implementation : Immortal.InstanceProxy, IClient1Proxy
    {

        public IClient1Proxy_Implementation(string remoteAmbrosiaRuntime, bool attachNeeded)
            : base(remoteAmbrosiaRuntime, attachNeeded)
        {
        }

        async Task
        IClient1Proxy.FinishRequestAsync(System.DateTime p_0)
        {
			 await FinishRequestAsync(p_0);
        }

        async Task
        FinishRequestAsync(System.DateTime p_0)
        {
            SerializableTaskCompletionSource rpcTask;
            // Make call, wait for reply
            // Compute size of serialized arguments
            var totalArgSize = 0;

			int arg0Size = 0;
			byte[] arg0Bytes = null;

            // Argument 0
            arg0Bytes = Ambrosia.BinarySerializer.Serialize<System.DateTime>(p_0);
arg0Size = IntSize(arg0Bytes.Length) + arg0Bytes.Length;

            totalArgSize += arg0Size;

            var wp = this.StartRPC<object>(methodIdentifier: 1 /* method identifier for FinishRequest */, lengthOfSerializedArguments: totalArgSize, taskToWaitFor: out rpcTask);
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
			return;
        }

        void IClient1Proxy.FinishRequestFork(System.DateTime p_0)
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

            var wp = this.StartRPC<object>(1 /* method identifier for FinishRequest */, totalArgSize, out rpcTask, RpcTypes.RpcType.FireAndForget);

            // Serialize arguments


            // Serialize arg0
            wp.curLength += wp.PageBytes.WriteInt(wp.curLength, arg0Bytes.Length);
Buffer.BlockCopy(arg0Bytes, 0, wp.PageBytes, wp.curLength, arg0Bytes.Length);
wp.curLength += arg0Bytes.Length;


            this.ReleaseBufferAndSend();
            return;
        }

        private object
        FinishRequest_ReturnValue(byte[] buffer, int cursor)
        {
            // buffer will be an empty byte array since the method FinishRequest returns void
            // so nothing to read, just getting called is the signal to return to the client
            return this;
        }
    }
}