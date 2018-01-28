using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlackBalls
{
    public class EntityController : MonoBehaviour
    {
        //public List<StreamZone> streams = new List<StreamZone>();
        //private List<Vector3> streamsVelo = new List<Vector3>();
        public Vector3 residualStreamVelocity;

        protected Vector3 finalVelocity;
        public Vector3 velocity;
        public Transform sprite;
        public float drag;

        public Vector3 trailVelocity;

        void Start()
        {
        }

        void FixedUpdate()
        {
            finalVelocity = velocity;
            
            //if (trailVelocity.sqrMagnitude < 1f)
            //{
            //    ApplyStreams(ref finalVelocity);
            //}
            //else
            //{
            //    finalVelocity += trailVelocity.normalized;
            //}

            AtUpdate(ref finalVelocity);

            transform.position += finalVelocity * Time.deltaTime;
            velocity *= (1 - drag);

            if (velocity.sqrMagnitude >= 10)
            {
                // Only for player
                LookAtDirection(velocity);
            }
            else
            {
                LookAtDirection(finalVelocity);
            }

            trailVelocity = Vector3.zero;
        }

        protected virtual void AtUpdate(ref Vector3 finalVelocity)
        {

        }

        private void LookAtDirection(Vector3 direction)
        {
            Vector3 veloOne = direction.normalized;
            if (veloOne.sqrMagnitude >= 0.1f)
            {
                Vector3 rotation = new Vector3();
                rotation.z = Mathf.Rad2Deg * (veloOne.x < 0 ? Mathf.Acos(veloOne.y) : -Mathf.Acos(veloOne.y));
                sprite.rotation = Quaternion.Euler(rotation);
            }
        }

        //public void AddStream(StreamZone stream)
        //{
        //    if (!streams.Contains(stream))
        //    {
        //        streams.Add(stream);
        //        streamsVelo.Add(Vector3.zero);
        //    }
        //}

        //public void RemoveStream(StreamZone stream)
        //{
        //    int i = streams.IndexOf(stream);
        //    residualStreamVelocity += streamsVelo[i];
        //    streams.RemoveAt(i);
        //    streamsVelo.RemoveAt(i);
        //}

        //private void ApplyStreams(ref Vector3 finalVelocity)
        //{
        //    residualStreamVelocity *= (1 - drag);

        //    for (int i = 0; i < streams.Count; i++)
        //    {
        //        streamsVelo[i] += streams[i].direction * Time.deltaTime;
        //        if (streamsVelo[i].sqrMagnitude >= streams[i].maxForce * streams[i].maxForce)
        //        {
        //            streamsVelo[i] = streamsVelo[i].normalized * streams[i].maxForce;
        //        }

        //        finalVelocity += streamsVelo[i];
        //    }

        //    finalVelocity += residualStreamVelocity;
        //}
    }
}