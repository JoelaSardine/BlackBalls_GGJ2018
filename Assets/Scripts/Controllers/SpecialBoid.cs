using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlackBalls.Boids
{
    public class SpecialBoid : BoidController
    {
        public bool isFollowingPlayer = false;
        
        protected override float calculateSpeed()
        {
            return isFollowingPlayer ? speed * 1.8f : speed;
        }

        protected override void ChangeCurrentVelocity()
        {
            currentVelocity += targetVelocity * (Time.deltaTime * 2);
        }
    }
}