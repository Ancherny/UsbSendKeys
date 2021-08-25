using System;
using System.IO;

internal static class Program
{
    private const string cfgPathDefault = @".\config.json";
    private const string cfgPathArg = "cfg_path";

    [STAThread]
    private static void Main()
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
                return;
            }

            cfgJsonPath = cfgPathDefault;
        }

        KeySender keySender;
        if (!KeySender.Create(out keySender, cfgJsonPath))
        {
            return;
        }
        keySender.StartSending();
    }
}
