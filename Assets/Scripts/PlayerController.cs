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

        void Start()
        {
        }

        protected override void AtUpdate()
        {
            // Receive Inputs
            velocity.x += Input.GetAxis("Horizontal") * acceleration;
            velocity.y += Input.GetAxis("Vertical") * acceleration;

            if (velocity.sqrMagnitude >= maxSpeed * maxSpeed)
            {
                velocity = velocity.normalized * maxSpeed;
            }
        }

        void Move()
        {

        }
    }
}