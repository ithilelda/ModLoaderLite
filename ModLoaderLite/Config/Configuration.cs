using FairyGUI;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

namespace ModLoaderLite.Config
{
    public static class Configuration
    {
        private static ConfigWindow wnd = new ConfigWindow();
        public static void Show() => wnd.Show();
        public static void Hide() => wnd.Hide();

        public static void Subscribe(EventCallback0 cb) => wnd.ConfigUpdated += cb;
        public static void Unsubscribe(EventCallback0 cb) => wnd.ConfigUpdated -= cb;

        public static void Serialize(Stream stream)
        {
            var formatter = new BinaryFormatter();
            formatter.Serialize(stream, wnd.ListItems);
        }
        public static void Deserialize()
        {
            var fileName = GameWatch.Instance.LoadFile;
            var fullName = Path.Combine(Directory.GetCurrentDirectory(), $"saves/{fileName}.mll");
            if (File.Exists(fullName))
            {
                using (var fs = new FileStream(fullName, FileMode.Open))
                {
                    var formatter = new BinaryFormatter();
                    var t = (Dictionary<string, Dictionary<string, ConfigItem>>)formatter.Deserialize(fs);
                    foreach(var modpair in t)
                    {
                        if (!wnd.ListItems.TryGetValue(modpair.Key, out var dict)) continue;
                        foreach(var itempair in modpair.Value)
                        {
                            dict[itempair.Key] = itempair.Value;
                        }
                        wnd.ListItems[modpair.Key] = dict;
                    }
                }
            }
        }

        public static void AddCheckBox(string modid, string id, string title, bool init=default)
        {
            if(!wnd.ListItems.TryGetValue(modid, out var dict))
            {
                dict = new Dictionary<string, ConfigItem>();
            }
            var item = new CheckBox { Type = 1, Title = title, Checked = init };
            if(!dict.ContainsKey(id)) dict.Add(id, item);
            wnd.ListItems[modid] = dict;
        }
        public static bool GetCheckBox(string modid, string id)
        {
            if (wnd.ListItems.TryGetValue(modid, out var dict))
            {
                if(dict.TryGetValue(id, out var item))
                {
                    return ((CheckBox)item).Checked;
                }
            }
            return default;
        }
        public static void SetCheckBox(string modid, string id, bool value)
        {
            if (wnd.ListItems.TryGetValue(modid, out var dict))
            {
                if (dict.TryGetValue(id, out var item))
                {
                    ((CheckBox)item).Checked = value;
                }
            }
        }

        public static void AddInput(string modid, string id, string title, string init=default)
        {
            if (!wnd.ListItems.TryGetValue(modid, out var dict))
            {
                dict = new Dictionary<string, ConfigItem>();
            }
            var item = new Input { Type = 2, Title = title, Value = init };
            if (!dict.ContainsKey(id)) dict.Add(id, item);
            wnd.ListItems[modid] = dict;
        }
        public static string GetInput(string modid, string id)
        {
            if (wnd.ListItems.TryGetValue(modid, out var dict))
            {
                if (dict.TryGetValue(id, out var item))
                {
                    return ((Input)item).Value;
                }
            }
            return default;
        }
        public static void SetInput(string modid, string id, string value)
        {
            if (wnd.ListItems.TryGetValue(modid, out var dict))
            {
                if (dict.TryGetValue(id, out var item))
                {
                    ((Input)item).Value = value;
                }
            }
        }

        public static void AddDropDown(string modid, string id, string title, params string[] contents)
        {
            if (!wnd.ListItems.TryGetValue(modid, out var dict))
            {
                dict = new Dictionary<string, ConfigItem>();
            }
            var item = new DropDown { Type = 2, Title = title, Values = contents, Value = contents[0] };
            if (!dict.ContainsKey(id)) dict.Add(id, item);
            wnd.ListItems[modid] = dict;
        }
        public static string GetDropDown(string modid, string id)
        {
            if (wnd.ListItems.TryGetValue(modid, out var dict))
            {
                if (dict.TryGetValue(id, out var item))
                {
                    return ((DropDown)item).Value;
                }
            }
            return default;
        }
        public static void SetDropDown(string modid, string id, string value)
        {
            if (wnd.ListItems.TryGetValue(modid, out var dict))
            {
                if (dict.TryGetValue(id, out var item))
                {
                    ((DropDown)item).Value = value;
                }
            }
        }
    }
}
