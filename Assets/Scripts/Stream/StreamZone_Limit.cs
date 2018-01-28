using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlackBalls
{
    public class StreamZone_Limit : StreamZone
    {
        //protected override void AtTriggerEnter(Collider other)
        //{
        //    PlayerController pc = other.GetComponent<PlayerController>();
        //    if (pc != null)
        //    {
        //        pc.AddStream(this);
        //    }
        //}

        //protected override void AtTriggerExit(Collider other)
        //{
        //    PlayerController pc = other.GetComponent<PlayerController>();
        //    if (pc != null)
        //    {
        //        pc.RemoveStream(this);
        //    }
        //}

        private void OnDrawGizmos()
        {
            Color c = Color.red;
            c.a = 0.1f;
            Gizmos.color = c;
            Gizmos.DrawCube(transform.position + col.center, col.size);
        }
    }    
}