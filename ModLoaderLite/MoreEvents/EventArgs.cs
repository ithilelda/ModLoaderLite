using XiaWorld;
using System;
using UnityEngine;

namespace ModLoaderLite.MoreEvents
{
    public delegate float ReduceDamageHandler(object sender, ReduceDamageEventArgs arg);
    public delegate float ReduceLingDamageHandler(object sender, ReduceLingDamageEventArgs arg);
    public class ReduceDamageEventArgs : EventArgs
    {
        public readonly Npc From;
        public readonly Npc Target;
        public readonly float Damage;
        public readonly g_emElementKind Element;
        public readonly g_emDamageSource Source;
        public readonly Vector3? Hitpos;

        public ReduceDamageEventArgs(Npc from, Npc target, float damage, g_emElementKind element, g_emDamageSource source, Vector3? hitpos)
        {
            From = from;
            Target = target;
            Damage = damage;
            Element = element;
            Source = source;
            Hitpos = hitpos;
        }
    }

    public class ReduceLingDamageEventArgs : EventArgs
    {
        public readonly Npc From;
        public readonly Npc Target;
        public readonly float Damage;
        public readonly g_emElementKind Element;
        public readonly bool Bodydamage;
        public readonly string Desc;

        public ReduceLingDamageEventArgs(Npc from, Npc target, float v, g_emElementKind element, bool bodydamage, string desc)
        {
            From = from;
            Target = target;
            Damage = v;
            Element = element;
            Bodydamage = bodydamage;
            Desc = desc;
        }
    }
}
