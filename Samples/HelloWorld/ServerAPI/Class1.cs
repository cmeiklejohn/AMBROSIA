using Ambrosia;
using System;
using System.Collections.Generic;

namespace Server
{
    public interface IServer
    {
        void StartRequest(DateTime sent, int random);

        [ImpulseHandler]
        void RecordState(DateTime sent, byte[] obj);
    }
}
