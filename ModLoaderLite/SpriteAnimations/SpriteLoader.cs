using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ModLoaderLite.SpriteAnimations
{
    static class SpriteLoader
    {
        static Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();
        static Dictionary<string, List<Sprite>> spriteSheets = new Dictionary<string, List<Sprite>>();
        public static Sprite LoadSprite(string localpath, float pixelsPerUnit)
        {
            if (!sprites.TryGetValue(localpath, out var sprite))
            {
                var tex = RenderPool.Instance.GetTexture2D(localpath);
                if (tex != null)
                {
                    sprite = Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), pixelsPerUnit);
                    sprites.Add(localpath, sprite);
                }
            }
            return sprite;
        }

        public static List<Sprite> LoadSpriteSheet(string localpath, int column, int row, float pixelsPerUnit)
        {
            if (!spriteSheets.TryGetValue(localpath, out var sprites))
            {
                var tex = RenderPool.Instance.GetTexture2D(localpath);
                if (tex != null)
                {
                    //KLog.Dbg($"the width of texture is: {tex.width}, the height is {tex.height}.");
                    sprites = new List<Sprite>(column * row);
                    var width = (float)tex.width / column;
                    var height = (float)tex.height / row;
                    //KLog.Dbg($"the width of each sprite is: {width}, the height is {height}.");
                    for (int r = 0; r < row; r++)
                    {
                        for (int c = 0; c < column; c++)
                        {
                            //KLog.Dbg($"the position of current sprite is: {c * width}, {r * height}.");
                            var sprite = Sprite.Create(tex, new Rect(c * width, r * height, width, height), new Vector2(0.5f, 0f), pixelsPerUnit);
                            sprites.Add(sprite);
                        }
                    }
                    spriteSheets.Add(localpath, sprites);
                }
            }
            return sprites;
        }
    }
}
