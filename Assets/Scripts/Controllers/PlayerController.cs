﻿using System.Collections;
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

        public TrailZoneManager trailZoneManager;

        public Boids.SpecialBoid theOtherOne;
        private bool hasKnownLove = false;

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
                if (finalVelocity >= 3.5f)
                {
                    if (trailZoneManager)
                    {
                        TrailBoidZone newZone = Instantiate(trailzonePrefab, transform.position, Quaternion.identity, zonesParent).GetComponent<TrailBoidZone>();
                        newZone.force = finalVelocity - 2f;
                        newZone.range = newZone.transform.localScale.x;
                        newZone.direction = sprite.up;
                        trailZoneManager.trails.Add(newZone);
                    }
                    else
                    {
                        Trailzone newZone = Instantiate(trailzonePrefab, transform.position, Quaternion.identity, zonesParent).GetComponent<Trailzone>();
                        newZone.force = finalVelocity;
                        newZone.direction = sprite.up;
                    }
                }
                timer = 0.1f;
            }
        }


        private void Update()
        {
            if (hasKnownLove)
            {
                if (theOtherOne.isFollowingPlayer = true)
                {
                    if (Vector3.SqrMagnitude(transform.position - theOtherOne.transform.position) >= 25.0f)
                    {
                        Debug.Log("Lost");
                        theOtherOne.isFollowingPlayer = false;
                    }
                }
            }
            else
            {
                if (Input.GetButtonDown("Calin"))
                {
                    if (Vector3.SqrMagnitude(transform.position - theOtherOne.transform.position) <= 10.0f)
                    {
                        Debug.Log("Calin OK");
                        hasKnownLove = true;
                        theOtherOne.isFollowingPlayer = true;
                    }
                }
            }
        }
    }
}