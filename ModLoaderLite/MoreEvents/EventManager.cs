using System;
using XLua;
using System.Collections.Generic;


namespace ModLoaderLite.MoreEvents
{
    public static class EventManager
    {
        public static event ReduceDamageHandler ReduceDamage;
        public static event ReduceLingDamageHandler ReduceLingDamage;

        internal static float OnReduceDamage(object sender, ReduceDamageEventArgs arg)
        {
            var ret = ReduceDamage?.Invoke(sender, arg);
            return ret.GetValueOrDefault(arg.Damage);
        }

        internal static float OnReduceLingDamage(object sender, ReduceLingDamageEventArgs arg)
        {
            var ret = ReduceLingDamage?.Invoke(sender, arg);
            return ret.GetValueOrDefault(arg.Damage);
        }
    }
}
