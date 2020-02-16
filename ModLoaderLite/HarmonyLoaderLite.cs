using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Harmony;


namespace ModLoaderLite
{
    public static class HarmonyLoaderLite
    {
        public static void Enter(string path, string assemblyName, string typeFullName)
        {
            var files = Directory.GetFiles(path, "*.dll", SearchOption.AllDirectories);
            var asms = AssemblyLoaderLite.LoadAssemblies(AssemblyLoaderLite.PreLoadAssemblies(files));
            Apply(asms, assemblyName, typeFullName);
        }
        private static void Apply(IEnumerable<Assembly> asms, string assemblyName, string typeFullName)
        {
            KLog.Dbg("Applying Harmony patches");
            var failed = new List<string>();
            foreach (var assembly in asms)
            {
                try
                {
                    KLog.Dbg($"Applying harmony patch: {assembly.FullName}");
                    var harmonyInstance = HarmonyInstance.Create(assembly.FullName);
                    harmonyInstance?.PatchAll(assembly);
                    KLog.Dbg($"Applying patch {assembly.FullName} succeeded!");
                    if(assembly.GetName().Name == assemblyName)
                    {
                        var init = assembly.GetType(typeFullName)?.GetMethod("Init");
                        if(init != null)
                        {
                            KLog.Dbg($"Found Init method for type {typeFullName} of {assemblyName}, Invoking...");
                            init.Invoke(null, null);
                        }
                    }
                }
                catch (Exception ex)
                {
                    failed.Add(assembly.FullName);
                    KLog.Dbg($"Patching harmony mod {assembly.FullName} failed!");
                    KLog.Dbg(ex.Message);
                }
            }
            if (failed.Count > 0)
            {
                var text = "\nThe following mods could not be patched:\n" + string.Join("\n\t", failed.ToArray());
                KLog.Dbg(text);
            }
        }
    }
}