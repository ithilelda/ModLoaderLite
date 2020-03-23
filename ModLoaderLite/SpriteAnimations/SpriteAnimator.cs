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
        SpriteRenderer renderer;
        IEnumerator<Sprite> iterator;

        public List<Sprite> Sprites { get; set; } = new List<Sprite>();
        public float frameTime { get; set; } = 1f;

        // unity messages.
        void Start()
        {
            renderer = gameObject.AddComponent<SpriteRenderer>();
            renderer.sortingLayerID = 59623437;
            iterator = Sprites.GetEnumerator();
        }

        void Update()
        {
            duration += Time.deltaTime;
            if (duration >= frameTime)
            {
                if (iterator.MoveNext())
                {
                    renderer.sprite = iterator.Current;
                }
                else iterator.Reset();
                duration = 0f;
            }
        }
    }
}
