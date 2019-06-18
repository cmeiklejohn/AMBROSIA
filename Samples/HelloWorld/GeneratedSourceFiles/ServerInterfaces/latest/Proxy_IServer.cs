
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

        async Task
        IServerProxy.StartRequestAsync(System.DateTime p_0,System.Int32 p_1)
        {
			 await StartRequestAsync(p_0,p_1);
        }

        async Task
        StartRequestAsync(System.DateTime p_0,System.Int32 p_1)
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
			int arg1Size = 0;
			byte[] arg1Bytes = null;

            // Argument 1
            arg1Bytes = Ambrosia.BinarySerializer.Serialize<System.Int32>(p_1);
arg1Size = IntSize(arg1Bytes.Length) + arg1Bytes.Length;

            totalArgSize += arg1Size;

            var wp = this.StartRPC<object>(methodIdentifier: 1 /* method identifier for StartRequest */, lengthOfSerializedArguments: totalArgSize, taskToWaitFor: out rpcTask);
			var asyncContext = new AsyncContext { SequenceNumber = Immortal.CurrentSequenceNumber };

            // Serialize arguments


            // Serialize arg0
            wp.curLength += wp.PageBytes.WriteInt(wp.curLength, arg0Bytes.Length);
Buffer.BlockCopy(arg0Bytes, 0, wp.PageBytes, wp.curLength, arg0Bytes.Length);
wp.curLength += arg0Bytes.Length;


            // Serialize arg1
            wp.curLength += wp.PageBytes.WriteInt(wp.curLength, arg1Bytes.Length);
Buffer.BlockCopy(arg1Bytes, 0, wp.PageBytes, wp.curLength, arg1Bytes.Length);
wp.curLength += arg1Bytes.Length;

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

        void IServerProxy.StartRequestFork(System.DateTime p_0,System.Int32 p_1)
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
            // Argument 1
			int arg1Size = 0;
			byte[] arg1Bytes = null;

            arg1Bytes = Ambrosia.BinarySerializer.Serialize<System.Int32>(p_1);
arg1Size = IntSize(arg1Bytes.Length) + arg1Bytes.Length;

            totalArgSize += arg1Size;

            var wp = this.StartRPC<object>(1 /* method identifier for StartRequest */, totalArgSize, out rpcTask, RpcTypes.RpcType.FireAndForget);

            // Serialize arguments


            // Serialize arg0
            wp.curLength += wp.PageBytes.WriteInt(wp.curLength, arg0Bytes.Length);
Buffer.BlockCopy(arg0Bytes, 0, wp.PageBytes, wp.curLength, arg0Bytes.Length);
wp.curLength += arg0Bytes.Length;


            // Serialize arg1
            wp.curLength += wp.PageBytes.WriteInt(wp.curLength, arg1Bytes.Length);
Buffer.BlockCopy(arg1Bytes, 0, wp.PageBytes, wp.curLength, arg1Bytes.Length);
wp.curLength += arg1Bytes.Length;


            this.ReleaseBufferAndSend();
            return;
        }

        private object
        StartRequest_ReturnValue(byte[] buffer, int cursor)
        {
            // buffer will be an empty byte array since the method StartRequest returns void
            // so nothing to read, just getting called is the signal to return to the client
            return this;
        }

        void IServerProxy.RecordStateFork(System.DateTime p_0,System.Byte[] p_1)
        {
			if (!Immortal.IsPrimary)
			{
                throw new Exception("Unable to send an Impulse RPC while not being primary.");
			}

            SerializableTaskCompletionSource rpcTask;

            // Compute size of serialized arguments
            var totalArgSize = 0;

            // Argument 0
			int arg0Size = 0;
			byte[] arg0Bytes = null;

            arg0Bytes = Ambrosia.BinarySerializer.Serialize<System.DateTime>(p_0);
arg0Size = IntSize(arg0Bytes.Length) + arg0Bytes.Length;

            totalArgSize += arg0Size;
            // Argument 1
			int arg1Size = 0;
			byte[] arg1Bytes = null;

            arg1Bytes = p_1;
arg1Size = IntSize(arg1Bytes.Length) + arg1Bytes.Length;

            totalArgSize += arg1Size;

            var wp = this.StartRPC<object>(2 /* method identifier for RecordState */, totalArgSize, out rpcTask, RpcTypes.RpcType.Impulse);

            // Serialize arguments


            // Serialize arg0
            wp.curLength += wp.PageBytes.WriteInt(wp.curLength, arg0Bytes.Length);
Buffer.BlockCopy(arg0Bytes, 0, wp.PageBytes, wp.curLength, arg0Bytes.Length);
wp.curLength += arg0Bytes.Length;


            // Serialize arg1
            wp.curLength += wp.PageBytes.WriteInt(wp.curLength, arg1Bytes.Length);
Buffer.BlockCopy(arg1Bytes, 0, wp.PageBytes, wp.curLength, arg1Bytes.Length);
wp.curLength += arg1Bytes.Length;


            this.ReleaseBufferAndSend();
            return;
        }

        private object
        RecordState_ReturnValue(byte[] buffer, int cursor)
        {
            // buffer will be an empty byte array since the method RecordState returns void
            // so nothing to read, just getting called is the signal to return to the client
            return this;
        }
    }
}