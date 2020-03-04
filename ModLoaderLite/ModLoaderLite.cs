using System;
using System.IO;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using XiaWorld;
using FairyGUI;
using UnityEngine;

namespace ModLoaderLite
{
    public static class ModLoaderLite
    {
        private static bool inited;

        static ModLoaderLite()
        {
            AppDomain.CurrentDomain.AssemblyResolve += HandleAssemblyResolve;
        }
        public static void Init()
        {
            if(!inited)
            {
                var harmony = new Harmony("jnjly.ModLoaderLite");
                PatchMoreEvents(harmony);
                //PatchSave(harmony);
                var activated = ModsMgr.Instance.AllMods.Where(p => p.Value.IsActive == true && p.Value.Name != "ModLoaderLite"); // excluding ourself.
                foreach(var p in activated)
                {
                    var files = Directory.GetFiles(p.Value.Path, "*.dll", SearchOption.AllDirectories);
                    var harmony_name = $"{p.Value.Author}.{p.Value.Name}";
                    foreach(var file in files)
                    {
                        var rasm = Utilities.AssemblyLoader.PreLoadAssembly(file);
                        var asm = Utilities.AssemblyLoader.LoadAssembly(rasm);
                        Utilities.AssemblyLoader.CallInit(asm);
                        Utilities.AssemblyLoader.ApplyHarmony(asm, harmony_name);
                    }
                }
                inited = true;
            }
        }
        public static void AddMenu()
        {
            // add a menu in the mainmenu.
            KLog.Dbg("ModLoaderLite adding config menu option...");
            try
            {
                var mainMenu = Traverse.Create(Wnd_GameMain.Instance).Field("MainMenu").GetValue<PopupMenu>();
                mainMenu.AddItem("MLL设置", () => Config.Configuration.Show());
            }
            catch (Exception ex)
            {
                KLog.Dbg(ex.Message);
                KLog.Dbg(ex.StackTrace);
            }
        }


        private static void PatchSave(Harmony harmony)
        {
            var original = typeof(MainManager).GetMethod("DoSave", new Type[] { typeof(string) });
            var prefix = typeof(Config.SaveHook).GetMethod("DoSavePrefix");
            harmony.Patch(original, new HarmonyMethod(prefix));
        }
        private static void PatchMoreEvents(Harmony harmony)
        {
            KLog.Dbg("ModLoaderLite patching more events module...");
            try
            {
                var rdoriginal = typeof(Npc).GetMethod("ReduceDamage", new Type[] { typeof(float), typeof(Npc), typeof(g_emElementKind), typeof(g_emDamageSource), typeof(Vector3?) });
                var rldoriginal = typeof(Npc).GetMethod("ReduceLingDamage");
                harmony.Patch(rdoriginal, new HarmonyMethod(typeof(MoreEvents.EventHooks), "ReduceDamagePrefix"));
                harmony.Patch(rldoriginal, new HarmonyMethod(typeof(MoreEvents.EventHooks), "ReduceLingDamagePrefix"));
            }
            catch (Exception ex)
            {
                KLog.Dbg(ex.Message);
                KLog.Dbg(ex.StackTrace);
            }
        }
        private static Assembly HandleAssemblyResolve(object sender, ResolveEventArgs arg)
        {
            var name = new AssemblyName(arg.Name).Name + ".dll";
            //KLog.Dbg($"resolving {arg.Name}");
            var location = Assembly.GetExecutingAssembly().Location;
            var thisDir = Path.GetDirectoryName(location);
            var askedFile = Path.Combine(thisDir, name);
            //KLog.Dbg($"the asked file is: {askedFile}");
            if (File.Exists(askedFile))
            {
                var asm = Assembly.LoadFrom(askedFile);
                return asm;
            }
            return null;
        }
    }
}
