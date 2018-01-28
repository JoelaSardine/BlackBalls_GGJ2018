using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlackBalls
{
    public class StreamZone : MonoBehaviour
    {
        public BoxCollider col;
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

        //private void OnTriggerEnter(Collider other)
        //{
        //    AtTriggerEnter(other);
        //}

        //private void OnTriggerExit(Collider other)
        //{
        //    AtTriggerExit(other);
        //}

        //protected virtual void AtTriggerEnter(Collider other)
        //{
        //    EntityController ec = other.GetComponent<EntityController>();
        //    if (ec != null)
        //    {
        //        ec.AddStream(this);
        //    }
        //}

        //protected virtual void AtTriggerExit(Collider other)
        //{
        //    EntityController ec = other.GetComponent<EntityController>();
        //    if (ec != null)
        //    {
        //        ec.RemoveStream(this);
        //    }
        //}

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawLine(transform.position, transform.position + direction * 10);
        }

        private void OnDrawGizmos()
        {
            if (col)
            {
                Matrix4x4 oldMat = Gizmos.matrix;
                Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
                Gizmos.matrix = rotationMatrix;

                Color c = Color.green;
                c.a = 0.1f;
                Gizmos.color = c;
                Gizmos.DrawCube(col.center, col.size);

                Gizmos.matrix = oldMat;
            }
        }
    }
}