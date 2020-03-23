using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace ModLoaderLite.SpriteAnimations
{
    public class AnimationController : MonoBehaviour
    {
        List<GameObject> gos = new List<GameObject>();

        void OnDestroy()
        {
            foreach (var go in gos)
            {
                Destroy(go);
            }
            gos.Clear();
        }

        // our api.
        public void AddSpriteAnimation(string texpath, int column = 1, int row = 1, float fps = 10f, float scale = 1f)
        {
            var sprites = SpriteLoader.LoadSpriteSheet(texpath, column, row, 100f / scale);
            if (sprites != null)
            {
                var go = new GameObject();
                var animator = go.AddComponent<SpriteAnimator>();
                animator.Sprites = sprites;
                animator.frameTime = 1 / fps;
                go.transform.parent = gameObject.transform;
                gos.Add(go);
            }
        }
        public void AddSprite(string texpath, float scale = 1f)
        {
            var sprite = SpriteLoader.LoadSprite(texpath, 100f / scale);
            if (sprite != null)
            {
                var go = new GameObject();
                var animator = go.AddComponent<SpriteAnimator>();
                animator.Sprites = new List<Sprite> { sprite };
                go.transform.parent = gameObject.transform;
                gos.Add(go);
            }
        }
    }
}
