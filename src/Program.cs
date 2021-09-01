using System;
using System.IO;

internal static class Program
{
    private const string cfgPathDefault = @".\config.json";
    private const string cfgPathArg = "cfg_path";

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

            Log.Info("Reading config file: " + cfgJsonPath);
            
            KeySender keySender;
            if (!KeySender.Create(out keySender, cfgJsonPath))
            {
                break;
            }
            keySender.StartSending();
            
        } while (false);

        Log.Info("Press any key to exit.");
        while (!Console.KeyAvailable)
        {
        }
    }
}
