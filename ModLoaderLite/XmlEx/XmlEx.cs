using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace ModLoaderLite.XmlEx
{
    public static class XmlEx
    {
        static Dictionary<Type, XmlSerializer> serializers = new Dictionary<Type, XmlSerializer>();
        public static List<T> LoadXmlEx<T>(string localpath, string root, string item, string modpath = null, string pattern = "*.xml")
        {
            var type = typeof(T);
            var defs_type = typeof(DefsEx<T>);
            var ret = new List<T>();
            if(!serializers.TryGetValue(type, out var serializer))
            {
                var type_attrs = new XmlAttributes();
                var root_attr = new XmlRootAttribute(root);
                type_attrs.XmlRoot = root_attr;
                var member_attrs = new XmlAttributes();
                var array_attr = new XmlArrayAttribute("List");
                var item_attr = new XmlArrayItemAttribute(item);
                member_attrs.XmlArray = array_attr;
                member_attrs.XmlArrayItems.Add(item_attr);
                var overrides = new XmlAttributeOverrides();
                overrides.Add(defs_type, type_attrs);
                overrides.Add(defs_type, "Defs", member_attrs);
                serializer = new XmlSerializer(defs_type, overrides);
                serializers.Add(type, serializer);
            }
            if(string.IsNullOrEmpty(modpath))
            {
                var paths = ModsMgr.Instance.GetPath(localpath).Where(pd => pd.mod != null).Select(pd => pd.path); // we ignore vanilla files.
                foreach (var path in paths)
                {
                    var files = Directory.GetFiles(path, pattern, SearchOption.AllDirectories);
                    foreach (var file in files)
                    {
                        using (var fs = new FileStream(file, FileMode.Open))
                        {
                            var defs = (DefsEx<T>)serializer.Deserialize(fs);
                            ret.AddRange(defs.Defs);
                        }
                    }
                }
            }
            else
            {
                var path = Path.Combine(modpath, localpath);
                var files = Directory.GetFiles(path, pattern, SearchOption.AllDirectories);
                foreach (var file in files)
                {
                    using (var fs = new FileStream(file, FileMode.Open))
                    {
                        var defs = (DefsEx<T>)serializer.Deserialize(fs);
                        ret.AddRange(defs.Defs);
                    }
                }
            }
            return ret;
        }
    }
}
