using System;
using System.Globalization;
using System.Text;

namespace SimpleJSON
{
    public abstract class JSONData : JSONNode
    {
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

        public abstract object ToObject();
    }

    public abstract class JSONDataT<TValue> : JSONData
    {
        protected TValue Value;

        public override object ToObject()
        {
            return Value;
        }

        protected JSONDataT(TValue value)
        {
            Value = value;
        }

        public override string AsString
        {
            get { return Value.ToString(); }
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

        public override void ToString(StringBuilder builder)
        {
            builder.Append(Value);
        }

        public override void ToString(StringBuilder builder, string aPrefix)
        {
            builder.Append(aPrefix);
            builder.Append(Value);
        }

        public override void ToJSON(StringBuilder builder, int prefix)
        {
            builder.Append(prefix);
            builder.Append(Value);
        }
    }

    public class JSONDataInt : JSONDataT<int>
    {
        public override JSONBinaryTag Tag
        {
            get { return JSONBinaryTag.IntValue; }
        }

        public JSONDataInt(int value) : base(value)
        {
        }

        public override double AsDouble
        {
            get { return Value; }
            set { Value = (int)value; }
        }

        public override float AsFloat
        {
            get { return Value; }
            set { Value = (int)value; }
        }

        public override int AsInt
        {
            get { return Value; }
            set { Value = value; }
        }

        public override long AsLong
        {
            get { return Value; }
            set { Value = (int)value; }
        }
    }

    public class JSONDataLong : JSONDataT<long>
    {
        public override JSONBinaryTag Tag
        {
            get { return JSONBinaryTag.LongValue; }
        }

        public JSONDataLong(long value) : base(value)
        {
        }

        public override double AsDouble
        {
            get { return Value; }
            set { Value = (long)value; }
        }

        public override float AsFloat
        {
            get { return Value; }
            set { Value = (long)value; }
        }

        public override int AsInt
        {
            get { return (int)Value; }
            set { Value = value; }
        }

        public override long AsLong
        {
            get { return Value; }
            set { Value = value; }
        }
    }

    public class JSONDataBool : JSONDataT<bool>
    {
        public override JSONBinaryTag Tag
        {
            get { return JSONBinaryTag.BoolValue; }
        }

        public JSONDataBool(bool value) : base(value)
        {
        }

        public override string ToJSON(int prefix)
        {
            return Value.ToString();
        }

        public override bool AsBool
        {
            get { return Value; }
            set { Value = value; }
        }

        public override void ToString(StringBuilder builder)
        {
            Escape(builder, Value.ToString().ToLower());
        }

        public override void ToString(StringBuilder builder, string aPrefix)
        {
            builder.Append(aPrefix);
            Escape(builder, Value.ToString().ToLower());
        }

        public override void ToJSON(StringBuilder builder, int prefix)
        {
            builder.Append(prefix);
            builder.Append(Value.ToString().ToLower());
        }
    }

    public class JSONDataFloat : JSONDataT<float>
    {
        public override JSONBinaryTag Tag
        {
            get { return JSONBinaryTag.FloatValue; }
        }

        public JSONDataFloat(float value) : base(value)
        {
        }

        public override string ToJSON(int prefix)
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }

        public override double AsDouble
        {
            get { return Value; }
            set { Value = (float)value; }
        }

        public override float AsFloat
        {
            get { return Value; }
            set { Value = value; }
        }

        public override int AsInt
        {
            get { return (int)Value; }
            set { Value = value; }
        }

        public override long AsLong
        {
            get { return (long)Value; }
            set { Value = value; }
        }
    }

    public class JSONDataDouble : JSONDataT<double>
    {
        public override JSONBinaryTag Tag
        {
            get { return JSONBinaryTag.DoubleValue; }
        }

        public JSONDataDouble(double value) : base(value)
        {
        }

        public override string ToJSON(int prefix)
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }

        public override double AsDouble
        {
            get { return Value; }
            set { Value = value; }
        }

        public override float AsFloat
        {
            get { return (float)Value; }
            set { Value = value; }
        }

        public override int AsInt
        {
            get { return (int)Value; }
            set { Value = value; }
        }

        public override long AsLong
        {
            get { return (long)Value; }
            set { Value = value; }
        }
    }

    public class JSONDataString : JSONDataT<string>
    {
        public override JSONBinaryTag Tag
        {
            get { return JSONBinaryTag.StringValue; }
        }

        public JSONDataString(string value) : base(value ?? "")
        {
            if (value == null)
            {
                Log.Warning("JSONDataString ctor data is null");
            }
        }

        public override string ToJSON(int prefix)
        {
            return Value;
        }

        public override string AsString
        {
            get { return Value; }
            set { Value = value; }
        }

        public override int AsInt
        {
            get
            {
                int i;
                return int.TryParse(Value, out i) ? i : 0;
            }
            set { throw new Exception(); }
        }

        public override long AsLong
        {
            get
            {
                long i;
                return long.TryParse(Value, out i) ? i : 0;
            }
            set { throw new Exception(); }
        }

        public override float AsFloat
        {
            get
            {
                float f;
                return float.TryParse(Value, out f) ? f : 0f;
            }
            set { throw new Exception(); }
        }

        public override double AsDouble
        {
            get
            {
                double d;
                return double.TryParse(Value, out d) ? d : 0.0;
            }
            set { throw new Exception(); }
        }

        public override bool AsBool
        {
            get
            {
                bool flag;
                return bool.TryParse(Value, out flag) && flag;
            }
            set { throw new Exception(); }
        }

        public override void ToString(StringBuilder builder)
        {
            builder.Append('"');
            Escape(builder, Value);
            builder.Append('"');
        }

        public override void ToString(StringBuilder builder, string aPrefix)
        {
//            builder.Append(aPrefix);
            builder.Append('"');
            Escape(builder, Value);
            builder.Append('"');
        }

        public override void ToJSON(StringBuilder builder, int prefix)
        {
            builder.Append(prefix);
            builder.Append('\\');
            builder.Append(Value);
            builder.Append('\\');
        }
    }
}