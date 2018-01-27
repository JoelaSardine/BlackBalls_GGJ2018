using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlackBalls
{
    public class IAController : EntityController
    {
        float timer;
        public float ImpulseDelay;
        Vector3 rdm;
        float x_accel;
        float y_accel;


        private void Start()
        {
            timer = Random.Range(0, ImpulseDelay);
            
            int x = Random.Range(-1, 1);
            int y = Random.Range(-1, 1);
            rdm = new Vector3(x, y);
        }

        protected override void AtUpdate(ref Vector3 finalVelocity)
        {
            x_accel += Random.Range(-0.01f, 0.01f);
            y_accel += Random.Range(-0.01f, 0.01f);
            rdm.x += x_accel; 
            rdm.y += y_accel;
            //finalVelocity += rdm.normalized;
        }
    }
}