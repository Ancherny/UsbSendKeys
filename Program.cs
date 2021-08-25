using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

internal static class Program
{
    private const string freeRiderProcessName = "FPVFreerider_Recharged";
    private const uint keyDownMsg = 0x0100;
    private const int keyU = 0x55;

    [DllImport("user32.dll")]
    private static extern bool PostMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

    [STAThread]
    private static void Main(string[] args)
    {
        Process [] processes = Process.GetProcessesByName(freeRiderProcessName);
        while(true)
        {
            foreach (Process proc in processes)
            {
                PostMessage(proc.MainWindowHandle, keyDownMsg, keyU, 0);
            }
            Thread.Sleep(5000);
        }
        // ReSharper disable once FunctionNeverReturns
    }
}
