using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

public class KeySender
{
    private const uint keyDownMsg = 0x0100;

    // Frame time to refresh RC TX state in milliseconds
    private const int frameDuration = 100;

    private readonly IRcTx _rcTx;
    private readonly Process _process;
    private readonly Config.Key[] _keys;

    private IChannelsState _lastState;

    [DllImport("user32.dll")]
    private static extern bool PostMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

    private KeySender(IRcTx rcTx, Process process, Config.Key[] keys)
    {
        _rcTx = rcTx;
        _process = process;
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

            Process [] processes = Process.GetProcessesByName(config.ProcName);
            if (processes.Length <= 0)
            {
                Log.Error($"No running process '{config.ProcName}' found.");
                break;
            }

            if (processes.Length != 1)
            {
                Log.Error($"More than one process '{config.ProcName}' is running.");
                break;
            }

            Process process = processes[0];

            if (config.Keys.Length <= 0)
            {
                Log.Error("No keys defined in config.");
                break;
            }

            keySender = new KeySender(tx, process, config.Keys);
            isSuccess = true;

        } while (false);

        return isSuccess;
    }

    public void StartSending()
    {
        Log.Info("Keys sending started. Press any key to exit.");

        while(!Console.KeyAvailable)
        {
            IChannelsState currentState;
            if (!_rcTx.GetChannelsState(out currentState))
            {
                break;
            }

            bool[] activated;
            if (!currentState.GetActivated(out activated, _keys, _lastState))
            {
                break;
            }

            bool wasError = false;
            foreach (Config.Key key in _keys)
            {
                if (key.ChannelId < 0 || key.ChannelId >= activated.Length)
                {
                    Log.Error("Channel id is out of range.");
                    wasError = true;
                    break;
                }

                if (activated[key.ChannelId])
                {
                    PostMessage(_process.MainWindowHandle, keyDownMsg, (int)key.KeyToPress, 0);
                }
            }
            if (wasError)
            {
                break;
            }

            _lastState = currentState;
            Thread.Sleep(frameDuration);
        }
        Log.Info("Key sending stopped. Exiting.");
    }
}
