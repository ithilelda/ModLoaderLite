using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModLoaderLite.MoreEvents
{
    public static class MoreEvents
    {
        public static void SubscribeReduceDamage(ReduceDamageHandler h) => EventManager.ReduceDamage += h;
        public static void UnsubscribeReduceDamage(ReduceDamageHandler h) => EventManager.ReduceDamage -= h;

        public static void SubscribeReduceLingDamage(ReduceLingDamageHandler h) => EventManager.ReduceLingDamage += h;
        public static void UnsubscribeReduceLingDamage(ReduceLingDamageHandler h) => EventManager.ReduceLingDamage -= h;
    }
}
