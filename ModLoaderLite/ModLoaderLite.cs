using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using Harmony;
using XiaWorld;

namespace ModLoaderLite
{
    public class ModLoaderLite
    {
        private static bool patched = false;
        private static Dictionary<string, Assembly> loadedAssemblies = new Dictionary<string, Assembly>();
        private static Dictionary<string, Assembly> patchedAssemblies = new Dictionary<string, Assembly>();

        static ModLoaderLite()
        {
            AppDomain.CurrentDomain.AssemblyResolve += HandleAssemblyResolve;
        }
        public ModLoaderLite()
        {
            if(!patched)
            {
                var harmony = HarmonyInstance.Create("ModLoaderLite");
                var original = typeof(MainManager).GetMethod("DoSave", new Type[] { typeof(string) });
                var prefix = typeof(Config.SaveHook).GetMethod("DoSavePrefix");
                harmony.Patch(original, new HarmonyMethod(prefix));
                patched = true;
            }
        }
        public Assembly Load(string file)
        {
            var rasm = Utilities.AssemblyLoader.PreLoadAssembly(file);
            if (rasm != null)
            {
                if(!loadedAssemblies.ContainsKey(rasm.FullName))
                {
                    var asm = Utilities.AssemblyLoader.LoadAssembly(rasm);
                    if(asm != null)
                    {
                        loadedAssemblies[asm.FullName] = asm;
                        return asm;
                    }
                }
                else
                {
                    KLog.Dbg($"{rasm.FullName} already loaded!");
                    return loadedAssemblies[rasm.FullName];
                }
            }
            return null;
        }
        public void ApplyHarmony(Assembly asm)
        {
            if(asm != null && !patchedAssemblies.ContainsKey(asm.FullName))
            {
                var suc = Utilities.AssemblyLoader.ApplyHarmony(asm);
                if(suc) patchedAssemblies[asm.FullName] = asm;
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
