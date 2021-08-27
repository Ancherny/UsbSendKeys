using System;
using System.IO;
using SimpleJSON;

public struct Config
{
    private const string txNameField = "tx_name";
    private const string procNameField = "proc_name";
    private const string keysField = "keys";
    private const string nameField = "name";
    private const string keyToPressField = "keyToPress";
    private const string channelField = "channel";
    private const string fromField = "from";
    private const string toField = "to";

    public struct Key
    {
        // Name of the operation keypress performs
        private string _name;

        // Code of the keypress to pass to the process
        private ConsoleKey _keyToPress;

        // Game controller channel id
        private int _channelId;

        // Game controller channel range to send keypress on enter
        private int _from;
        private int _to;

        public string Name
        {
            get { return _name; }
        }
        public ConsoleKey KeyToPress
        {
            get { return _keyToPress; }
        }
        public int ChannelId
        {
            get { return _channelId; }
        }

        public bool IsActive(int channelValue)
        {
            return channelValue >= _from && channelValue <= _to;
        }

        private delegate bool GetValue<T>(out T value, JSONNode node);

        private static bool GetField<T>(
            out T value,
            JSONNode node,
            string fieldName,
            T defaultValue,
            GetValue<T> getValue,
            bool acceptDefaultValue = false)
        {
            value = defaultValue;
            bool isSuccess = false;
            do
            {
                JSONNode valueNode = node[fieldName];
                if (valueNode == null)
                {
                    break;
                }
                isSuccess = getValue(out value, valueNode);

            } while (false);

            isSuccess |= acceptDefaultValue;

            if (!isSuccess)
            {
                Log.Error($"Cannot get {typeof(T)} value from field '{fieldName}'");
            }
            return isSuccess;
        }

        private static bool GetInt(out int value,  JSONNode node, string fieldName)
        {
            return GetField(
                out value,
                node, fieldName,
                -1,
                (out int iv, JSONNode jn) => int.TryParse(jn, out iv));
        }

        private static bool GetString(out string value,  JSONNode node, string fieldName)
        {
            return GetField(
                out value,
                node,
                fieldName,
                null,
                (out string sv, JSONNode jn) =>
                {
                    sv = jn;
                    return !string.IsNullOrEmpty(sv);
                });
        }

        // ReSharper disable once UnusedMember.Local
        private static bool GetBool(out bool value,  JSONNode node, string fieldName)
        {
            return GetField(
                out value,
                node,
                fieldName,
                false,
                (out bool bv, JSONNode jn) => bool.TryParse(jn, out bv),
                true);
        }

        private static bool GetConsoleKey(out ConsoleKey value,  JSONNode node, string fieldName)
        {
            return GetField(
                out value,
                node,
                fieldName,
                ConsoleKey.Escape,
                (out ConsoleKey kv, JSONNode jn) => Enum.TryParse(jn, out kv));
        }

        public bool ReadFromJson(JSONNode node)
        {
            bool isSuccess = true;
            isSuccess &= GetString(out _name, node, nameField);
            isSuccess &= GetConsoleKey(out _keyToPress, node, keyToPressField);

            int channel;
            isSuccess &= GetInt(out channel, node, channelField);
            if (channel < 1 || channel > 8)
            {
                Log.Error($"Bad 'channel' value: #{channel}  Channel value should be in range (1:8)");
                isSuccess = false;
            }

            // ChannelId is zero-based array index
            _channelId = channel - 1;

            isSuccess &= GetInt(out _from, node, fromField);
            if (_from < 1000 || _from > 2000)
            {
                Log.Error($"Bad 'from' value: #{_from}  From value should be in range (1000:2000) as RC TX microseconds.");
                isSuccess = false;
            }

            isSuccess &= GetInt(out _to, node, toField);
            if (_to < 1000 || _to > 2000)
            {
                Log.Error($"Bad 'to' value: #{_to}  From value should be in range (1000:2000) as RC TX microseconds.");
                isSuccess = false;
            }

            if (_from >= _to)
            {
                Log.Error($"'from' value should be less than 'to' value.");
                isSuccess = false;
            }

            return isSuccess;
        }
    }

    // Name of the game controller device to read
    public string TxName { get; private set; }

    // Name of the running process to send keypresses to
    public string ProcName { get; private set; }

    // Keypresses mapping
    public Key[] Keys { get; private set; }

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
