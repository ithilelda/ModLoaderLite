using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ModLoaderLite.SpriteAnimations
{
    public class SpriteAnimator : MonoBehaviour
    {
        float duration, frameTime;
        SpriteRenderer renderer;
        IEnumerator<Sprite> iterator;

        public SpriteSheet SpriteSheet { get; set; }


        // unity messages.
        void Start()
        {
            renderer = gameObject.AddComponent<SpriteRenderer>();
            renderer.sortingLayerID = 59623437;
            frameTime = 1f / (SpriteSheet?.FPS).GetValueOrDefault(12f);
            iterator = SpriteSheet?.Sprites?.GetEnumerator();
        }

        void Update()
        {
            duration += Time.deltaTime;
            if (duration >= frameTime)
            {
                if (iterator?.MoveNext() == true)
                {
                    renderer.sprite = iterator.Current;
                }
                else iterator?.Reset();
                duration = 0f;
            }
        }

    }
}
