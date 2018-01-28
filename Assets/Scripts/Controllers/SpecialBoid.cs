using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlackBalls.Boids
{
    public class SpecialBoid : BoidController
    {
        public Material lostMaterial;
        public Renderer myRenderer;

        public bool isFollowingPlayer = false;

        private float timer = 10f;

        public void ChangeMaterial()
        {
            myRenderer.material = lostMaterial;
        }

        protected override float calculateSpeed()
        {
            return isFollowingPlayer ? speed * 1.8f : speed;
        }

        protected override void ChangeCurrentVelocity()
        {
            currentVelocity += targetVelocity * (Time.deltaTime * 2);
        }

        protected override void AtUpdate()
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                timer = Random.Range(5f, 20f);
                AkSoundEngine.PostEvent("Play_companion", gameObject);
            }
        }
    }
}