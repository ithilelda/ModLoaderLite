using FairyGUI;
using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using XiaWorld;

namespace ModLoaderLite.Config
{
    public static class Configuration
    {
        static ConfigWindow wnd
        {
            get => UILogicMgr.GetWindow<ConfigWindow>();
        }

        public static void Show() => wnd.Show();
        public static void Hide() => wnd.Hide();

        public static void Subscribe(EventCallback0 cb) => wnd.ConfigUpdated += cb;
        public static void Unsubscribe(EventCallback0 cb) => wnd.ConfigUpdated -= cb;

        public static void Save()
        {
            MLLMain.AddOrOverWriteSave("ModLoaderLite.Config", wnd.ListItems);
        }
        public static void Load()
        {
            var t = MLLMain.GetSaveOrNull<Dictionary<string, Dictionary<string, ConfigItem>>>("ModLoaderLite.Config");
            if (t != null)
            {
                foreach (var modpair in t)
                {
                    if (!wnd.ListItems.TryGetValue(modpair.Key, out var dict)) continue;
                    foreach (var itempair in modpair.Value)
                    {
                        dict.UpdateConfig(itempair.Key, itempair.Value);
                    }
                    wnd.ListItems[modpair.Key] = dict;
                }
                wnd.OnConfigClicked();
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

        private static void UpdateConfig(this Dictionary<string, ConfigItem> d, string key, ConfigItem other)
        {
            // we only update configs that are added by mods. missing configs will not be added.
            // also, if the type doesn't match, we don't update either.
            if (d.TryGetValue(key, out var self) && self.Type == other.Type)
            {
                // we do not want to preserve the title, because we may change it.
                other.Title = self.Title;
                d[key] = other;
            }
        }
    }
}
