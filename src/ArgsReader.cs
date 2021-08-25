using System;
using System.Collections.Generic;

internal static class ArgsReader
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
    public static bool GetString(out string value, string argName, string defaultValue=null)
    {
        value = null;
        Dictionary<string, string> customArgsDict = GetArgs();
        if (customArgsDict.TryGetValue(argName, out value))
        {
            return true;
        }

        if (defaultValue != null)
        {
            value = defaultValue;
            return true;
        }
        return false;
    }

    public static bool GetBool(out bool value, string argName, bool defaultValue=false)
    {
        string arg;
        if (GetString(out arg, argName))
        {
            arg = arg.ToLower();
            value = arg == "true" ||
                    arg == "on" ||
                    arg == "yes";
            return true;
        }

        value = defaultValue;
        return false;
    }
}
