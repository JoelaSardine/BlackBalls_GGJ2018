using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlackBalls
{
    public class StreamZone : MonoBehaviour
    {
        public Vector3 direction;
        public float force;

        // Use this for initialization
        void Start()
        {
            direction.Normalize();
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("Enter " + other.name);
        }

        private void OnTriggerExit(Collider other)
        {
            Debug.Log("Exit " + other.name);
        }

        private void OnTriggerStay(Collider other)
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();

            if (null != rb)
            {
                rb.AddForce(direction * force);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawLine(transform.position, transform.position + direction * 10);
        }
    }
}