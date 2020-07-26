using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Windows.Forms;

using Newtonsoft.Json;

namespace JsonDictionary
{
    internal static class JsonIo
    {
        public static bool SaveJson<T>(T data, string fileName)
        {
            var jsonSerializer = new DataContractJsonSerializer(typeof(T));
            var fileStream = File.Open(fileName, FileMode.Create);
            jsonSerializer.WriteObject(fileStream, data);
            fileStream.Close();
            fileStream.Dispose();

            return true;
        }

        public static List<T> LoadJson<T>(string fileName)
        {
            var jsonSerializer = new DataContractJsonSerializer(typeof(List<T>));
            var fileStream = File.Open(fileName, FileMode.Open);

            var newValues = (List<T>) jsonSerializer.ReadObject(fileStream);
            fileStream.Close();
            fileStream.Dispose();

            return newValues;
        }

        public static void SaveBinary<T>(T tree, string filename)
        {
            using (Stream file = File.Open(filename, FileMode.Create))
            {
                var bf = new BinaryFormatter();
                bf.Serialize(file, tree);
            }
        }

        public static T LoadBinary<T>(string filename)
        {
            T nodeList = default;
            using (Stream file = File.Open(filename, FileMode.Open))
            {
                try
                {
                    var bf = new BinaryFormatter();
                    var obj = bf.Deserialize(file);
                    nodeList = (T) obj;
                }
                catch (Exception ex)
                {
                    throw new Exception("File parse exception: " + ex.Message + Environment.NewLine +
                                        ex.InnerException.Message);
                }
            }

            return nodeList;
        }

        // possibly need rework
        public static string JsonShiftBrackets(string original)
        {
            var searchTokens = new[] {": {", ": ["};
            foreach (var token in searchTokens)
            {
                var i = original.IndexOf(token, StringComparison.Ordinal);
                while (i >= 0)
                {
                    int currentPos;
                    if (original[i + token.Length] != '\r' && original[i + token.Length] != '\n'
                    ) // not a single bracket
                    {
                        currentPos = i + 3;
                    }
                    else // need to shift bracket down the line
                    {
                        var j = i - 1;
                        var trail = 0;

                        if (j >= 0)
                            while (original[j] != '\n' && original[j] != '\r' && j >= 0)
                            {
                                if (original[j] == ' ') trail++;
                                else trail = 0;
                                j--;
                            }

                        if (j < 0) j = 0;

                        if (!(original[j] == '/' && original[j + 1] == '/')) // if it's a comment
                            original = original.Insert(i + 2, Environment.NewLine + new string(' ', trail));
                        currentPos = i + 3;
                    }

                    i = original.IndexOf(token, currentPos, StringComparison.Ordinal);
                }
            }

            return original;
        }

        // possibly need rework
        private static string JsonShiftBrackets_v2(string original)
        {
            var searchTokens = new[] {": {", ": ["};
            foreach (var token in searchTokens)
            {
                var i = original.IndexOf(token, StringComparison.Ordinal);
                while (i >= 0)
                {
                    int currentPos;
                    if (original[i + token.Length] != '\r' && original[i + token.Length] != '\n'
                    ) // not a single bracket
                    {
                        currentPos = i + 3;
                    }
                    else // need to shift bracket down the line
                    {
                        var j = i - 1;
                        var trail = 0;

                        if (j >= 0)
                            while (original[j] != '\n' && original[j] != '\r' && j >= 0)
                            {
                                if (original[j] == ' ') trail++;
                                else trail = 0;
                                j--;
                            }

                        if (j < 0) j = 0;

                        if (!(original[j] == '/' && original[j + 1] == '/')) // if it's a comment
                            original = original.Insert(i + 2, Environment.NewLine + new string(' ', trail));
                        currentPos = i + 3;
                    }

                    i = original.IndexOf(token, currentPos, StringComparison.Ordinal);
                }
            }

            var stringList = ConvertTextToStringList(original);

            const char prefixItem = ' ';
            const int prefixStep = 2;
            var openBrackets = new[] {'{', '['};
            var closeBrackets = new[] {'}', ']'};

            var prefixLength = 0;
            var prefix = "";
            var result = new StringBuilder();
            for (var i = 0; i < stringList.Length; i++)
            {
                stringList[i] = stringList[i].Trim();
                if (closeBrackets.Contains(stringList[i][0]))
                {
                    prefixLength -= prefixStep;
                    prefix = new string(prefixItem, prefixLength);
                }

                result.AppendLine(prefix + stringList[i]);

                if (openBrackets.Contains(stringList[i][0]))
                {
                    prefixLength += prefixStep;
                    prefix = new string(prefixItem, prefixLength);
                    if (stringList[i].Length > 1 && closeBrackets.Contains(stringList[i][stringList[i].Length - 1]))
                    {
                        prefixLength -= prefixStep;
                        prefix = new string(prefixItem, prefixLength);
                    }
                }
            }

            return result.ToString().Trim();
        }

        private static string[] ConvertTextToStringList(string data)
        {
            var lineDivider = new List<char> {'\x0d', '\x0a'};
            var stringCollection = new List<string>();
            var unparsedData = "";
            foreach (var t in data)
                if (lineDivider.Contains(t))
                {
                    if (unparsedData.Length > 0)
                    {
                        stringCollection.Add(unparsedData);
                        unparsedData = "";
                    }
                }
                else
                {
                    unparsedData += t;
                }

            if (unparsedData.Length > 0) stringCollection.Add(unparsedData);
            return stringCollection.ToArray();
        }

        public static string TrimJson(string original, bool trimEol)
        {
            original = original.Trim();
            if (trimEol)
            {
                original = original.Replace("\r\n", "\n");
                original = original.Replace('\r', '\n');
            }

            var i = original.IndexOf("\n ", StringComparison.Ordinal);
            while (i >= 0)
            {
                original = original.Replace("\n ", "\n");
                i = original.IndexOf("\n ", i, StringComparison.Ordinal);
            }

            if (trimEol) return original;

            i = original.IndexOf("\r ", StringComparison.Ordinal);
            while (i >= 0)
            {
                original = original.Replace("\r ", "\r");
                i = original.IndexOf("\r ", i, StringComparison.Ordinal);
            }

            return original;
        }

        public static string CompactJson(string json)
        {
            json = json.Trim();
            try
            {
                return ReformatJson(json, Formatting.None);
            }
            catch
            {
                return json;
            }
        }

        public static string BeautifyJson(string json, bool reformatJson)
        {
            json = json.Trim();
            try
            {
                json = ReformatJson(json, Formatting.Indented);
            }
            catch
            {
            }

            return reformatJson ? JsonShiftBrackets_v2(json) : json;
        }

        private static string ReformatJson(string json, Formatting formatting)
        {
            if (json[0] != '{' && json[0] != '[') return json;
            using (var stringReader = new StringReader(json))
            {
                using (var stringWriter = new StringWriter())
                {
                    ReformatJson(stringReader, stringWriter, formatting);
                    return stringWriter.ToString();
                }
            }
        }

        private static void ReformatJson(TextReader textReader, TextWriter textWriter, Formatting formatting)
        {
            using (var jsonReader = new JsonTextReader(textReader))
            {
                using (var jsonWriter = new JsonTextWriter(textWriter))
                {
                    jsonWriter.Formatting = formatting;
                    jsonWriter.WriteToken(jsonReader);
                }
            }
        }
    }
}
