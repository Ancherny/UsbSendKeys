using System;

public interface IRcTx : IDisposable
{
    // Polls current channels' state.
    // Channel state is standard RC TX microseconds range of (1000:2000) 
    bool GetChannelsState(out int[] state);
}
