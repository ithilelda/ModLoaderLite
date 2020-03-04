using System;
using System.Collections.Generic;


namespace ModLoaderLite.MoreEvents
{
    static class EventManager
    {
        public static event ReduceDamageHandler ReduceDamage;
        public static event ReduceLingDamageHandler ReduceLingDamage;

        public static float OnReduceDamage(object sender, ReduceDamageEventArgs arg)
        {
            var ret = ReduceDamage?.Invoke(sender, arg);
            return ret.GetValueOrDefault(arg.Damage);
        }

        public static float OnReduceLingDamage(object sender, ReduceLingDamageEventArgs arg)
        {
            var ret = ReduceLingDamage?.Invoke(sender, arg);
            return ret.GetValueOrDefault(arg.Damage);
        }
    }
}
