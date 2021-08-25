using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

internal static class Program
{
    private const string cfgPathDefault = @".\config.json";
    private const string cfgPathArg = "cfg_path";
    private const uint keyDownMsg = 0x0100;
    private const int keyU = 0x55;

    [DllImport("user32.dll")]
    private static extern bool PostMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

    private static bool ReadJsonConfig(string cfgJsonPath)
    {
        return true;
    }

    [STAThread]
    private static void Main()
    {
        do
        {
            string cfgJsonPath;
            if (!ArgsReader.GetString(out cfgJsonPath, cfgPathArg))
            {
                if (!File.Exists(cfgPathDefault))
                {
                    Log.Error("No config path defined.");
                    Log.Info("Expected command line:");
                    Log.Info("-args:cfg_path=<path>");
                    Log.Info("  where cfg_path defines path to the json config file");
                    break;
                }

                cfgJsonPath = cfgPathDefault;
            }

            Config config;
            if (!Config.ReadFromJson(out config, cfgJsonPath))
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
            Log.Info("Success!");
            break;

            Process proc = processes[0];
            while(true)
            {
                PostMessage(proc.MainWindowHandle, keyDownMsg, keyU, 0);
                Thread.Sleep(5000);
            }
            
        } while (false);
    }
}
