﻿// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com

using System.Linq;
using System.Runtime.Serialization;
using System.Text;

using Newtonsoft.Json.Linq;

namespace JsonDictionary
{
    [DataContract]
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
    public enum JsonVariableType
    {
        [EnumMember] Unknown,
        [EnumMember] Object,
        [EnumMember] Array,
        [EnumMember] String,
        [EnumMember] Number,
        [EnumMember] Boolean,
        [EnumMember] Null,
        [EnumMember] Comment,
    }

    [DataContract]
    public class JsonProperty
    {
        [DataMember] public char PathDelimiter = '.';
        [DataMember] public int LineId; // line # in complete project properties collection
        [DataMember] public string FullFileName; // original path + file name
        [DataMember] public string Name; // property name
        [DataMember] public string Value; // property value
        [DataMember] public JTokenType VariableType; // type of the variable
        [DataMember] public JsoncContentType ContentType; // file type (event, string, rules, ...)
        [DataMember] public string Version; // schema version declared in the beginning of the file
        [DataMember] public JsonItemType ItemType; // type of the property as per JSON classification (property, array, object)
        [DataMember] public string Parent; // parent name
        [DataMember] public bool Shared; // is original file in shared or project folder
        [DataMember] public int SourceLineNumber; // line number in the source file
        [DataMember] public int StartPosition; // property beginning byte # in the original file
        [DataMember] public int EndPosition; // property ending byte # in the original file
        [DataMember] public string FlattenedJsonPath; // JSON path of the property after events flattening

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

        [DataMember] private string _unifiedFlattenedJsonPath = null;
        public string UnifiedFlattenedJsonPath => _unifiedFlattenedJsonPath ??= UnifyPath(FlattenedJsonPath); // json flattened path with no array [] brackets

        [DataMember] private int _jsonDepth = -1; // depth in the original JSON structure
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

        [DataMember] private string _parentPath = null;
        public string ParentPath // parent object path
        {
            get
            {
                if (_parentPath == null)
                {
                    if (!string.IsNullOrEmpty(JsonPath) && JsonPath.Contains(PathDelimiter))
                        _parentPath = JsonPath.Substring(0, JsonPath.LastIndexOf(PathDelimiter));
                    else
                        _parentPath = "";
                }

                return _parentPath;
            }
        }

        [DataMember] private string _unifiedPath = null;
        public string UnifiedPath => _unifiedPath ??= UnifyPath(JsonPath); // json path with no array [] brackets

        public JsonProperty()
        {
            FullFileName = "";
            JsonPath = "";
            Name = "";
            Value = "";
            VariableType = JTokenType.Undefined;
            ContentType = JsoncContentType.Unknown;
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
            foreach (var token in path.Split(PathDelimiter))
            {
                var pos = token.IndexOf('[');
                if (pos >= 0)
                {
                    unifiedPath.Append(token.Substring(0, pos) + PathDelimiter);
                }
                else
                {
                    unifiedPath.Append(token + PathDelimiter);
                }
            }
            return unifiedPath.ToString().TrimEnd(PathDelimiter);
        }

        // incorrect
        public int GetPathDepth(string path)
        {
            return string.IsNullOrEmpty(path) ? 0 : path.Count(c => c == PathDelimiter);
        }
    }
}
