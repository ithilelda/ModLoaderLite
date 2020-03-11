using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace ModLoaderLite.XmlEx
{
    public static class XmlEx
    {
        static Dictionary<string, XmlSerializer> serializers = new Dictionary<string, XmlSerializer>();

        // this method is for loading vanilla valid xml. It has a <List> tag inside the root tag, so we have to wrap it around something.
        public static List<T> LoadXmlEx<T>(string localpath, string root, string item, string modpath = null, string pattern = "*.xml")
        {
            var defs_type = typeof(DefsEx<T>);
            // we only create a new serializer if we can't find one for the type DefsEx<T>.
            var id = Path.Combine(localpath, defs_type.ToString());
            if (!serializers.ContainsKey(id))
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
                var serializer = new XmlSerializer(defs_type, overrides);
                serializers.Add(id, serializer);
            }
            return LoadXmlFile<DefsEx<T>>(localpath, modpath, pattern).SelectMany(i => i.Defs).ToList();
        }
        // this method is for loading fully custom xmls. Such xml doesn't contain the List tag, so we don't have to wrap it around anything, so we can directly desrialize into a list.
        // providing option to specify the root element, so that you can wrap the list around anything.
        // using XmlType attribute in T to specify your list element name, in case it is different than your class T name.
        public static List<T> LoadXml<T>(string localpath, string root, string modpath = null, string pattern = "*.xml")
        {
            var type = typeof(List<T>);
            var id = Path.Combine(localpath, type.ToString());
            if (!serializers.ContainsKey(id))
            {
                var serializer = new XmlSerializer(type, new XmlRootAttribute(root));
                serializers.Add(id, serializer);
            }
            return LoadXmlFile<List<T>>(localpath, modpath, pattern).SelectMany(i => i).ToList();
        }

        static List<T> LoadXmlFile<T>(string localpath, string modpath, string pattern)
        {
            var ret = new List<T>();
            var id = Path.Combine(localpath, typeof(T).ToString());
            if (serializers.TryGetValue(id, out var serializer))
            {
                if (string.IsNullOrEmpty(modpath))
                {
                    var paths = ModsMgr.Instance.GetPath(localpath).Where(pd => pd.mod != null).Select(pd => pd.path); // we ignore vanilla files.
                    foreach (var path in paths)
                    {
                        var files = Directory.GetFiles(path, pattern, SearchOption.AllDirectories);
                        foreach (var file in files)
                        {
                            using (var fs = new FileStream(file, FileMode.Open))
                            {
                                var defs = (T)serializer.Deserialize(fs);
                                ret.Add(defs);
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
                            var defs = (T)serializer.Deserialize(fs);
                            ret.Add(defs);
                        }
                    }
                }
            }
            return ret;
        }
    }
}
