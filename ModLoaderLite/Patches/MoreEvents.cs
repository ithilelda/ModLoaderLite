using System;
using XiaWorld;
using HarmonyLib;
using UnityEngine;
using XiaWorld.Fight;
using ModLoaderLite.MoreEvents;

namespace ModLoaderLite.Patches
{
    [HarmonyPatch(typeof(Npc))]
    static class MoreEvents
    {
        [HarmonyPrefix]
        [HarmonyPatch("ReduceDamage", new Type[] { typeof(float), typeof(Npc), typeof(g_emElementKind), typeof(g_emDamageSource), typeof(Vector3?), typeof(float), typeof(string), typeof(FabaoBase) })]
        static bool ReduceDamagePrefix(Npc __instance, ref float v, Npc from, ref g_emElementKind element, ref g_emDamageSource source, ref Vector3? hitpos, ref float Penetration, ref string desc, FabaoBase fabao)
        {
            var arg = new ReduceDamageEventArgs(from, __instance, v, element, source, hitpos, Penetration, desc, fabao);
            v = EventManager.OnReduceDamage(__instance, arg);
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch("ReduceLingDamage")]
        static bool ReduceLingDamagePrefix(Npc __instance, ref float v, ref Npc from, ref g_emElementKind element, ref bool bodydamage, string desc)
        {
            var arg = new ReduceLingDamageEventArgs(from, __instance, v, element, bodydamage, desc);
            v = EventManager.OnReduceLingDamage(__instance, arg);
            return true;
        }
    }
}
