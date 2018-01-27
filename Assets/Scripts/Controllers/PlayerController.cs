using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlackBalls
{
    public class PlayerController : EntityController
    {
        //private Rigidbody rb;
        public float acceleration = 1f;
        public float maxSpeed = 1f;

        public Transform zonesParent;
        public GameObject trailzonePrefab;

        float timer;

        void Start()
        {
        }

        protected override void AtUpdate(ref Vector3 finalVelocity)
        {
            // Receive Inputs
            velocity.x += Input.GetAxis("Horizontal") * acceleration;
            velocity.y += Input.GetAxis("Vertical") * acceleration;

            if (velocity.sqrMagnitude >= maxSpeed * maxSpeed)
            {
                velocity = velocity.normalized * maxSpeed;
            }

            ManageTrailZone(finalVelocity.magnitude);
        }


        private void ManageTrailZone(float finalVelocity)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                if (finalVelocity > 2f)
                {
                    Trailzone newZone = Instantiate(trailzonePrefab, transform.position, Quaternion.identity, zonesParent).GetComponent<Trailzone>();
                    
                    newZone.force = finalVelocity;
                    newZone.direction = sprite.up;
                }
                timer = 0.1f;
            }
        }
    }
}