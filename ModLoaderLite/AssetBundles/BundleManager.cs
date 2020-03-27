using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ModLoaderLite.AssetBundles
{
    public static class BundleManager
    {
        static Dictionary<string, AssetBundle> AssetBundles { get; set; } = new Dictionary<string, AssetBundle>();

        public static void LoadAllBundlesFromMod(ModsMgr.ModData data)
        {
            var bundles = Load("Resources/AssetBundles", data.Path, "*");
            foreach (var b in bundles)
            {
                AssetBundles[b.name] = b;
            }
        }
        public static AssetBundle GetAB(string key) => AssetBundles.TryGetValue(key, out var ret) ? ret : default;

        static List<AssetBundle> Load(string localpath, string modpath, string pattern)
        {
            var bundles = Utilities.Util.GetModFiles(localpath, modpath, pattern);
            var ret = new List<AssetBundle>();
            foreach(var bf in bundles)
            {
                try
                {
                    var loadedAB = AssetBundle.LoadFromFile(bf);
                    if (loadedAB != null)
                    {
                        KLog.Dbg($"[ModLoaderLite BundleManager] AB {loadedAB.name} loaded!");
                        ret.Add(loadedAB);
                    }
                }
                catch (Exception e)
                {
                    KLog.Dbg(e.Message);
                    KLog.Dbg(e.StackTrace);
                }
            }
            return ret;
        }
    }
}
