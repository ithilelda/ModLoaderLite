using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ModLoaderLite.AssetBundles
{
    public static class BundleManager
    {
        static Dictionary<string, AssetBundle> assetBundles { get; set; } = new Dictionary<string, AssetBundle>();

        public static AssetBundle GetAB(string key) => assetBundles.TryGetValue(key, out var ret) ? ret : default;
        public static GameObject GetPrefabFromAB(string abName, string prefabName)
        {
            if (assetBundles.TryGetValue(abName, out var ab))
            {
                return ab.LoadAsset<GameObject>(prefabName);
            }
            return null;
        }
        public static AssetBundle LoadAssetBundleFromMod(ModsMgr.ModData data, string fileName)
        {
            if (!assetBundles.TryGetValue(fileName, out var loadedAB))
            {
                try
                {
                    var bundleFile = Path.Combine(Path.Combine(data.Path, "Resources/AssetBundles"), fileName);
                    loadedAB = AssetBundle.LoadFromFile(bundleFile);
                    if (loadedAB != null)
                    {
                        KLog.Dbg($"[ModLoaderLite BundleManager] AB {fileName} loaded!");
                        assetBundles.Add(fileName, loadedAB);
                    }
                }
                catch (Exception e)
                {
                    KLog.Dbg(e.Message);
                    KLog.Dbg(e.StackTrace);
                }
            }
            return loadedAB;
        }
        public static void UnLoadAssetBundle(string key)
        {
            if (assetBundles.TryGetValue(key, out var ab))
            {
                ab.Unload(true);
                assetBundles.Remove(key);
            }
        }
    }
}
