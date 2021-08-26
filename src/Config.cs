using System;
using System.IO;
using SimpleJSON;

public struct Config
{
    private const string txNameField = "tx_name";
    private const string procNameField = "proc_name";
    private const string keysField = "keys";
    private const string codeField = "code";
    private const string channelField = "channel";
    private const string fromField = "from";
    private const string toField = "to";

    public struct Key
    {
        // Code of the keypress to pass to the process
        private int _code;

        // Game controller channel id
        private int _channel;

        // Game controller channel range to send keypress on enter
        private int _from;
        private int _to;

        public int Code
        {
            get { return _code; }
        }
        public int Channel
        {
            get { return _channel; }
        }
        public int From
        {
            get { return _from; }
        }
        public int To
        {
            get { return _to; }
        }

        private static bool GetInt(out int value,  JSONNode node, string intName)
        {
            value = -1;
            bool isSuccess = false;
            do
            {
                JSONNode intNode = node[intName];
                if (intNode == null)
                {
                    Log.Error($"Cannot get int value fro field '{intName}'");
                    break;
                }

                value = intNode.AsInt;
                isSuccess = true;

            } while (false);

            return isSuccess;
        }

        public bool ReadFromJson(JSONNode node)
        {
            bool isSuccess = true;
            isSuccess &= GetInt(out _code, node, codeField);
            isSuccess &= GetInt(out _channel, node, channelField);
            isSuccess &= GetInt(out _from, node, fromField);
            isSuccess &= GetInt(out _to, node, toField);
            return isSuccess;
        }
    }

    // Name of the game controller device to read 
    public string TxName { get; private set; }

    // Name of the running process to send keypresses to 
    public string ProcName { get; private set; }

    // Keypresses mapping
    public Key[] Keys { get; set; }

    public bool ReadFromJson(string jsonPath)
    {
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

            TxName = root[txNameField];
            if (string.IsNullOrEmpty(TxName))
            {
                Log.Error("Cannot get device name from config.");
                break;
            }

            ProcName = root[procNameField];
            if (string.IsNullOrEmpty(ProcName))
            {
                Log.Error("Cannot get process name from config.");
                break;
            }

            JSONArray keyNodes = root[keysField] as JSONArray;
            if (keyNodes == null)
            {
                Log.Error("Cannot get keys array from config.");
                break;
            }

            bool keysReadOk = true;
            Keys = new Key[keyNodes.Count];
            for (int keyId = 0; keyId < keyNodes.Count; keyId++)
            {
                JSONNode keyNode = keyNodes[keyId];
                if (!Keys[keyId].ReadFromJson(keyNode))
                {
                    Log.Error($"Failed to read key#{keyId} from config.");
                    keysReadOk = false;
                }
            }

            if (!keysReadOk)
            {
                break;
            }

            isSuccess = true;

        } while (false);

        if (!isSuccess)
        {
            Log.Error("Failed to create from config file at: " + jsonPath);
        }
        return isSuccess;
    }
}
