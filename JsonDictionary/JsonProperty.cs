// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com

using System;
using System.Runtime.Serialization;
using System.Text;

namespace JsonDictionary
{
    [DataContract]
    [Serializable]
    public enum JsoncContentType
    {
        [EnumMember] Unknown,
        [EnumMember] DataViews,
        [EnumMember] Events,
        [EnumMember] Layout,
        [EnumMember] Rules,
        [EnumMember] Search,
        [EnumMember] Combo,
        [EnumMember] Tools,
        [EnumMember] Strings,
        [EnumMember] Patch
    }

    [DataContract]
    [Serializable]
    public enum JsonItemType
    {
        [EnumMember] Unknown,
        [EnumMember] Property,
        [EnumMember] ObjectStart,
        [EnumMember] ObjectEnd,
        [EnumMember] ArrayStart,
        [EnumMember] ArrayEnd
    }

    [DataContract]
    [Serializable]
    public class JsonProperty
    {
        [DataMember] public int LineId; // line # in complete project properties collection
        [DataMember] public string FullFileName; // original path + file name

        [DataMember] private string _jsonPath;
        public string JsonPath // JSON path of the property
        {
            set
            {
                _jsonPath = value;
                FlattenedJsonPath = value;
                _jsonDepth = -1;
            }
            get => _jsonPath;
        }

        private int _jsonDepth = -1; // depth in the original JSON structure
        public int JsonDepth
        {
            get
            {
                if (_jsonDepth == -1)
                {
                    _jsonDepth = GetPathDepth(_jsonPath);
                }

                return _jsonDepth;
            }
        }

        [DataMember] public string Name; // property name
        [DataMember] public string Value; // property value
        [DataMember] public JsoncContentType FileType; // file type (event, string, rules, ...)
        [DataMember] public string Version; // schema version declared in the beginning of the file
        [DataMember] public JsonItemType ItemType; // type of the property as per JSON classification (property, array, object)
        [DataMember] public string Parent; // parent name
        [DataMember] public bool Shared; // is original file in shared or project folder
        [DataMember] public int SourceLineNumber; // line number in the source file
        [DataMember] public int StartPosition; // property beginning byte # in the original file
        [DataMember] public int EndPosition; // property ending byte # in the original file

        [DataMember] public string FlattenedJsonPath; // JSON path of the property after events flattening

        private string _parentPath = null;
        public string ParentPath // parent object path
        {
            get
            {
                if (_parentPath == null)
                {
                    if (!string.IsNullOrEmpty(JsonPath) && JsonPath.Contains("."))
                        _parentPath = JsonPath.Substring(0, JsonPath.LastIndexOf('.'));
                    else
                        _parentPath = "";
                }

                return _parentPath;
            }
        }

        private string _unifiedPath = null;
        public string UnifiedPath // json path with no array [] brackets
        {
            get
            {
                if (_unifiedPath == null)
                {
                    _unifiedPath = UnifyPath(JsonPath);
                }

                return _unifiedPath;
            }
        }

        private string _unifiedFlattenedPath = null;
        public string UnifiedFlattenedPath // json flattened path with no array [] brackets
        {
            get
            {
                if (_unifiedFlattenedPath == null)
                {
                    _unifiedFlattenedPath = UnifyPath(FlattenedJsonPath);
                }

                return _unifiedFlattenedPath;
            }
        }

        public JsonProperty()
        {
            FullFileName = "";
            JsonPath = "";
            FlattenedJsonPath = "";
            Name = "";
            Value = "";
            FileType = JsoncContentType.Unknown;
            Version = "";
            ItemType = JsonItemType.Unknown;
            Parent = "";
            Shared = false;
            SourceLineNumber = -1;
            StartPosition = -1;
            EndPosition = -1;
        }

        public string UnifyPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return "";

            var unifiedPath = new StringBuilder();
            foreach (var token in path.Split('.'))
            {
                var pos = token.IndexOf('[');
                if (pos >= 0)
                {
                    unifiedPath.Append(token.Substring(0, pos) + ".");
                }
                else
                {
                    unifiedPath.Append(token + ".");
                }
            }
            return unifiedPath.ToString().TrimEnd('.');
        }

        public int GetPathDepth(string path)
        {
            if (string.IsNullOrEmpty(path))
                return 0;

            var depth = 0;
            foreach (var c in path)
            {
                if (c == '.')
                {
                    depth++;
                }
            }

            return depth;
        }
    }
}
