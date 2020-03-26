using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ModLoaderLite.AssetBundles
{
    public static class BundleManager
    {
        public static List<AssetBundle> Load(string localpath, string modpath)
        {
            var bundles = Utilities.Util.GetModFiles(localpath, modpath, "*.bundle");
            var ret = new List<AssetBundle>();
            foreach(var bf in bundles)
            {
                try
                {
                    var loadedAB = AssetBundle.LoadFromFile(bf);
                    if (loadedAB != null)
                    {
                        KLog.Dbg($"AB loaded! {loadedAB.name}");
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
