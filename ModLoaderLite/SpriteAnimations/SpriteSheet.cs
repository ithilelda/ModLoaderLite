using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ModLoaderLite.SpriteAnimations
{
    public class SpriteSheet
    {
        public string Name { get; set; }
        public List<Sprite> Sprites { get; set; }
        public float FPS { get; set; }
    }
}
