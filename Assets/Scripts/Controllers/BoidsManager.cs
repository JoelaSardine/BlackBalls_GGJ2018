using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlackBalls.Boids
{
    public class BoidsManager : MonoBehaviour
    {
        public TrailZoneManager trailZoneManager;
        public List<BoidController> entities;
        public PlayerController player;

        public GameObject entityPrefab;
        public int entityCount;
        public Vector2 entitySpawnSurface;

        public float detectionRange = 10f;
        public float repulsionRange = 1f;

        [Header("Factors")]
        public float cohesionForce = 1f;
        public float repulsionForce = 1f;
        public float imitationForce = 1f;
        public float followForce = 1f;

        //public LimitBorder[] limitBorder;

        void Start()
        {
            //foreach (var border in limitBorder)
            //{
            //    border.OnEntityEnter += OnEntityIsDestroyed;
            //}

            float x, y;
            for (int i = 0; i < entityCount; i++)
            {
                x = Random.Range(-entitySpawnSurface.x / 2, entitySpawnSurface.x / 2);
                y = Random.Range(-entitySpawnSurface.y / 2, entitySpawnSurface.y / 2);
                BoidController boid = Instantiate(entityPrefab, new Vector3(x, y, 0), Quaternion.identity, transform).GetComponent<BoidController>();
                entities.Add(boid);
            }
        }

        void Update()
        {
            foreach (var boid in entities)
            {
                Vector3 pos = boid.transform.position;

                boid.targetVelocity = Vector3.zero;
                Vector3 cohesion = Vector3.zero;
                Vector3 repulsion = Vector3.zero;
                Vector3 imitation = Vector3.zero;
                Vector3 follow = Vector3.zero;
                Vector3 barycenter = Vector3.zero;
                boid.isFollowing = false;
                int n = 0;

                foreach (var other in entities)
                {
                    // Doesn't consider himself
                    if (other == boid) continue;

                    float squaredDistance = (other.transform.position - pos).sqrMagnitude;
                    if (squaredDistance < detectionRange * detectionRange)
                    {
                        // Cohesion
                        barycenter += other.transform.position;
                        n++;

                        // Imitation
                        imitation += other.currentVelocity;

                        // Repulsion
                        if (squaredDistance <= repulsionRange * repulsionRange)
                        {
                            repulsion += (pos - other.transform.position);
                        }
                    }
                }

                if ((player.transform.position - pos).sqrMagnitude <= repulsionRange * repulsionRange)
                {
                    repulsion += (pos - player.transform.position);
                }

                cohesion = ((barycenter / n) - pos).normalized;
                repulsion = repulsion.normalized;
                imitation = imitation.normalized;

                // Courants
                // TODO

                // Trail
                for (int i = trailZoneManager.trails.Count - 1; i >= 0; --i)
                {
                    TrailBoidZone zone = trailZoneManager.trails[i];
                    if ((pos - zone.transform.position).sqrMagnitude <= zone.range * zone.range)
                    {
                        follow = zone.direction;
                        boid.isFollowing = true;
                        break;
                    }
                }
                
                CheckLevelBorders(boid, pos);

                boid.targetVelocity = cohesion * cohesionForce 
                                    + repulsion * repulsionForce 
                                    + imitation * imitationForce
                                    + follow * followForce;
            }


        }

        private void CheckLevelBorders(BoidController boid, Vector3 pos)
        {
            if (pos.x <= -entitySpawnSurface.x / 2)
            {
                pos.x += entitySpawnSurface.x;
                boid.transform.position = pos;
            }
            else if (pos.x >= entitySpawnSurface.x / 2)
            {
                pos.x -= entitySpawnSurface.x;
                boid.transform.position = pos;
            }
            else if (pos.y >= entitySpawnSurface.y / 2)
            {
                pos.y -= entitySpawnSurface.y;
                boid.transform.position = pos;
            }
            else if (pos.y <= -entitySpawnSurface.y / 2)
            {
                pos.y += entitySpawnSurface.y;
                boid.transform.position = pos;
            }
        }
    }
}