using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using XiaWorld;

namespace ModLoaderLite.Patches
{
    [HarmonyPatch(typeof(BuildingThing), "OnInit")]
    static class BuildingHelperExtension
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codes)
        {
            var counter = 0;
            foreach (var code in codes)
            {
                if (code.Calls(typeof(string).GetMethod("Concat", new Type[] { typeof(string), typeof(string) })))
                {
                    counter++;
                    if (counter == 2)
                    {
                        yield return new CodeInstruction(OpCodes.Pop);
                        yield return new CodeInstruction(OpCodes.Pop);
                        yield return new CodeInstruction(OpCodes.Ldarg_1);
                        yield return new CodeInstruction(OpCodes.Call, typeof(BuildingHelperExtension).GetMethod("GetHelperTypeName", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static));
                        counter = 0;
                        continue;
                    }
                }
                yield return code;
            }
        }

        static string GetHelperTypeName(ThingDef def)
        {
            var helper = def.Building.HelperClass;
            if (helper.Contains(",")) // with comma, it is an assembly qualified name, we simply return it.
            {
                return helper;
            }
            else // without comma, it is a normal class of the base game.
            {
                return "XiaWorld." + helper;
            }
        }
    }
}
