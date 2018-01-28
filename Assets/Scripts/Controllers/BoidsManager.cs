using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlackBalls.Boids
{
    [System.Serializable]
    public class RaceStats
    {
        public string id;
        public GameObject prefab;
        public int population;
        public bool likes_A1;
        public bool likes_A2;
        public bool likes_B1;
        public bool likes_B2;
        public bool likes_C1;
        public bool likes_C2;
    }

    public class BoidsManager : MonoBehaviour
    {
        public int followersCount;

        [Header("To link")]
        public TrailZoneManager trailZoneManager;
        public PlayerController player;
        public SpecialBoid theOtherOne;
 
        [Header("Spawn values")]
        public Vector2 entitySpawnSurface;
        public RaceStats[] races;
        //public GameObject entityPrefab;
        //public int entityCount;
        
        [Header("Ranges")]
        public float streamDetectionRange = 10f;
        public float detectionRange = 10f;
        public float repulsionRange = 1f;

        [Header("Factors")]
        public float cohesionForce = 1f;
        public float repulsionForce = 1f;
        public float imitationForce = 1f;
        public float followForce = 1f;
        public float streamForce = 1f;

        //public LimitBorder[] limitBorder;

        private List<List<BoidController>> entitiesByRace = new List<List<BoidController>>();
        private List<StreamPoint> streamPoints;

        void Start()
        {
            // Get all stream points in the scene
            // Maybe we should use an octree
            StreamPoint[] p = GameObject.FindObjectsOfType<StreamPoint>();
            streamPoints = new List<StreamPoint>(p);

            float x, y, i;
            for (int r = 0; r < races.Length; ++r)
            {
                Transform ancester = new GameObject(races[r].id).GetComponent<Transform>();
                ancester.parent = this.transform;

                List<BoidController> racepopulation = new List<BoidController>(races[r].population);
                for (i = 0; i < races[r].population; ++i)
                {
                    x = Random.Range(-entitySpawnSurface.x / 2, entitySpawnSurface.x / 2);
                    y = Random.Range(-entitySpawnSurface.y / 2, entitySpawnSurface.y / 2);
                    BoidController boid = Instantiate(races[r].prefab, new Vector3(x, y, 0), Quaternion.identity, ancester).GetComponent<BoidController>();
                    racepopulation.Add(boid);
                }
                entitiesByRace.Add(racepopulation);
            }
        }

        void Update()
        {
            followersCount = 0;

            UpdateAllBoids();

            //foreach (var boid in entities)
            //{
            //    if (boid == theOtherOne)
            //    {
            //        UpdateOtherOne();
            //    }
            //    else
            //    {
            //        UpdateBoid(boid);
            //    }
            //}

            Debug.Log("Followers = " + followersCount);
        }

        private void UpdateAllBoids()
        {
            for (int r = 0; r < races.Length; ++r)
            {
                foreach (var boid in entitiesByRace[r])
                {
                    Vector3 pos = boid.transform.position;

                    boid.targetVelocity = Vector3.zero;
                    Vector3 cohesion = Vector3.zero;
                    Vector3 repulsion = Vector3.zero;
                    Vector3 imitation = Vector3.zero;
                    Vector3 follow = Vector3.zero;
                    Vector3 streaming = Vector3.zero; // courants
                    Vector3 barycenter = Vector3.zero;
                    boid.isFollowing = false;
                    int n = 0;

                    ConsiderRace(0, races[r].likes_A1, pos, ref repulsion, ref imitation, ref barycenter, ref n);
                    ConsiderRace(1, races[r].likes_A2, pos, ref repulsion, ref imitation, ref barycenter, ref n);
                    ConsiderRace(2, races[r].likes_B1, pos, ref repulsion, ref imitation, ref barycenter, ref n);
                    ConsiderRace(3, races[r].likes_B2, pos, ref repulsion, ref imitation, ref barycenter, ref n);
                    ConsiderRace(4, races[r].likes_C1, pos, ref repulsion, ref imitation, ref barycenter, ref n);
                    ConsiderRace(5, races[r].likes_C2, pos, ref repulsion, ref imitation, ref barycenter, ref n);

                    cohesion = ((barycenter / n) - pos).normalized;
                    repulsion = repulsion.normalized;
                    imitation = imitation.normalized;

                    // Repulsion by player
                    if ((player.transform.position - pos).sqrMagnitude <= repulsionRange * repulsionRange)
                    {
                        repulsion += (pos - player.transform.position);
                    }

                    // Courants
                    foreach (var point in streamPoints)
                    {
                        if ((point.transform.position - pos).sqrMagnitude < streamDetectionRange * streamDetectionRange)
                        {
                            streaming += point.Direction;
                        }
                    }

                    // Trail
                    for (int i = trailZoneManager.trails.Count - 1; i >= 0; --i)
                    {
                        TrailBoidZone zone = trailZoneManager.trails[i];
                        if ((pos - zone.transform.position).sqrMagnitude <= zone.range * zone.range)
                        {
                            follow = zone.direction;
                            boid.isFollowing = true;
                            ++followersCount;
                            break;
                        }
                    }

                    CheckLevelBorders(boid, pos);

                    // Apply to boid velocity
                    boid.targetVelocity = cohesion * cohesionForce
                                        + repulsion * repulsionForce
                                        + imitation * imitationForce
                                        + follow * followForce
                                        + streaming * streamForce;
                }
            }
        }

        private void ConsiderRace(int targetRace, bool like, Vector3 pos, ref Vector3 repulsion, ref Vector3 imitation, ref Vector3 barycenter, ref int n)
        {
            if (like)
            {
                foreach (var other in entitiesByRace[targetRace])
                {
                    UpdateBoidWithOther(pos, other, ref barycenter, ref repulsion, ref imitation, ref n);
                }
            }
            else
            {
                foreach (var other in entitiesByRace[targetRace])
                {
                    UpdateBoidWithOther(pos, other, ref repulsion);
                }
            }
        }

        //private void UpdateOtherOne()
        //{
        //    SpecialBoid boid = theOtherOne;
        //    Vector3 pos = boid.transform.position;

        //    if (boid.isFollowingPlayer)
        //    {
        //        boid.targetVelocity = Vector3.zero;
        //        Vector3 repulsion = Vector3.zero;
        //        Vector3 imitation = player.sprite.up;
        //        Vector3 cohesion = (player.transform.position - pos);

        //        foreach (var other in entities)
        //        {
        //            // Doesn't consider himself
        //            if (other == boid) continue;

        //            float squaredDistance = (other.transform.position - pos).sqrMagnitude;
        //            if (squaredDistance < detectionRange * detectionRange)
        //            {
        //                // Repulsion
        //                if (squaredDistance <= repulsionRange * repulsionRange)
        //                {
        //                    repulsion += (pos - other.transform.position);
        //                }
        //            }
        //        }

        //        repulsion = repulsion.normalized;

        //        float squaredDistance2 = (player.transform.position - pos).sqrMagnitude;
        //        if (squaredDistance2 >= detectionRange * repulsionRange)
        //        {
        //            Debug.Log("false");
        //            boid.isFollowingPlayer = false;
        //        }

        //        // Courants
        //        // TODO
                
        //        //CheckLevelBorders(boid, pos);

        //        boid.targetVelocity = cohesion * followForce
        //                            + repulsion * repulsionForce
        //                            + imitation * imitationForce;
        //    }
        //    else // is not following player
        //    {
        //        UpdateBoid(theOtherOne);

        //        float squaredDistance2 = (player.transform.position - pos).sqrMagnitude;
        //        if (squaredDistance2 <= detectionRange * repulsionRange)
        //        {
        //            Debug.Log("true");
        //            boid.isFollowingPlayer = true;
        //        }
        //    }
        //}

        private void UpdateBoidWithOther(Vector3 selfPos, BoidController other, ref Vector3 repulsion)
        {
            float squaredDistance = (other.transform.position - selfPos).sqrMagnitude;
            if (squaredDistance < detectionRange * detectionRange)
            {
                // Repulsion
                if (squaredDistance <= repulsionRange * repulsionRange)
                {
                    repulsion += (selfPos - other.transform.position);
                }
            }
        }

        private void UpdateBoidWithOther(Vector3 selfPos, BoidController other,
            ref Vector3 barycenter, ref Vector3 repulsion, ref Vector3 imitation, ref int baryN)
        {
            float squaredDistance = (other.transform.position - selfPos).sqrMagnitude;
            if (squaredDistance < detectionRange * detectionRange)
            {
                // Cohesion
                barycenter += other.transform.position;
                baryN++;

                // Imitation
                imitation += other.currentVelocity;

                // Repulsion
                if (squaredDistance <= repulsionRange * repulsionRange)
                {
                    repulsion += (selfPos - other.transform.position);
                }
            }
        }

        //private void UpdateBoid(BoidController boid)
        //{
        //    Vector3 pos = boid.transform.position;

        //    boid.targetVelocity = Vector3.zero;
        //    Vector3 cohesion = Vector3.zero;
        //    Vector3 repulsion = Vector3.zero;
        //    Vector3 imitation = Vector3.zero;
        //    Vector3 follow = Vector3.zero;
        //    Vector3 streaming = Vector3.zero; // courants
        //    Vector3 barycenter = Vector3.zero;
        //    boid.isFollowing = false;
        //    int n = 0;

        //    foreach (var other in entities)
        //    {
        //        // Doesn't consider himself
        //        if (other == boid) continue;

        //        float squaredDistance = (other.transform.position - pos).sqrMagnitude;
        //        if (squaredDistance < detectionRange * detectionRange)
        //        {
        //            // Cohesion
        //            barycenter += other.transform.position;
        //            n++;

        //            // Imitation
        //            imitation += other.currentVelocity;

        //            // Repulsion
        //            if (squaredDistance <= repulsionRange * repulsionRange)
        //            {
        //                repulsion += (pos - other.transform.position);
        //            }
        //        }
        //    }

        //    cohesion = ((barycenter / n) - pos).normalized;
        //    repulsion = repulsion.normalized;
        //    imitation = imitation.normalized;

        //    // Repulsion by player
        //    if ((player.transform.position - pos).sqrMagnitude <= repulsionRange * repulsionRange)
        //    {
        //        repulsion += (pos - player.transform.position);
        //    }

        //    // Courants
        //    foreach (var point in streamPoints)
        //    {
        //        if ((point.transform.position - pos).sqrMagnitude < detectionRange * repulsionRange)
        //        {
        //            streaming += point.Direction;
        //        }
        //    }
            
        //    // Trail
        //    for (int i = trailZoneManager.trails.Count - 1; i >= 0; --i)
        //    {
        //        TrailBoidZone zone = trailZoneManager.trails[i];
        //        if ((pos - zone.transform.position).sqrMagnitude <= zone.range * zone.range)
        //        {
        //            follow = zone.direction;
        //            boid.isFollowing = true;
        //            ++followersCount;
        //            break;
        //        }
        //    }
            
        //    CheckLevelBorders(boid, pos);

        //    // Apply to boid velocity
        //    boid.targetVelocity = cohesion * cohesionForce
        //                        + repulsion * repulsionForce
        //                        + imitation * imitationForce
        //                        + follow * followForce
        //                        + streaming * streamForce;

        //}
        
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