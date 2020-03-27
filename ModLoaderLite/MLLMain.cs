using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using HarmonyLib;
using XiaWorld;
using FairyGUI;
using UnityEngine;

namespace ModLoaderLite
{
    public static class MLLMain
    {
        static bool depLoaded;
        static bool inited;
        static List<Assembly> assemblies = new List<Assembly>();
        static Dictionary<string, object> saves = new Dictionary<string, object>();


        // the handle resolve event doesn't fire if an assembly of the same name is present...
        // so we have to manually load our own dependencies to prevent conflict.
        public static void LoadDep()
        {
            if (!depLoaded)
            {
                KLog.Dbg("[ModLoaderLite] Loading dependencies...");
                KLog.Dbg($"[ModLoaderLite] {typeof(MLLMain).AssemblyQualifiedName}");
                var currentDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var harmonyLib = Path.Combine(currentDir, "0Harmony.dll");
                //var protobuf = Path.Combine(currentDir, "protobuf-net.dll");
                Assembly.LoadFrom(harmonyLib);
                KLog.Dbg("[ModLoaderLite] Harmony loaded.");
                //Assembly.LoadFrom(protobuf);
                //KLog.Dbg("[ModLoaderLite] Protobuf loaded.");
                depLoaded = true;
            }
        }
        // single time method that loads the assemblies and applies harmony patches.
        // also calls the oninit events that ought to be run only once each game.
        public static void Init()
        {
            if(!inited)
            {
                KLog.Dbg("[ModLoaderLite] loading assemblies...");
                var harmony = new Harmony("jnjly.ModLoaderLite");
                PatchMoreEvents(harmony);
                PatchSave(harmony);
                var activated = ModsMgr.Instance.AllMods.Where(p => p.Value.IsActive == true && p.Value.Name != "ModLoaderLite"); // excluding ourself.
                foreach (var p in activated)
                {
                    try
                    {
                        var files = Directory.GetFiles(p.Value.Path, $"{p.Value.Name}.dll", SearchOption.AllDirectories);
                        var harmony_name = $"{p.Value.Author}.{p.Value.Name}";
                        foreach (var file in files)
                        {
                            var rasm = Utilities.Util.PreLoadAssembly(file);
                            var asm = Utilities.Util.LoadAssembly(rasm);
                            if (asm != null) assemblies.Add(asm);
                            Utilities.Util.Call(asm, "OnInit");
                            Utilities.Util.ApplyHarmony(asm, harmony_name);
                        }
                        AssetBundles.BundleManager.LoadAssetBundleFromMod(p.Value, p.Value.Name.ToLower());
                    }
                    catch (Exception ex)
                    {
                        KLog.Dbg($"the mod {p.Value.DisplayName} cannot be loaded!");
                        KLog.Dbg($"the error is: {ex.Message}");
                    }
                }
                inited = true;
            }
        }
        // will be called each time a game is loaded.
        public static void Load()
        {
            KLog.Dbg("[ModLoaderLite] game loading...");
            AddMenu();
            DoLoad();
            foreach (var asm in assemblies)
            {
                Utilities.Util.Call(asm, "OnLoad");
            }
            Config.Configuration.Load();
        }

        // a simple method to get mod specific saved data.
        public static T GetSaveOrNull<T>(string id) => saves.TryGetValue(id, out var ret) ? (T)ret : default;
        // same for adding mod specific data. Later data will overwrite former, and a bool is returned false if adding, true if overwriting.
        public static bool AddOrOverWriteSave(string id, object item)
        {
            if (item == null) throw new ArgumentNullException("item");
            var contained = saves.ContainsKey(id);
            saves[id] = item;
            return contained;
        }


        static void AddMenu()
        {
            // add a menu in the mainmenu.
            KLog.Dbg("[ModLoaderLite] adding config menu option...");
            var mainMenu = Traverse.Create(Wnd_GameMain.Instance).Field("MainMenu").GetValue<PopupMenu>();
            mainMenu?.AddItem("MLL设置", () => Config.Configuration.Show());
        }
        static void PatchSave(Harmony harmony)
        {
            KLog.Dbg("[ModLoaderLite] patching save hook...");
            try
            {
                var original = typeof(MainManager).GetMethod("DoSave", new Type[] { typeof(string) });
                var prefix = typeof(MLLMain).GetMethod("DoSavePrefix", BindingFlags.NonPublic | BindingFlags.Static);
                harmony.Patch(original, new HarmonyMethod(prefix));
            }
            catch (Exception ex)
            {
                KLog.Dbg(ex.Message);
                KLog.Dbg(ex.StackTrace);
            }
        }
        static void PatchMoreEvents(Harmony harmony)
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
        static void DoSavePrefix(string name)
        {
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), $"saves/{name}.mll");
            using (var fs = new FileStream(fullPath, FileMode.Create))
            {
                try
                {
                    Config.Configuration.Save();
                    foreach (var asm in assemblies)
                    {
                        Utilities.Util.Call(asm, "OnSave");
                    }
                    var serializer = new BinaryFormatter();
                    serializer.Serialize(fs, saves);
                }
                catch (Exception e)
                {
                    KLog.Dbg("Failed to deserialize. Reason: " + e.Message);
                    KLog.Dbg(e.StackTrace);
                }
            }
        }
        static void DoLoad()
        {
            var fileName = GameWatch.Instance.LoadFile;
            var fullName = Path.Combine(Directory.GetCurrentDirectory(), $"saves/{fileName}.mll");
            if (File.Exists(fullName))
            {
                using (var fs = new FileStream(fullName, FileMode.Open))
                {
                    try
                    {
                        var serializer = new BinaryFormatter();
                        saves = (Dictionary<string, object>)serializer.Deserialize(fs);
                    }
                    catch(Exception e)
                    {
                        KLog.Dbg("Failed to serialize. Reason: " + e.Message);
                        KLog.Dbg(e.StackTrace);
                    }
                }
            }
        }
    }
}
