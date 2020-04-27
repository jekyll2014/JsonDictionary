using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace JsonDictionary
{
    [DataContract, Serializable]
    public enum JsoncContentType
    {
        [EnumMember] DataViews,
        [EnumMember] Events,
        [EnumMember] Layout,
        [EnumMember] Rules,
        [EnumMember] Search,
        [EnumMember] Combo,
        [EnumMember] Tools,
        [EnumMember] PageTemplate,
        [EnumMember] Template,
        [EnumMember] Strings,
    }

    [DataContract, Serializable]
    public enum JsoncNodeType
    {
        [EnumMember] Unknown,
        [EnumMember] Property,
        [EnumMember] Object,
        [EnumMember] Array,
    }

    [DataContract, Serializable]
    public class MetaNode
    {
        [DataMember] public int Depth;
        [DataMember] public string ParentName;
        [DataMember] public string Name;
        [DataMember] public JsoncNodeType Type;
        [DataMember] public string Version;
        [DataMember] public Dictionary<string, string> ExamplesList;

        public MetaNode(string name, string parentName, JsoncNodeType type, int depth, string example, string fileName, string version)
        {
            this.Name = name;
            this.ParentName = parentName;
            this.Type = type;
            this.Depth = depth;
            this.Version = version;
            this.ExamplesList = new Dictionary<string, string>() { { example, fileName } };
        }
    }

    [DataContract, Serializable]
    public class JsoncDictionary
    {
        [DataMember]
        public static Dictionary<string, JsoncContentType> FileNames = new Dictionary<string, JsoncContentType>()
        {
            { "dataviews.jsonc", JsoncContentType.DataViews},
            { "events.jsonc", JsoncContentType.Events},
            { "layout.jsonc", JsoncContentType.Layout},
            { "rules.jsonc", JsoncContentType.Rules},
            { "search.jsonc", JsoncContentType.Search},
            { "combo.jsonc", JsoncContentType.Combo},
            { "tools.jsonc", JsoncContentType.Tools},
            { "pagetemplate.jsonc", JsoncContentType.PageTemplate},
            { "template.jsonc", JsoncContentType.Template},
            { "strings.jsonc", JsoncContentType.Strings},
        };

        [DataMember]
        public static Dictionary<string, JsoncNodeType> NodeTypes = new Dictionary<string, JsoncNodeType>()
        {
            { "JProperty", JsoncNodeType.Property},
            { "JObject", JsoncNodeType.Object},
            { "JArray", JsoncNodeType.Array},
        };

        [DataMember] public JsoncContentType Type;
        [DataMember] public List<MetaNode> Nodes;

        public readonly bool CollectAllFileNames = false;

        public JsoncDictionary(JsoncContentType type, bool collectAllFileNames = false)
        {
            this.CollectAllFileNames = collectAllFileNames;
            this.Type = type;
            Nodes = new List<MetaNode>();
        }

        public string Add(MetaNode newNode)
        {
            if (newNode == null) return "No data to add";

            var node = Nodes?.Where(n => n?.Depth == newNode.Depth
                                         && n.ParentName.Equals(newNode.ParentName, StringComparison.Ordinal)
                                         && n.Name.Equals(newNode.Name, StringComparison.Ordinal)
                                         && n.Type == newNode.Type
                                         && n.Version.Equals(newNode.Version, StringComparison.Ordinal)).ToArray();
            if (node == null)
            {
                return "Can't fine node " + newNode.Name + " to add new data";
            }

            var errorString = "";
            if (!node.Any())
            {
                Nodes?.Add(newNode);
            }
            else
            {
                if (node.Count() > 1)
                {
                    errorString = "More than 1 similar object found on add node " + node[0].Name + " to collection";
                }
                var examples = node.FirstOrDefault()?.ExamplesList;

                if (examples == null)
                {
                    errorString = "Object with no examples found on add node " + node[0].Name + " to collection";
                    examples = newNode.ExamplesList;
                    return errorString;
                }

                foreach (var newExample in newNode.ExamplesList)
                {
                    if (!examples.ContainsKey(newExample.Key)) examples.Add(newExample.Key, newExample.Value);
                    else if (CollectAllFileNames)
                    {
                        examples[newExample.Key] += "\r\n" + newExample.Value;
                    }
                }
            }

            return errorString;
        }
    }
}
