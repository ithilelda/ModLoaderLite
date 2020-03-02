using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Harmony;
using XiaWorld;
using FairyGUI;

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
                //PatchSelf();
                var activated = ModsMgr.Instance.AllMods.Where(p => p.Value.IsActive == true);
                foreach(var p in activated)
                {
                    var files = Directory.GetFiles(p.Value.Path, "*.dll", SearchOption.AllDirectories);
                    foreach(var file in files)
                    {
                        var rasm = Utilities.AssemblyLoader.PreLoadAssembly(file);
                        var asm = Utilities.AssemblyLoader.LoadAssembly(rasm);
                        Utilities.AssemblyLoader.CallInit(asm);
                        Utilities.AssemblyLoader.ApplyHarmony(asm);
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
            }
        }


        private static void PatchSelf()
        {
            var harmony = HarmonyInstance.Create("ModLoaderLite");
            var original = typeof(MainManager).GetMethod("DoSave", new Type[] { typeof(string) });
            var prefix = typeof(Config.SaveHook).GetMethod("DoSavePrefix");
            harmony.Patch(original, new HarmonyMethod(prefix));
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
