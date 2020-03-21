using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ModLoaderLite.JsonExs
{
    public static class JsonEx
    {
        static JsonSerializerSettings settings = new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore };
        public static List<T> LoadJson<T>(string localpath, string modpath = null, string pattern = "*.json")
        {
            var ret = new List<T>();
            foreach (var file in Utilities.Util.GetModFiles(localpath, modpath, pattern))
            {
                try
                {
                    var str = File.ReadAllText(file);
                    var items = JsonConvert.DeserializeObject<List<T>>(str, settings);
                    ret.AddRange(items);
                }
                catch (Exception ex)
                {
                    KLog.Dbg(ex.Message);
                    KLog.Dbg(ex.StackTrace);
                }
            }
            return ret;
        }
    }
}
