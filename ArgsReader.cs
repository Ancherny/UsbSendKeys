using System;
using System.Collections.Generic;

public class ArgsReader
{
    private const string argsPrefix = "-args:";
    private const char argsSep = ';';

    private static Dictionary<string, string> GetArgs()
    {
        Dictionary<string, string> argsDict = new Dictionary<string, string>();
        string[] cmdArgs = Environment.GetCommandLineArgs();

        string argsSrc = Array.Find(cmdArgs, arg => arg.StartsWith(argsPrefix));
        if (!string.IsNullOrEmpty(argsSrc))
        {
            argsSrc = argsSrc.Replace(argsPrefix, "");
            string[] args = argsSrc.Split(argsSep);
            foreach (string arg in args)
            {
                string[] parts = arg.Split('=');
                if (parts.Length == 2)
                {
                    argsDict.Add(parts[0], parts[1]);
                }
                else
                {
                    Log.Warning($"Argument '{arg}' is malformed.");
                }
            }
        }
        return argsDict;
    }

    // ReSharper disable once MemberCanBePrivate.Global
    public static string GetString(string argName, string defaultValue=null)
    {
        Dictionary<string, string> customArgsDict = GetArgs();
        string argValue;
        if (!customArgsDict.TryGetValue(argName, out argValue))
        {
            if (defaultValue == null)
            {
                Log.Error($"Cannot get argument '{argName}' from the command line.");
                argValue = string.Empty;
            }
            else
            {
                argValue = defaultValue;
            }
        }

        return argValue;
    }

    public static bool GetBool(string optionName, bool defaultValue)
    {
        string valueStr = GetString(argName:optionName, defaultValue:defaultValue ? "yes" : "no");
        return valueStr.ToLower() == "yes";
    }
}
