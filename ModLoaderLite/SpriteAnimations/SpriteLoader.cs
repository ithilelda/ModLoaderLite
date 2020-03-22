using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ModLoaderLite.SpriteAnimations
{
    public static class SpriteLoader
    {
        static Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();
        static Dictionary<string, List<Sprite>> spriteSheets = new Dictionary<string, List<Sprite>>();
        public static Sprite LoadSprite(string localpath)
        {
            if (!sprites.TryGetValue(localpath, out var sprite))
            {
                var tex = RenderPool.Instance.GetTexture2D(localpath);
                if (tex != null)
                {
                    sprite = Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), new Vector2(0.5f, 0f));
                    sprites.Add(localpath, sprite);
                }
            }
            return sprite;
        }
        public static Sprite GetSprite(string path) => sprites.TryGetValue(path, out var ret) ? ret : default;

        public static List<Sprite> LoadSpriteSheet(string localpath, int column, int row)
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
                            var sprite = Sprite.Create(tex, new Rect(c * width, r * height, width, height), new Vector2(0.5f, 0f));
                            sprites.Add(sprite);
                        }
                    }
                    spriteSheets.Add(localpath, sprites);
                }
            }
            return sprites;
        }
        public static GameObject CreateSpecialEffect(string texpath, int column=1, int row=1, float fps=10f)
        {
            var sprites = LoadSpriteSheet(texpath, column, row);
            var go = new GameObject();
            if (sprites != null)
            {
                KLog.Dbg($"sprite sheet {texpath} loading succeeded!");
                go.AddComponent<SpriteRenderer>();
                var animator = go.AddComponent<SpriteAnimator>();
                animator.Init(sprites, fps);
            }
            return go;
        }
        public static GameObject CreateSingleEffect(string texpath)
        {
            var sprite = LoadSprite(texpath);
            var go = new GameObject();
            if (sprite != null)
            {
                KLog.Dbg($"single sprite {texpath} loading succeeded!");
                go.AddComponent<SpriteRenderer>().sprite = sprite;
                var animator = go.AddComponent<SpriteAnimator>();
            }
            return go;
        }
    }
}
