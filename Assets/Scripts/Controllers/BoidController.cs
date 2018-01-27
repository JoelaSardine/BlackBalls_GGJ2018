using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlackBalls.Boids
{
    public class BoidController : MonoBehaviour
    {
        public Transform spriteTransform;
        
        public float speed = 1.0f;
        public bool isFollowing = false;

        public Vector3 targetVelocity;
        public Vector3 currentVelocity;

        void Start()
        {
            float x, y;
            x = Random.Range(-1f, 1f);
            y = Random.Range(-1f, 1f);
            currentVelocity = (new Vector3(x, y)).normalized;
        }

        void Update()
        {
            currentVelocity += targetVelocity * Time.deltaTime;
            currentVelocity.Normalize();

            transform.position += currentVelocity * Time.deltaTime * speed * (isFollowing ? 2 : 1);

            // 2D Lookat
            Vector3 rotation = new Vector3();
            rotation.z = Mathf.Rad2Deg * (currentVelocity.x < 0 ? Mathf.Acos(currentVelocity.y) : -Mathf.Acos(currentVelocity.y));
            spriteTransform.rotation = Quaternion.Euler(rotation);
            
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, targetVelocity + transform.position);
        }
    }
}