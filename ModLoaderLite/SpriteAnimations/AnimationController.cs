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
        public void AddSpriteAnimation(string name)
        {
            var spriteSheet = SpriteManager.GetSpriteSheet(name);
            if (spriteSheet != null)
            {
                var go = new GameObject();
                var animator = go.AddComponent<SpriteAnimator>();
                animator.SpriteSheet = spriteSheet;
                go.transform.parent = gameObject.transform;
                gos.Add(go);
            }
        }
    }
}
