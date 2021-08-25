using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

public class KeySender
{
    private const uint keyDownMsg = 0x0100;
    private const int keyU = 0x55;

    private readonly IRcTx _tx;
    private readonly Process _process;

    [DllImport("user32.dll")]
    private static extern bool PostMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

    private KeySender(IRcTx tx, Process process)
    {
        _tx = tx;
        _process = process;
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
            keySender = new KeySender(tx, process);
            isSuccess = true;

        } while (false);

        return isSuccess;
    }

    public void StartSending()
    {
        // while(true)
        // {
        //     PostMessage(proc.MainWindowHandle, keyDownMsg, keyU, 0);
        //     Thread.Sleep(5000);
        // }
    }
}
