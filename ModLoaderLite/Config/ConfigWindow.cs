using FairyGUI;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ModLoaderLite.Config
{
    [Serializable]
    abstract class ConfigItem
    {
        public int Type;
        public string Title;
    }
    [Serializable]
    class CheckBox : ConfigItem
    {
        public bool Checked;
    }
    [Serializable]
    class Input : ConfigItem
    {
        public string Value;
    }
    [Serializable]
    class DropDown : ConfigItem
    {
        public string[] Values;
        public string Value;
    }
    class ConfigWindow : Window
    {
        public event EventCallback0 ConfigUpdated;
        public GComponent Frame;
        public GList ConfigList;
        public GButton Enter;
        public Dictionary<string, Dictionary<string, ConfigItem>> ListItems = new Dictionary<string, Dictionary<string, ConfigItem>>();
        public ConfigWindow()
        {
            contentPane = UIPackage.CreateObject("ModLoaderLite", "ConfigWindow").asCom;
            Frame = contentPane.GetChild("frame").asCom;
            Frame.text = "MLL设置";
            closeButton = Frame.GetChild("n5");
            ConfigList = contentPane.GetChild("n1").asList;
            Enter = contentPane.GetChild("enter").asButton;
            Enter.onClick.Add(OnEnterClicked);
        }

        protected override void OnShowUpdate(params object[] objs)
        {
            Center();
            ConfigList.RemoveChildrenToPool();
            foreach (var modpair in ListItems)
            {
                var mod = ModsMgr.Instance.FindMod(modpair.Key, null, true);
                if (mod == null) continue;
                var item = ConfigList.AddItemFromPool().asButton;
                item.title = mod.DisplayName;
                item.GetChild("mod").text = modpair.Key;
                item.GetChild("id").text = "Title";
                item.GetController("type").selectedIndex = 0;
                foreach (var itempair in modpair.Value)
                {
                    var config = itempair.Value;
                    var button = ConfigList.AddItemFromPool().asButton;
                    button.title = config.Title;
                    button.GetController("type").selectedIndex = config.Type;
                    button.GetChild("mod").text = modpair.Key;
                    button.GetChild("id").text = itempair.Key;
                    switch (config.Type)
                    {
                        case 1:
                            var cb = (CheckBox)config;
                            var checkbox = button.GetChild("cb").asButton;
                            checkbox.selected = cb.Checked;
                            break;
                        case 2:
                            var ip = (Input)config;
                            button.GetChild("ip").asLabel.text = ip.Value;
                            break;
                        case 3:
                            var dd = (DropDown)config;
                            var dropdown = button.GetChild("dd").asComboBox;
                            dropdown.items = dd.Values;
                            dropdown.values = dd.Values;
                            dropdown.value = dd.Value;
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        // events.
        private void OnEnterClicked()
        {
            foreach(var obj in ConfigList.GetChildren())
            {
                var item = obj.asButton;
                var type = item.GetController("type");
                var mod = item.GetChild("mod").text;
                var id = item.GetChild("id").text;
                if(id != "Title")
                {
                    var config = ListItems[mod][id];
                    switch (type.selectedIndex)
                    {
                        case 1:
                            var cb = (CheckBox)config;
                            var checkbox = item.GetChild("cb").asButton;
                            cb.Checked = checkbox.selected;
                            break;
                        case 2:
                            var ip = (Input)config;
                            ip.Value = item.GetChild("ip").asLabel.text;
                            break;
                        case 3:
                            var dd = (DropDown)config;
                            var dropdown = item.GetChild("dd").asComboBox;
                            dd.Value = dropdown.value;
                            break;
                        default:
                            break;
                    }
                }
            }
            ConfigUpdated?.Invoke();
            Hide();
        }
    }
}
