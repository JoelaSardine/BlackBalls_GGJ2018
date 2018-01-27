using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlackBalls
{
    public class StreamZone : MonoBehaviour
    {
        public Vector3 direction;
        public float maxForce;

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
            EntityController ec = other.GetComponent<EntityController>();
            if (ec != null)
            {
                ec.AddStream(this);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            EntityController ec = other.GetComponent<EntityController>();
            if (ec != null)
            {
                ec.RemoveStream(this);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawLine(transform.position, transform.position + direction * 10);
        }
    }
}