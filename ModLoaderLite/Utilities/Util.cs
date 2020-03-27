using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using HarmonyLib;

namespace ModLoaderLite.Utilities
{
    static class Util
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
                var harmony_name = string.IsNullOrEmpty(name) ? asm.FullName : name;
                try
                {
                    KLog.Dbg($"Applying harmony patch: {harmony_name}");
                    var harmonyInstance = new Harmony(harmony_name);
                    harmonyInstance?.PatchAll(asm);
                    KLog.Dbg($"Applying patch {harmony_name} succeeded!");
                    return true;
                }
                catch (Exception ex)
                {
                    KLog.Dbg($"Patching harmony mod {harmony_name} failed!");
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
                    KLog.Dbg($"[ModLoaderLite] calling the {method} method for {name}...");
                    asm.GetType($"{name}.{name}")?.GetMethod(method)?.Invoke(null, null);
                }
                catch (ArgumentException ae)
                {
                    KLog.Dbg(ae.Message);
                }
                catch (TargetInvocationException tie)
                {
                    KLog.Dbg($"invocation of {method} in {asm.FullName} failed!");
                    var ie = tie.InnerException;
                    KLog.Dbg(ie.Message);
                    KLog.Dbg(ie.StackTrace);
                }
                catch (Exception e)
                {
                    KLog.Dbg(e.Message);
                    KLog.Dbg(e.StackTrace);
                }
            }
        }

        public static List<string> GetModFiles(string localpath, string modpath, string pattern)
        {
            var files = new List<string>();
            if (string.IsNullOrEmpty(modpath))
            {
                var paths = ModsMgr.Instance.GetPath(localpath).Where(pd => pd.mod != null).Select(pd => pd.path); // we ignore vanilla files.
                foreach (var path in paths)
                {
                    try
                    {
                        files.AddRange(Directory.GetFiles(path, pattern, SearchOption.AllDirectories));
                    }
                    catch (Exception ex)
                    {
                        KLog.Dbg($"Unable to get files in path {path}, ignoring the mod!");
                        KLog.Dbg($"the error is: {ex.Message}");
                    }
                }
            }
            else
            {
                try
                {
                    var path = Path.Combine(modpath, localpath);
                    files.AddRange(Directory.GetFiles(path, pattern, SearchOption.AllDirectories));
                }
                catch (Exception ex)
                {
                    KLog.Dbg($"Unable to get files in {localpath} of {modpath}, check your directory name parameters!");
                    KLog.Dbg($"the error is: {ex.Message}");
                }
            }
            return files;
        }
    }
}