using System.Collections.Generic;

public interface IChannelsState
{
    // Compares current state and given last state and returns bool for each channel:
    // True - channel state was changed to active from inactive.
    // False - channel state was not changed. 
    bool GetActivated(out List<Config.Key> activated, Config.Key[] keys, IChannelsState lastChannelsState);
}
