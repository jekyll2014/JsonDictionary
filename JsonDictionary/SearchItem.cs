// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com

using System;
using System.Collections;

namespace JsonDictionary
{
    public class SearchItem : ICloneable, IEqualityComparer, IEquatable<SearchItem>
    {
        public enum SearchCondition
        {
            Contains,
            StartsWith,
            EndsWith
        }

        public string Version;
        public SearchCondition Condition;
        public StringComparison CaseSensitive;
        public string Value;

        public SearchItem(string _defaultVersion)
        {
            Version = _defaultVersion;
            Condition = SearchCondition.Contains;
            CaseSensitive = StringComparison.OrdinalIgnoreCase;
            Value = "";
        }

        public object Clone()
        {
            return new SearchItem(Version)
            {
                Condition = Condition,
                CaseSensitive = CaseSensitive,
                Value = Value,
            };
        }

        bool IEqualityComparer.Equals(object x, object y)
        {
            if (x is SearchItem itemX && y is SearchItem itemY)
                return itemX.Version == itemY.Version
                       && itemX.Condition == itemY.Condition
                       && itemX.CaseSensitive == itemY.CaseSensitive
                       && itemX.Value == itemY.Value;

            return false;
        }

        public int GetHashCode(object obj)
        {
            if (obj is SearchItem item)
                return (item.Version + item.Condition + item.CaseSensitive + item.Value).GetHashCode();

            return obj.GetHashCode();
        }

        public bool Equals(SearchItem other)
        {
            return Version == other.Version
                   && Condition == other.Condition
                   && CaseSensitive == other.CaseSensitive
                   && Value == other.Value;
        }
    }
}
