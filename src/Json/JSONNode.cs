using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SimpleJSON
{
    public abstract class JSONNode
    {
        #region common interface

        public virtual void Add(string aKey, JSONNode aItem)
        {
        }

        public virtual JSONNode this[int aIndex]
        {
            get { return null; }
            set { }
        }

        public virtual JSONNode this[string aKey]
        {
            get { return null; }
            set { }
        }

        public virtual int Count
        {
            get { return 0; }
        }

        public virtual void Add(JSONNode aItem)
        {
            Add(string.Empty, aItem);
        }

        public virtual JSONNode Remove(string aKey)
        {
            return null;
        }

        public virtual JSONNode Remove(int aIndex)
        {
            return null;
        }

        public virtual JSONNode Remove(JSONNode aNode)
        {
            return aNode;
        }

        // ReSharper disable once MemberCanBeProtected.Global
        public virtual IEnumerable<JSONNode> Children
        {
            get { yield break; }
        }

        public IEnumerable<JSONNode> DeepChildren
        {
            get
            {
                foreach (JSONNode C in Children)
                foreach (JSONNode D in C.DeepChildren)
                    yield return D;
            }
        }

        public override string ToString()
        {
            return "JSONNode";
        }


        public virtual string ToString(string aPrefix)
        {
            return "JSONNode";
        }

        public virtual void ToString(StringBuilder builder)
        {
            builder.Append("JSONNode");
        }

        public virtual void ToString(StringBuilder builder, string aPrefix)
        {
            builder.Append("JSONNode");
        }

        public abstract string ToJSON(int prefix);
        public abstract void ToJSON(StringBuilder builder, int prefix);

        #endregion common interface

        #region typecasting properties

        public abstract JSONBinaryTag Tag { get; }
//        public virtual string Value { get; set; }

        // ReSharper disable once MemberCanBeProtected.Global
        public abstract string AsString { get; set; }
        public abstract int AsInt { get; set; }
        public abstract long AsLong { get; set; }
        public abstract float AsFloat { get; set; }
        public abstract double AsDouble { get; set; }
        public abstract bool AsBool { get; set; }

        public virtual JSONArray AsArray
        {
            get { return this as JSONArray; }
        }

        public virtual JSONClass AsObject
        {
            get { return this as JSONClass; }
        }

        public virtual JSONData AsData
        {
            get { return this as JSONData; }
        }

        #endregion typecasting properties

        #region operators

        public static implicit operator JSONNode(string s)
        {
            return new JSONDataString(s);
        }

        public static implicit operator string(JSONNode d)
        {
            return (d == null) ? null : d.AsString;
        }

        public static bool operator ==(JSONNode a, object b)
        {
            if (b == null && a is JSONLazyCreator)
                return true;
            return ReferenceEquals(a, b);
        }

        public static bool operator !=(JSONNode a, object b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj);
        }

        public override int GetHashCode()
        {
            // ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
            return base.GetHashCode();
        }

        #endregion operators

        internal static void Escape(StringBuilder builder, string aText)
        {
            foreach (char c in aText)
            {
                switch (c)
                {
                    case '\\':
                        builder.Append("\\\\");
                        break;
                    case '\"':
                        builder.Append("\\\"");
                        break;
                    case '\n':
                        builder.Append("\\n");
                        break;
                    case '\r':
                        builder.Append("\\r");
                        break;
                    case '\t':
                        builder.Append("\\t");
                        break;
                    case '\b':
                        builder.Append("\\b");
                        break;
                    case '\f':
                        builder.Append("\\f");
                        break;
                    default:
                        builder.Append(c);
                        break;
                }
            }
        }

        private static JSONData Numberize(string token)
        {
            bool flag;
            int integer;
            long longInt;
            double real;

            if (int.TryParse(token, out integer))
            {
                return new JSONDataInt(integer);
            }

            if (long.TryParse(token, out longInt))
            {
                return new JSONDataLong(longInt);
            }

            if (double.TryParse(token, out real))
            {
                return new JSONDataDouble(real);
            }

            if (bool.TryParse(token, out flag))
            {
                return new JSONDataBool(flag);
            }

            throw new NotImplementedException(token);
        }

        private static void AddElement(JSONNode ctx, string token, string tokenName, bool tokenIsString)
        {
            JSONData data;
            if (tokenIsString)
            {
                data = new JSONDataString(token);
            }
            else if (token == "null")
            {
                data = new JSONDataString("");
            }
            else
            {
                data = Numberize(token);
            }

            if (ctx is JSONArray)
            {
                ctx.Add(data);
            }
            else
            {
                ctx.Add(tokenName, data);
            }
        }

        public static JSONNode Parse(string aJSON)
        {
            Stack<JSONNode> stack = new Stack<JSONNode>();
            JSONNode ctx = null;
            int i = 0;
            StringBuilder Token = new StringBuilder();
            string TokenName = string.Empty;
            bool QuoteMode = false;
            bool TokenIsString = false;
            while (i < aJSON.Length)
            {
                switch (aJSON[i])
                {
                    case '{':
                        if (QuoteMode)
                        {
                            Token.Append(aJSON[i]);
                            break;
                        }

                        stack.Push(new JSONClass());
                        if (ctx != null)
                        {
                            TokenName = TokenName.Trim();
                            if (ctx is JSONArray)
                                ctx.Add(stack.Peek());
                            else if (TokenName != string.Empty)
                                ctx.Add(TokenName, stack.Peek());
                        }

                        TokenName = string.Empty;
                        Token.Remove(0, Token.Length);
                        ctx = stack.Peek();
                        break;

                    case '[':
                        if (QuoteMode)
                        {
                            Token.Append(aJSON[i]);
                            break;
                        }

                        stack.Push(new JSONArray());
                        if (ctx != null)
                        {
                            TokenName = TokenName.Trim();

                            if (ctx is JSONArray)
                                ctx.Add(stack.Peek());
                            else if (TokenName != string.Empty)
                                ctx.Add(TokenName, stack.Peek());
                        }

                        TokenName = string.Empty;
                        Token.Remove(0, Token.Length);
                        ctx = stack.Peek();
                        break;

                    case '}':
                    case ']':
                        if (QuoteMode)
                        {
                            Token.Append(aJSON[i]);
                            break;
                        }

                        if (stack.Count == 0)
                            throw new Exception("JSON Parse: Too many closing brackets");

                        stack.Pop();
                        if (Token.Length != 0 || TokenIsString)
                        {
                            TokenName = TokenName.Trim();
                            AddElement(ctx, Token.ToString(), TokenName, TokenIsString);
                            TokenIsString = false;
                        }

                        TokenName = string.Empty;
                        Token.Remove(0, Token.Length);

                        if (stack.Count > 0)
                            ctx = stack.Peek();
                        break;

                    case ':':
                        if (QuoteMode)
                        {
                            Token.Append(aJSON[i]);
                            break;
                        }

                        TokenName = Token.ToString();
                        Token.Remove(0, Token.Length);
                        TokenIsString = false;
                        break;

                    case '"':
                        QuoteMode ^= true;
                        TokenIsString = QuoteMode == true ? true : TokenIsString;
                        break;

                    case ',':
                        if (QuoteMode)
                        {
                            Token.Append(aJSON[i]);
                            break;
                        }

                        if (Token.Length != 0 || TokenIsString)
                        {
                            AddElement(ctx, Token.ToString(), TokenName, TokenIsString);
                            TokenIsString = false;
                        }

                        TokenName = string.Empty;
                        Token.Remove(0, Token.Length);
                        TokenIsString = false;
                        break;

                    case '\r':
                    case '\n':
                        break;

                    case ' ':
                    case '\t':
                        if (QuoteMode)
                            Token.Append(aJSON[i]);
                        break;

                    case '\\':
                        ++i;
                        if (QuoteMode)
                        {
                            char C = aJSON[i];
                            switch (C)
                            {
                                case 't':
                                    Token.Append('\t');
                                    break;
                                case 'r':
                                    Token.Append('\r');
                                    break;
                                case 'n':
                                    Token.Append('\n');
                                    break;
                                case 'b':
                                    Token.Append('\b');
                                    break;
                                case 'f':
                                    Token.Append('\f');
                                    break;
                                case 'u':
                                {
                                    string s = aJSON.Substring(i + 1, 4);
                                    Token.Append((char)int.Parse(
                                        s,
                                        System.Globalization.NumberStyles.AllowHexSpecifier));
                                    i += 4;
                                    break;
                                }
                                default:
                                    Token.Append(C);
                                    break;
                            }
                        }

                        break;

                    default:
                        Token.Append(aJSON[i]);
                        break;
                }

                ++i;
            }

            if (QuoteMode)
            {
                throw new Exception("JSON Parse: Quotation marks seems to be messed up.");
            }

            return ctx;
        }

        public virtual void Serialize(System.IO.BinaryWriter aWriter)
        {
        }

        public void SaveToStream(System.IO.Stream aData)
        {
            BinaryWriter W = new System.IO.BinaryWriter(aData);
            Serialize(W);
        }

#if USE_SharpZipLib
		public void SaveToCompressedStream(System.IO.Stream aData)
		{
			using (var gzipOut = new ICSharpCode.SharpZipLib.BZip2.BZip2OutputStream(aData))
			{
				gzipOut.IsStreamOwner = false;
				SaveToStream(gzipOut);
				gzipOut.Close();
			}
		}
		
		public void SaveToCompressedFile(string aFileName)
		{
			
			#if USE_FileIO
			System.IO.Directory.CreateDirectory((new System.IO.FileInfo(aFileName)).Directory.FullName);
			using(var F = System.IO.File.OpenWrite(aFileName))
			{
				SaveToCompressedStream(F);
			}
			
			#else
			throw new Exception("Can't use File IO stuff in webplayer");
			#endif
		}
		public string SaveToCompressedBase64()
		{
			using (var stream = new System.IO.MemoryStream())
			{
				SaveToCompressedStream(stream);
				stream.Position = 0;
				return System.Convert.ToBase64String(stream.ToArray());
			}
		}

#else
        public void SaveToCompressedStream(System.IO.Stream aData)
        {
            throw new Exception(
                "Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
        }

        public void SaveToCompressedFile(string aFileName)
        {
            throw new Exception(
                "Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
        }

        public string SaveToCompressedBase64()
        {
            throw new Exception(
                "Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
        }
#endif

        public void SaveToFile(string aFileName)
        {
            //	#if USE_FileIO
            System.IO.Directory.CreateDirectory((new System.IO.FileInfo(aFileName)).Directory.FullName);
            using (FileStream f = System.IO.File.OpenWrite(aFileName))
            {
                SaveToStream(f);
            }
//			#else
//			throw new Exception ("Can't use File IO stuff in webplayer");
//			#endif
        }

        public string SaveToBase64()
        {
            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                SaveToStream(stream);
                stream.Position = 0;
                return System.Convert.ToBase64String(stream.ToArray());
            }
        }

        public static JSONNode Deserialize(System.IO.BinaryReader aReader)
        {
            JSONBinaryTag type = (JSONBinaryTag)aReader.ReadByte();
            switch (type)
            {
                case JSONBinaryTag.Array:
                {
                    int count = aReader.ReadInt32();
                    JSONArray tmp = new JSONArray();
                    for (int i = 0; i < count; i++)
                        tmp.Add(Deserialize(aReader));
                    return tmp;
                }
                case JSONBinaryTag.Class:
                {
                    int count = aReader.ReadInt32();
                    JSONClass tmp = new JSONClass();
                    for (int i = 0; i < count; i++)
                    {
                        string key = aReader.ReadString();
                        JSONNode val = Deserialize(aReader);
                        tmp.Add(key, val);
                    }

                    return tmp;
                }
                case JSONBinaryTag.StringValue:
                {
                    return new JSONDataString(aReader.ReadString());
                }
                case JSONBinaryTag.IntValue:
                {
                    return new JSONDataInt(aReader.ReadInt32());
                }
                case JSONBinaryTag.LongValue:
                {
                    return new JSONDataLong(aReader.ReadInt64());
                }
                case JSONBinaryTag.DoubleValue:
                {
                    return new JSONDataDouble(aReader.ReadDouble());
                }
                case JSONBinaryTag.BoolValue:
                {
                    return new JSONDataBool(aReader.ReadBoolean());
                }
                case JSONBinaryTag.FloatValue:
                {
                    return new JSONDataFloat(aReader.ReadSingle());
                }

                default:
                {
                    throw new Exception("Error deserializing JSON. Unknown tag: " + type);
                }
            }
        }

#if USE_SharpZipLib
		public static JSONNode LoadFromCompressedStream(System.IO.Stream aData)
		{
			var zin = new ICSharpCode.SharpZipLib.BZip2.BZip2InputStream(aData);
			return LoadFromStream(zin);
		}
		public static JSONNode LoadFromCompressedFile(string aFileName)
		{
			#if USE_FileIO
			using(var F = System.IO.File.OpenRead(aFileName))
			{
				return LoadFromCompressedStream(F);
			}
			#else
			throw new Exception("Can't use File IO stuff in webplayer");
			#endif
		}
		public static JSONNode LoadFromCompressedBase64(string aBase64)
		{
			var tmp = System.Convert.FromBase64String(aBase64);
			var stream = new System.IO.MemoryStream(tmp);
			stream.Position = 0;
			return LoadFromCompressedStream(stream);
		}
#else
        public static JSONNode LoadFromCompressedFile(string aFileName)
        {
            throw new Exception(
                "Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
        }

        public static JSONNode LoadFromCompressedStream(System.IO.Stream aData)
        {
            throw new Exception(
                "Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
        }

        public static JSONNode LoadFromCompressedBase64(string aBase64)
        {
            throw new Exception(
                "Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
        }
#endif

        public static JSONNode LoadFromStream(System.IO.Stream aData)
        {
            using (BinaryReader r = new System.IO.BinaryReader(aData))
            {
                return Deserialize(r);
            }
        }

        public static JSONNode LoadFromFile(string aFileName)
        {
#if USE_FileIO
			using (var F = System.IO.File.OpenRead (aFileName)) {
				return LoadFromStream (F);
			}
#else
            throw new Exception("Can't use File IO stuff in webplayer");
#endif
        }

        public static JSONNode LoadFromBase64(string aBase64)
        {
            byte[] tmp = System.Convert.FromBase64String(aBase64);
            MemoryStream stream = new System.IO.MemoryStream(tmp);
            stream.Position = 0;
            return LoadFromStream(stream);
        }
    }
}