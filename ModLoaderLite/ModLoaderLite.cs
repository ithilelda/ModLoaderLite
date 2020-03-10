using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using HarmonyLib;
using XiaWorld;
using FairyGUI;
using UnityEngine;

namespace ModLoaderLite
{
    public static class ModLoaderLite
    {
        static bool depLoaded;
        static bool inited;
        static List<Assembly> assemblies = new List<Assembly>();

        // the handle resolve event doesn't fire if an assembly of the same name is present...
        // so we have to manually load our own dependencies to prevent conflict.
        public static void LoadDep()
        {
            if (!depLoaded)
            {
                try
                {
                    KLog.Dbg("[ModLoaderLite] Loading dependencies...");
                    KLog.Dbg($"[ModLoaderLite] {typeof(ModLoaderLite).AssemblyQualifiedName}");
                    var currentDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    var harmonyLib = Path.Combine(currentDir, "0Harmony.dll");
                    Assembly.LoadFrom(harmonyLib);
                    KLog.Dbg("[ModLoaderLite] Harmony loaded.");
                    depLoaded = true;
                }
                catch (Exception ex)
                {
                    KLog.Dbg(ex.Message);
                    KLog.Dbg(ex.StackTrace);
                }
            }
        }
        // single time method that loads the assemblies and applies harmony patches.
        // also calls the oninit events that ought to be run only once each game.
        public static void Init()
        {
            if(!inited)
            {
                try
                {
                    KLog.Dbg("[ModLoaderLite] loading assemblies...");
                    var harmony = new Harmony("jnjly.ModLoaderLite");
                    PatchMoreEvents(harmony);
                    PatchSave(harmony);
                    var activated = ModsMgr.Instance.AllMods.Where(p => p.Value.IsActive == true && p.Value.Name != "ModLoaderLite"); // excluding ourself.
                    foreach (var p in activated)
                    {
                        var files = Directory.GetFiles(p.Value.Path, "*.dll", SearchOption.AllDirectories);
                        var harmony_name = $"{p.Value.Author}.{p.Value.Name}";
                        foreach (var file in files)
                        {
                            var rasm = Utilities.AssemblyLoader.PreLoadAssembly(file);
                            var asm = Utilities.AssemblyLoader.LoadAssembly(rasm);
                            if (asm != null) assemblies.Add(asm);
                            Utilities.AssemblyLoader.Call(asm, "OnInit");
                            Utilities.AssemblyLoader.ApplyHarmony(asm, harmony_name);
                        }
                    }
                    inited = true;
                }
                catch (Exception ex)
                {
                    KLog.Dbg(ex.Message);
                    KLog.Dbg(ex.StackTrace);
                }
            }
        }
        // will be called each time a game is loaded.
        public static void Load()
        {
            try
            {
                KLog.Dbg("[ModLoaderLite] game loading...");
                AddMenu();
                foreach (var asm in assemblies)
                {
                    Utilities.AssemblyLoader.Call(asm, "OnLoad");
                }
                Config.Configuration.Deserialize(null);
            }
            catch (Exception ex)
            {
                KLog.Dbg(ex.Message);
                KLog.Dbg(ex.StackTrace);
            }
        }


        private static void AddMenu()
        {
            // add a menu in the mainmenu.
            KLog.Dbg("[ModLoaderLite] adding config menu option...");
            var mainMenu = Traverse.Create(Wnd_GameMain.Instance).Field("MainMenu").GetValue<PopupMenu>();
            mainMenu.AddItem("MLL设置", () => Config.Configuration.Show());
        }
        private static void PatchSave(Harmony harmony)
        {
            KLog.Dbg("[ModLoaderLite] patching save hook...");
            var original = typeof(MainManager).GetMethod("DoSave", new Type[] { typeof(string) });
            var prefix = typeof(Config.SaveHook).GetMethod("DoSavePrefix");
            harmony.Patch(original, new HarmonyMethod(prefix));
        }
        private static void PatchMoreEvents(Harmony harmony)
        {
            KLog.Dbg("[ModLoaderLite] patching more events module...");
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
    }
}
