using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace JsonDictionary
{
    [DataContract]
    [Serializable]
    public enum JsoncNodeType
    {
        [EnumMember] Unknown,
        [EnumMember] Property,
        [EnumMember] Object,
        [EnumMember] Array
    }

    [DataContract]
    [Serializable]
    public class MetaNode
    {
        [DataMember] public int Depth;
        [DataMember] public string ParentName;
        [DataMember] public string Name;
        [DataMember] public JsoncNodeType Type;
        [DataMember] public string Version;
        [DataMember] public Dictionary<string, string> ExamplesList;

        public MetaNode(string name, string parentName, JsoncNodeType type, int depth, string example, string fileName,
            string version)
        {
            Name = name;
            ParentName = parentName;
            Type = type;
            Depth = depth;
            Version = version;
            ExamplesList = new Dictionary<string, string> {{example, fileName}};
        }
    }

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
    public class JsoncDictionary
    {
        [DataMember] public JsoncContentType Type;
        [DataMember] public List<MetaNode> Nodes;

        public readonly bool CollectAllFileNames;

        public JsoncDictionary(JsoncContentType type, bool collectAllFileNames = false)
        {
            CollectAllFileNames = collectAllFileNames;
            Type = type;
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
            if (node == null) return "Can't fine node " + newNode.Name + " to add new data";

            var errorString = "";
            if (!node.Any())
            {
                Nodes?.Add(newNode);
            }
            else
            {
                if (node.Length > 1)
                    errorString = "More than 1 similar object found on add node " + node[0].Name + " to collection";

                var examples = node.FirstOrDefault()?.ExamplesList;

                if (examples == null)
                {
                    errorString = "Object with no examples found on add node " + node[0].Name + " to collection";
                    return errorString;
                }

                foreach (var newExample in newNode.ExamplesList)
                    if (!examples.ContainsKey(newExample.Key))
                        examples.Add(newExample.Key, newExample.Value);
                    else if (CollectAllFileNames) examples[newExample.Key] += Environment.NewLine + newExample.Value;
            }

            return errorString;
        }
    }
}
