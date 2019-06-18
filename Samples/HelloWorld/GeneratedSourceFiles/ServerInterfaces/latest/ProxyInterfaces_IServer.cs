
using System;
using Ambrosia;
using System.Threading.Tasks;
using static Ambrosia.StreamCommunicator;

namespace Server
{
    /// <summary>
    // Generated from IServer by the proxy generation.
    // This is the API that any immortal implementing the interface must be a subtype of.
    /// </summary>
    public interface IServer
    {
        Task StartRequestAsync(System.DateTime p_0,System.Int32 p_1);
        Task RecordStateAsync(System.DateTime p_0,System.Byte[] p_1);
    }

    /// <summary>
    // Generated from IServer by the proxy generation.
    // This is the API that is used to call a immortal that implements
    /// </summary>
    [Ambrosia.InstanceProxy(typeof(IServer))]
    public interface IServerProxy
    {
        Task StartRequestAsync(System.DateTime p_0,System.Int32 p_1);
        void StartRequestFork(System.DateTime p_0,System.Int32 p_1);
        void RecordStateFork(System.DateTime p_0,System.Byte[] p_1);
    }
}