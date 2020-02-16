using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

namespace ModLoaderLite
{
    public static class Extensions
    {
        public static bool TryGetValueOrDefault(this Dictionary<string, bool> d, string key) => d.TryGetValue(key, out var ret) ? ret : default;
    }
    public class ModLoaderLite
    {
        private static Dictionary<string, bool> patched = new Dictionary<string, bool>();

        static ModLoaderLite()
        {
            AppDomain.CurrentDomain.AssemblyResolve += HandleAssemblyResolve;
        }
        public void Load(string path, string assemblyName="", string typeFullName="")
        {
            if(!patched.TryGetValueOrDefault(path))
            {
                HarmonyLoaderLite.Enter(path, assemblyName, typeFullName);
                patched[path] = true;
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
