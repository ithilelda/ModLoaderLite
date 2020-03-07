using System;
using System.IO;
using System.Reflection;
using HarmonyLib;


namespace ModLoaderLite.Utilities
{
    static class AssemblyLoader
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
                    KLog.Dbg(ex.StackTrace);
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
                    KLog.Dbg(ex.StackTrace);
                }
            }
            return null;
        }
        public static bool ApplyHarmony(Assembly asm, string name)
        {
            if(asm != null)
            {
                try
                {
                    var harmony_name = string.IsNullOrEmpty(name) ? asm.FullName : name;
                    KLog.Dbg($"Applying harmony patch: {asm.FullName}");
                    var harmonyInstance = new Harmony(harmony_name);
                    harmonyInstance?.PatchAll(asm);
                    KLog.Dbg($"Applying patch {asm.GetName().Name} succeeded!");
                    return true;
                }
                catch (Exception ex)
                {
                    KLog.Dbg($"Patching harmony mod {asm.GetName().Name} failed!");
                    KLog.Dbg(ex.Message);
                    KLog.Dbg(ex.StackTrace);
                }
            }
            return false;
        }
        public static void Call(Assembly asm, string method)
        {
            if (asm != null)
            {
                try
                {
                    var name = asm.GetName().Name;
                    asm.GetType($"{name}.{name}")?.GetMethod(method)?.Invoke(null, null);
                }
                catch(Exception ex)
                {
                    KLog.Dbg(ex.Message);
                    KLog.Dbg(ex.StackTrace);
                }
            }
        }
    }
}