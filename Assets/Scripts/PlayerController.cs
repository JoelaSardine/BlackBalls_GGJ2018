using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlackBalls
{
    public class PlayerController : MonoBehaviour
    {
        private Rigidbody rb;

        public float speedForce = 100f;

        void Start()
        {
            rb = GetComponent<Rigidbody>();
        }

        void Update()
        {
            // Receive Inputs

            rb.AddForce(Input.GetAxis("Horizontal") * speedForce, Input.GetAxis("Vertical") * speedForce, 0);
        }

        void Move()
        {

        }
    }
}