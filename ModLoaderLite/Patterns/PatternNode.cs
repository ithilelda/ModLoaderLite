using System;
using System.Collections.Generic;

namespace ModLoaderLite.Patterns
{
    public class PatternNode : IEquatable<PatternNode>
    {
        public string Name { get; set; }
        public int Count { get; set; }
        public bool IsFsItem { get; set; }

        public bool Equals(PatternNode other)
        {
            if (other == null) return false;
            else
            {
                var nameMatch = string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(other.Name) || Name == other.Name;
                var countMatch = Count == 0 || other.Count == 0 || Count == other.Count;
                return nameMatch && countMatch && IsFsItem == other.IsFsItem;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is PatternNode r) return Equals(r);
            else return false;
        }

        public override int GetHashCode()
        {
            return (Name?.GetHashCode()).GetValueOrDefault() * Count + IsFsItem.GetHashCode();
        }
    }
}
