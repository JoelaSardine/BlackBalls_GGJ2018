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
                transform.localScale *= 1.01f;
            }
        }

        protected override void AtTriggerEnter(Collider other)
        {
            IAController ia = other.GetComponent<IAController>();
            if (ia != null)
            {
                ia.AddStream(this);
            }
        }

        protected override void AtTriggerExit(Collider other)
        {
            IAController ia = other.GetComponent<IAController>();
            if (ia != null)
            {
                ia.RemoveStream(this);
            }
        }
    }
}