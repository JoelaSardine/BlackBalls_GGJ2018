using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlackBalls
{
    public class Trailzone : StreamZone
    {
        public float force;

        void Start()
        {

        }

        void Update()
        {
            force -= Time.deltaTime;
            if (force <= 0)
            {
                Destroy(this.gameObject);
            }
            else
            {
                transform.localScale += Vector3.one * 0.01f;
                //transform.localScale *= 1.008f;
            }
        }

        protected override void AtTriggerEnter(Collider other)
        {}

        protected override void AtTriggerExit(Collider other)
        {}

        private void OnTriggerStay(Collider other)
        {
            IAController ia = other.GetComponent<IAController>();
            if (ia != null)
            {
                ia.trailVelocity += direction * force;
            }
        }
    }
}