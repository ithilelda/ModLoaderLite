using System;
using System.Collections.Generic;
using System.Linq;
using XiaWorld;

namespace ModLoaderLite.Patterns
{
    public static class PatternConvert
    {
        public static Arrangement ToArrangement(IEnumerable<ItemThing> items)
        {
            var ret = new Arrangement();
            ret.Reactants = items.Select(i => i == null ? new PatternNode { Name = "NOTHING", Count = -1} : new PatternNode { Name = i.def.Name, Count = i.Count, IsFsItem = i.FSItemState > 0}).ToList();
            return ret;
        }
        public static Combination ToCombination(IEnumerable<ItemThing> items)
        {
            var ret = new Combination();
            var reactants = items.Where(i => i != null).GroupBy(i => new { i.def.Name, State = i.FSItemState > 0 }).Select(g => new PatternNode { Name = g.Key.Name, Count = g.Count(), IsFsItem = g.Key.State });
            ret.Reactants = new HashSet<PatternNode>(reactants);
            return ret;
        }
    }
}
