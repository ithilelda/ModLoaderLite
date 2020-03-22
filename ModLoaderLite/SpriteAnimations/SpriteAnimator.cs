using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ModLoaderLite.SpriteAnimations
{
    public class SpriteAnimator : MonoBehaviour
    {
        float duration;
        float frametime = 1f;
        int frame;
        SpriteRenderer renderer;
        List<Sprite> sprites;

        // unity messages.
        void Start()
        {
            renderer = GetComponent<SpriteRenderer>();
            renderer.sortingLayerID = 59623437;
        }

        void Update()
        {
            duration += Time.deltaTime;
            if (duration >= frametime)
            {
                if (sprites != null)
                {
                    renderer.sprite = sprites[frame];
                    frame++;
                    if (frame >= sprites.Count) frame = 0;
                }
                duration = 0f;
            }
        }

        // our api.
        public void Init(List<Sprite> ss, float fps)
        {
            sprites = ss;
            frametime = 1.0f / fps;
        }
    }
}
