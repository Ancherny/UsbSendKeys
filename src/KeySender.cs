using System;
using System.Collections.Generic;
using System.Threading;
using WindowsInput;

public class KeySender
{
    // Frame time to refresh RC TX state in milliseconds
    private const int frameDuration = 100;

    private readonly IRcTx _rcTx;
    private readonly Config.Key[] _keys;

    private IChannelsState _lastState;

    private KeySender(IRcTx rcTx, Config.Key[] keys)
    {
        _rcTx = rcTx;
        _keys = keys;
    }

    public static bool Create(out KeySender keySender, string cfgJsonPath)
    {
        keySender = null;
        bool isSuccess = false;
        do
        {
            Config config = new Config();
            if (!config.ReadFromJson(cfgJsonPath))
            {
                break;
            }

            IRcTx tx;
            if (!DirectInputRcTx.CreateByName(out tx, config.TxName))
            {
                break;
            }

            if (config.Keys.Length <= 0)
            {
                Log.Error("No keys defined in config.");
                break;
            }

            keySender = new KeySender(tx, config.Keys);
            isSuccess = true;

        } while (false);

        return isSuccess;
    }

    public void StartSending()
    {
        Log.Info("Keys sending started. Press Esc to exit.");
        InputSimulator inputSimulator = new InputSimulator();

        bool doRepeat = true;
        do
        {
            IChannelsState currentState;
            if (!_rcTx.GetChannelsState(out currentState))
            {
                break;
            }

            List<Config.Key> activated;
            if (!currentState.GetActivated(out activated, _keys, _lastState))
            {
                break;
            }

            foreach (Config.Key key in activated)
            {
                Log.Info($"Key '{key.Name}' is activated");
                inputSimulator.Keyboard.KeyPress(key.KeyToPress);
            }

            _lastState = currentState;
            Thread.Sleep(frameDuration);

            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                doRepeat = key.Key != ConsoleKey.Escape;
            }
            
        } while (doRepeat);
        
        Log.Info("Key sending stopped. Exiting.");
    }
}
