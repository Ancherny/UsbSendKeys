using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

internal static class Program
{
    private const string procNameArg = "proc_name";
    private const uint keyDownMsg = 0x0100;
    private const int keyU = 0x55;

    private static string procName = null;

    [DllImport("user32.dll")]
    private static extern bool PostMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

    private static void GiveHelp()
    {
        Log.Info("Expected command line:");
        Log.Info("-args:img=<process_name>");
        Log.Info("  where process_name defines running process to send keystrokes to");
    }

    private static bool ParseArguments()
    {
        bool isSuccess = false;
        do
        {
            if (!ArgsReader.GetString(out procName, procNameArg))
            {
                Log.Error("No process name defined.");
                break;
            }
            
            isSuccess = true;

        } while (false);

        return isSuccess;
    }
    
    [STAThread]
    private static void Main()
    {
        do
        {
            if (!ParseArguments())
            {
                GiveHelp();
                break;
            }
            
            Process [] processes = Process.GetProcessesByName(procName);
            if (processes.Length <= 0)
            {
                Log.Error($"No running process '{procName}' found.");
                break;
            }

            if (processes.Length != 1)
            {
                Log.Error($"More than one process '{procName}' is running.");
                break;
            }

            Process proc = processes[0];
            while(true)
            {
                PostMessage(proc.MainWindowHandle, keyDownMsg, keyU, 0);
                Thread.Sleep(5000);
            }
            
        } while (false);
    }
}
