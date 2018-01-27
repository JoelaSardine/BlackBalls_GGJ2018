using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlackBalls
{
    public class EntityController : MonoBehaviour
    {
        protected Vector3 velocity;
        public Transform sprite;
        public float drag;


        void Start()
        {

        }

        void Update()
        {
            AtUpdate();

            transform.position += velocity * Time.deltaTime;
            velocity *= (1 - drag);

            LookAtDirection();
        }

        protected virtual void AtUpdate()
        {

        }

        private void LookAtDirection()
        {
            Vector3 veloOne = velocity.normalized;
            if (veloOne.sqrMagnitude >= 0.1f)
            {
                Vector3 rotation = new Vector3();
                rotation.z = Mathf.Rad2Deg * (veloOne.x < 0 ? Mathf.Acos(veloOne.y) : -Mathf.Acos(veloOne.y));
                Debug.Log(rotation.z);
                sprite.rotation = Quaternion.Euler(rotation);
            }
        }
    }
}