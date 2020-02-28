using System;
using System.IO;
using System.Reflection;
using Harmony;


namespace ModLoaderLite.Utilities
{
    public static class AssemblyLoader
    {
        public static Assembly PreLoadAssembly(string file)
        {
            var fileName = Path.GetFileName(file);
            // we exclude errogenous libraries that may be a problem.
            if (!(fileName.ToLower() == "0harmony") && !(fileName.ToLower().Contains("modloaderlite")))
            {
                try
                {
                    KLog.Dbg($"Pre-Loading: {fileName}");
                    var assembly = Assembly.ReflectionOnlyLoadFrom(file);
                    return assembly;
                }
                catch (Exception ex)
                {
                    KLog.Dbg($"Pre-Loading assembly {fileName} failed!");
                    KLog.Dbg(ex.Message);
                }
            }
            return null;
        }
        public static Assembly LoadAssembly(Assembly asm)
        {
            if (asm != null)
            {
                try
                {
                    KLog.Dbg($"Loading: {asm.FullName}");
                    var loaded = Assembly.LoadFrom(asm.Location);
                    return loaded;
                }
                catch (Exception ex)
                {
                    KLog.Dbg($"loading assembly {asm.GetName()} failed!");
                    KLog.Dbg(ex.Message);
                }
            }
            return null;
        }
        public static bool ApplyHarmony(Assembly asm)
        {
            if(asm != null)
            {
                try
                {
                    KLog.Dbg($"Applying harmony patch: {asm.FullName}");
                    var harmonyInstance = HarmonyInstance.Create(asm.FullName);
                    harmonyInstance?.PatchAll(asm);
                    KLog.Dbg($"Applying patch {asm.FullName} succeeded!");
                    return true;
                }
                catch (Exception ex)
                {
                    KLog.Dbg($"Patching harmony mod {asm.FullName} failed!");
                    KLog.Dbg(ex.Message);
                }
            }
            return false;
        }
    }
}