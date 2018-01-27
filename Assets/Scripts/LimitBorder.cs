using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlackBalls
{
    public class LimitBorder : MonoBehaviour
    {
        public delegate void OnEntityEnterDelegate();
        public OnEntityEnterDelegate OnEntityEnter;
        public BoxCollider col;

        private void OnTriggerEnter(Collider other)
        {
            EntityController ec = other.GetComponent<EntityController>();
            if (ec != null)
            {
                Destroy(ec.gameObject);
            }

            if (OnEntityEnter != null)
            {
                OnEntityEnter();
            }
        }

        private void OnDrawGizmos()
        {
            Color c = Color.black;
            c.a = 0.5f;
            Gizmos.color = c;
            Gizmos.DrawCube(transform.position + col.center, col.size);
        }
    }
}