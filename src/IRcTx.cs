using System;

public interface IRcTx : IDisposable
{
    bool GetChannelsState(out IChannelsState channelsState);
}
