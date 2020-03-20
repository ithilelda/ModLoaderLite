using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace ModLoaderLite.JsonExs
{
    public static class JsonEx
    {
        static JsonSerializerSettings settings = new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore };
        public static List<T> LoadJson<T>(string localpath, string modpath = null, string pattern = "*.json")
        {
            var ret = new List<T>();
            if (string.IsNullOrEmpty(modpath))
            {
                var paths = ModsMgr.Instance.GetPath(localpath).Where(pd => pd.mod != null).Select(pd => pd.path); // we ignore vanilla files.
                foreach (var path in paths)
                {
                    var files = Directory.GetFiles(path, pattern, SearchOption.AllDirectories);
                    foreach (var file in files)
                    {
                        var str = File.ReadAllText(file);
                        var items = JsonConvert.DeserializeObject<List<T>>(str, settings);
                        ret.AddRange(items);
                    }
                }
            }
            else
            {
                var path = Path.Combine(modpath, localpath);
                var files = Directory.GetFiles(path, pattern, SearchOption.AllDirectories);
                foreach (var file in files)
                {
                    var str = File.ReadAllText(file);
                    var items = JsonConvert.DeserializeObject<List<T>>(str);
                    ret.AddRange(items);
                }
            }
            return ret;
        }
    }
}
