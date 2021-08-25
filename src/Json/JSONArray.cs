using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SimpleJSON
{
    public class JSONArray : JSONNode, IEnumerable
    {
        private readonly List<JSONNode> m_List = new List<JSONNode>();

        public override JSONBinaryTag Tag
        {
            get { return JSONBinaryTag.Array; }
        }

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

        public override JSONNode this[int aIndex]
        {
            get
            {
                if (aIndex < 0 || aIndex >= m_List.Count)
                    return new JSONLazyCreator(this);
                return m_List[aIndex];
            }
            set
            {
                if (aIndex < 0 || aIndex >= m_List.Count)
                    m_List.Add(value);
                else
                    m_List[aIndex] = value;
            }
        }

        public override JSONNode this[string aKey]
        {
            get { return new JSONLazyCreator(this); }
            set { m_List.Add(value); }
        }

        public override int Count
        {
            get { return m_List.Count; }
        }

        public override void Add(string aKey, JSONNode aItem)
        {
            m_List.Add(aItem);
        }

        public override JSONNode Remove(int aIndex)
        {
            if (aIndex < 0 || aIndex >= m_List.Count)
                return null;
            JSONNode tmp = m_List[aIndex];
            m_List.RemoveAt(aIndex);
            return tmp;
        }

        public override JSONNode Remove(JSONNode aNode)
        {
            m_List.Remove(aNode);
            return aNode;
        }

        public override IEnumerable<JSONNode> Children
        {
            get
            {
                foreach (JSONNode N in m_List)
                    yield return N;
            }
        }

        public IEnumerator GetEnumerator()
        {
            foreach (JSONNode N in m_List)
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
            builder.Append("[");
            int counter = 0;
            foreach (JSONNode N in m_List)
            {
                if (counter >= 1)
                    builder.Append(",");
                N.ToString(builder);
                counter++;
            }

            builder.Append("]");
        }

        public override void ToString(StringBuilder builder, string aPrefix)
        {
            builder.Append("[");
            int counter = 0;
            foreach (JSONNode N in m_List)
            {
                if (counter >= 1)
                    builder.Append(",");
                builder.AppendFormat("\n{0}", aPrefix);
                N.ToString(builder, aPrefix);
                counter++;
            }

            builder.AppendFormat("\n{0}]", aPrefix);
        }

        public override void ToJSON(StringBuilder builder, int prefix)
        {
            builder.Append("[");
            int counter = 0;
            foreach (JSONNode n in m_List)
            {
                if (counter >= 1)
                    builder.Append(",");
                builder.Append("\n");
                n.ToJSON(builder, prefix + 1);
                counter++;
            }

            builder.Append("\n]");
        }

        public override void Serialize(System.IO.BinaryWriter aWriter)
        {
            aWriter.Write((byte)JSONBinaryTag.Array);
            aWriter.Write(m_List.Count);
            foreach (JSONNode n in m_List)
            {
                n.Serialize(aWriter);
            }
        }
    }
}