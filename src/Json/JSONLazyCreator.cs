using System.Text;

namespace SimpleJSON
{
    internal class JSONLazyCreator : JSONNode
    {
        public override JSONBinaryTag Tag
        {
            get { return JSONBinaryTag.Class; }
        }

        private JSONNode m_Node;
        private readonly string m_Key;

        public JSONLazyCreator(JSONNode aNode)
        {
            m_Node = aNode;
            m_Key = null;
        }

        public JSONLazyCreator(JSONNode aNode, string aKey)
        {
            m_Node = aNode;
            m_Key = aKey;
        }

        private void Set(JSONNode aVal)
        {
            if (m_Key == null)
            {
                m_Node.Add(aVal);
            }
            else
            {
                m_Node.Add(m_Key, aVal);
            }

            m_Node = null; // Be GC friendly.
        }

        public override JSONNode this[int aIndex]
        {
            get { return new JSONLazyCreator(this); }
            set
            {
                JSONArray tmp = new JSONArray();
                tmp.Add(value);
                Set(tmp);
            }
        }

        public override JSONNode this[string aKey]
        {
            get { return new JSONLazyCreator(this, aKey); }
            set
            {
                JSONClass tmp = new JSONClass { { aKey, value } };
                Set(tmp);
            }
        }

        public override void Add(JSONNode aItem)
        {
            JSONArray tmp = new JSONArray();
            tmp.Add(aItem);
            Set(tmp);
        }

        public override void Add(string aKey, JSONNode aItem)
        {
            JSONClass tmp = new JSONClass { { aKey, aItem } };
            Set(tmp);
        }

        public static bool operator ==(JSONLazyCreator a, object b)
        {
            return b == null || ReferenceEquals(a, b);
        }

        public static bool operator !=(JSONLazyCreator a, object b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            return obj == null || ReferenceEquals(this, obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return string.Empty;
        }

        public override string ToString(string aPrefix)
        {
            return string.Empty;
        }

        public override string ToJSON(int prefix)
        {
            return string.Empty;
        }

        public override void ToString(StringBuilder builder)
        {
        }

        public override void ToString(StringBuilder builder, string aPrefix)
        {
        }

        public override void ToJSON(StringBuilder builder, int prefix)
        {
        }

        public override int AsInt
        {
            get
            {
                JSONData tmp = new JSONDataInt(0);
                Set(tmp);
                return 0;
            }
            set
            {
                JSONData tmp = new JSONDataInt(value);
                Set(tmp);
            }
        }

        public override long AsLong
        {
            get
            {
                JSONData tmp = new JSONDataLong(0);
                Set(tmp);
                return 0;
            }
            set
            {
                JSONData tmp = new JSONDataLong(value);
                Set(tmp);
            }
        }

        public override float AsFloat
        {
            get
            {
                JSONData tmp = new JSONDataFloat(0.0f);
                Set(tmp);
                return 0.0f;
            }
            set
            {
                JSONData tmp = new JSONDataFloat(value);
                Set(tmp);
            }
        }

        public override double AsDouble
        {
            get
            {
                JSONData tmp = new JSONDataDouble(0.0);
                Set(tmp);
                return 0.0;
            }
            set
            {
                JSONData tmp = new JSONDataDouble(value);
                Set(tmp);
            }
        }

        public override bool AsBool
        {
            get
            {
                JSONData tmp = new JSONDataBool(false);
                Set(tmp);
                return false;
            }
            set
            {
                JSONData tmp = new JSONDataBool(value);
                Set(tmp);
            }
        }

        public override JSONArray AsArray
        {
            get
            {
                JSONArray tmp = new JSONArray();
                Set(tmp);
                return tmp;
            }
        }

        public override JSONClass AsObject
        {
            get
            {
                JSONClass tmp = new JSONClass();
                Set(tmp);
                return tmp;
            }
        }

        public override string AsString
        {
            get
            {
                JSONData tmp = new JSONDataString(string.Empty);
                Set(tmp);
                return string.Empty;
            }
            set
            {
                JSONData tmp = new JSONDataString(value);
                Set(tmp);
            }
        }
    }
}