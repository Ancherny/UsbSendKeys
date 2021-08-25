using System;
using System.IO;
using SimpleJSON;

internal struct Config
{
    private const string deviceNameField = "device_name";
    private const string procNameField = "proc_name";
    private const string keysField = "keys";
    private const string codeField = "code";
    private const string channelField = "channel";
    private const string fromField = "from";
    private const string toField = "to";
    
    public struct Key
    {
        // Code of the keypress to pass to the process
        public int Code { get; set; }
        
        // Game controller channel id
        public int Channel { get; set; }
        
        // Game controller channel range to send keypress on enter
        public int From { get; set; }
        public int To { get; set; }
    }

    // Name of the game controller device to read 
    public string DeviceName { get; set; }

    // Name of the running process to send keypresses to 
    public string ProcName { get; set; }
    
    // Keypresses mapping
    public Key[] Keys { get; set; }

    public static bool ReadFromJson(out Config config, string jsonPath)
    {
        config = new Config();
        bool isSuccess = false;
        do
        {
            if (!File.Exists(jsonPath))
            {
                Log.Error("Config file does not exist: " + jsonPath);
                break;
            }

            JSONNode root;
            try
            {
                root = JSONNode.Parse(File.ReadAllText(jsonPath));
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
                break;
            }

            if (root == null)
            {
                break;
            }
            
            config.DeviceName = root[deviceNameField];
            if (string.IsNullOrEmpty(config.DeviceName))
            {
                Log.Error("Cannot get device name from config.");
                break;
            }

            config.ProcName = root[procNameField];
            if (string.IsNullOrEmpty(config.ProcName))
            {
                Log.Error("Cannot get process name from config.");
                break;
            }

            isSuccess = true;

        } while (false);

        if (!isSuccess)
        {
            Log.Error("Failed to parse config file at: " + jsonPath);
        }
        return isSuccess;
    }
}
