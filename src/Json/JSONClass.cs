using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleJSON
{
    public class JSONClass : JSONNode, IEnumerable
    {
        private readonly Dictionary<string, JSONNode> m_Dict = new Dictionary<string, JSONNode>();

        public override string AsString
        {
            get { return ToString(); }
            set { throw new Exception(); }
        }

        public override int AsInt
        {
            get { throw new Exception(); }
            set { throw new Exception(); }
        }

        public override long AsLong
        {
            get { throw new Exception(); }
            set { throw new Exception(); }
        }

        public override float AsFloat
        {
            get { throw new Exception(); }
            set { throw new Exception(); }
        }

        public override double AsDouble
        {
            get { throw new Exception(); }
            set { throw new Exception(); }
        }

        public override bool AsBool
        {
            get { throw new Exception(); }
            set { throw new Exception(); }
        }

        public override JSONBinaryTag Tag
        {
            get { return JSONBinaryTag.Class; }
        }

        public Dictionary<string, JSONNode> Dict
        {
            get { return m_Dict; }
        }

        public override JSONNode this[string aKey]
        {
            get { return m_Dict.ContainsKey(aKey) ? m_Dict[aKey] : new JSONLazyCreator(this, aKey); }
            set
            {
                if (m_Dict.ContainsKey(aKey))
                    m_Dict[aKey] = value;
                else
                    m_Dict.Add(aKey, value);
            }
        }

        public override JSONNode this[int aIndex]
        {
            get
            {
                if (aIndex < 0 || aIndex >= m_Dict.Count)
                    return null;
                return m_Dict.ElementAt(aIndex).Value;
            }
            set
            {
                if (aIndex < 0 || aIndex >= m_Dict.Count)
                    return;
                string key = m_Dict.ElementAt(aIndex).Key;
                m_Dict[key] = value;
            }
        }

        public override int Count
        {
            get { return m_Dict.Count; }
        }


        public override void Add(string aKey, JSONNode aItem)
        {
            if (!string.IsNullOrEmpty(aKey))
            {
                if (m_Dict.ContainsKey(aKey))
                    m_Dict[aKey] = aItem;
                else
                    m_Dict.Add(aKey, aItem);
            }
            else
                m_Dict.Add(Guid.NewGuid().ToString(), aItem);
        }

        public override JSONNode Remove(string aKey)
        {
            if (!m_Dict.ContainsKey(aKey))
                return null;
            JSONNode tmp = m_Dict[aKey];
            m_Dict.Remove(aKey);
            return tmp;
        }

        public override JSONNode Remove(int aIndex)
        {
            if (aIndex < 0 || aIndex >= m_Dict.Count)
                return null;
            KeyValuePair<string, JSONNode> item = m_Dict.ElementAt(aIndex);
            m_Dict.Remove(item.Key);
            return item.Value;
        }

        public override JSONNode Remove(JSONNode aNode)
        {
            try
            {
                KeyValuePair<string, JSONNode> item = m_Dict.First(k => k.Value == aNode);
                m_Dict.Remove(item.Key);
                return aNode;
            }
            catch
            {
                return null;
            }
        }

        public override IEnumerable<JSONNode> Children
        {
            get
            {
                foreach (KeyValuePair<string, JSONNode> N in m_Dict)
                    yield return N.Value;
            }
        }

        public IEnumerator GetEnumerator()
        {
            foreach (KeyValuePair<string, JSONNode> N in m_Dict)
                yield return N;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            ToString(builder);
            return builder.ToString();
        }

        public override string ToString(string aPrefix)
        {
            StringBuilder builder = new StringBuilder();
            ToString(builder, aPrefix);
            return builder.ToString();
        }

        public override string ToJSON(int prefix)
        {
            StringBuilder builder = new StringBuilder();
            ToJSON(builder, prefix);
            return builder.ToString();
        }

        public override void ToString(StringBuilder builder)
        {
            builder.Append("{");
            int counter = 0;
            foreach (KeyValuePair<string, JSONNode> N in m_Dict)
            {
                if (counter >= 1)
                    builder.Append(",");

                builder.Append("\"");
                Escape(builder, N.Key);
                builder.Append("\":");
                N.Value.ToString(builder);
                counter++;
            }

            builder.Append("}");
        }

        public override void ToString(StringBuilder builder, string aPrefix)
        {
            builder.Append("{");
            int counter = 0;
            foreach (KeyValuePair<string, JSONNode> N in m_Dict)
            {
                if (counter >= 1)
                    builder.Append(",");

                builder.AppendFormat("\n{0}", aPrefix);
                builder.Append("\"");
                Escape(builder, N.Key);
                builder.Append("\":");
                N.Value.ToString(builder, aPrefix + aPrefix);
                counter++;
            }

            builder.Append("\n");
            builder.Append(aPrefix);
            builder.Append("}");
        }

        public override void ToJSON(StringBuilder builder, int prefix)
        {
            builder.Append("{");
            int counter = 0;
            foreach (KeyValuePair<string, JSONNode> n in m_Dict)
            {
                if (counter >= 1)
                    builder.Append(",");
                builder.AppendFormat("\"{0}\":", n.Key);
                n.Value.ToJSON(builder, prefix + 1);
                counter++;
            }

            builder.Append("\n}");
        }

        public override void Serialize(System.IO.BinaryWriter aWriter)
        {
            aWriter.Write((byte)JSONBinaryTag.Class);
            aWriter.Write(m_Dict.Count);
            foreach (string K in m_Dict.Keys)
            {
                aWriter.Write(K);
                m_Dict[K].Serialize(aWriter);
            }
        }
    }
}