using XiaWorld;
using UnityEngine;

namespace ModLoaderLite.MoreEvents
{
    static class EventHooks
    {
        static bool ReduceDamagePrefix (Npc __instance, ref float v, ref Npc from, ref g_emElementKind element, ref g_emDamageSource source, ref Vector3? hitpos)
        {
            var arg = new ReduceDamageEventArgs(from, __instance, v, element, source, hitpos);
            v = EventManager.OnReduceDamage(__instance, arg);
            return true;
        }
        static bool ReduceLingDamagePrefix(Npc __instance, ref float v, ref Npc from, ref g_emElementKind element, ref bool bodydamage, string desc)
        {
            var arg = new ReduceLingDamageEventArgs(from, __instance, v, element, bodydamage, desc);
            v = EventManager.OnReduceLingDamage(__instance, arg);
            return true;
        }
    }
}
