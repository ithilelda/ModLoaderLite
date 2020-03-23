using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using ModLoaderLite.JsonExs;


namespace ModLoaderLite.SpriteAnimations
{
    public static class SpriteManager
    {
        static Dictionary<string, SpriteSheet> spriteSheetsCache = new Dictionary<string, SpriteSheet>();
        static Dictionary<string, Texture2D> textureCache = new Dictionary<string, Texture2D>();
        
        public static SpriteSheet GetSpriteSheet(string name) => spriteSheetsCache.TryGetValue(name, out var ret) ? ret : default;

        internal static void LoadAll()
        {
            var defs = JsonEx.LoadJson<SpriteSheetDef>("Resources/SpriteSheets");
            foreach (var def in defs)
            {
                try
                {
                    LoadSpriteSheet(def);
                }
                catch (Exception e)
                {
                    KLog.Dbg(e.Message);
                    KLog.Dbg(e.StackTrace);
                }
            }
        }
        static bool LoadSpriteSheet(SpriteSheetDef def)
        {
            if (!spriteSheetsCache.ContainsKey(def.Name))
            {
                if (!textureCache.TryGetValue(def.Texture, out var tex))
                {
                    tex = ModsMgr.Instance.LoadTexture2D(Path.Combine("Resources/", def.Texture));
                    if (tex != null) textureCache.Add(def.Texture, tex);
                }
                if (tex != null)
                {
                    //KLog.Dbg($"the width of texture is: {tex.width}, the height is {tex.height}.");
                    var sprites = new List<Sprite>(def.Count);
                    if (def.Maps == null || def.Maps.Count != def.Count)
                    {
                        var width = (float)tex.width / def.Column;
                        var height = (float)tex.height / def.Row;
                        var maps = new List<Rect>();
                        //KLog.Dbg($"the width of each sprite is: {width}, the height is {height}.");
                        for (int i = 0; i < def.Count; i++)
                        {
                            //KLog.Dbg($"the position of current sprite is: {c * width}, {r * height}.");
                            var r = i / def.Column;
                            var c = i % def.Column;
                            maps.Add(new Rect(c * width, r * height, width, height));
                        }
                        def.Maps = maps;
                    }
                    foreach (var rect in def.Maps)
                    {
                        var sprite = Sprite.Create(tex, rect, def.Center, 100f / def.Scale);
                        sprites.Add(sprite);
                    }
                    spriteSheetsCache.Add(def.Name, new SpriteSheet { Name = def.Name, FPS = def.FPS, Sprites = sprites });
                    return true;
                }
            }
            return false;
        }
    }
}
