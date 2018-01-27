using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlackBalls
{
    public class EntityController : MonoBehaviour
    {
        public List<StreamZone> streams = new List<StreamZone>();
        private List<Vector3> streamsVelo = new List<Vector3>();
        private Vector3 residualStreamVelocity;

        protected Vector3 velocity;
        public Transform sprite;
        public float drag;

        void Start()
        {
        }

        void Update()
        {
            AtUpdate();

            Vector3 finalVelocity = velocity;
            ApplyStreams(ref finalVelocity);

            transform.position += finalVelocity * Time.deltaTime;
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
                sprite.rotation = Quaternion.Euler(rotation);
            }
        }

        public void AddStream(StreamZone stream)
        {
            if (!streams.Contains(stream))
            {
                streams.Add(stream);
                streamsVelo.Add(Vector3.zero);
            }
        }

        public void RemoveStream(StreamZone stream)
        {
            int i = streams.IndexOf(stream);
            residualStreamVelocity += streamsVelo[i];
            streams.RemoveAt(i);
            streamsVelo.RemoveAt(i);
        }

        private void ApplyStreams(ref Vector3 finalVelocity)
        {
            residualStreamVelocity *= (1 - drag);

            for (int i = 0; i < streams.Count; i++)
            {
                streamsVelo[i] += streams[i].direction * Time.deltaTime;
                if (streamsVelo[i].sqrMagnitude >= streams[i].maxForce * streams[i].maxForce)
                {
                    streamsVelo[i] = streamsVelo[i].normalized * streams[i].maxForce;
                }

                finalVelocity += streamsVelo[i];
            }

            finalVelocity += residualStreamVelocity;
        }
    }
}