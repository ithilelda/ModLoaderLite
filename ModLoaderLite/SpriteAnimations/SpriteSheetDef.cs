using System.Collections.Generic;
using UnityEngine;

namespace ModLoaderLite.SpriteAnimations
{
    public class SpriteSheetDef
    {
        public string Name { get; set; }
        public string Texture { get; set; }
        public Vector2 Center { get; set; }
        public int Column { get; set; }
        public int Row { get; set; }
        public int Count { get; set; }
        public float FPS { get; set; }
        public float Scale { get; set; }
        public List<Rect> Maps { get; set; }
    }
}
