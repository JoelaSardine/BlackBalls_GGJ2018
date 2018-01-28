using System;
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
        public int minFollowerCount;

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
        public float minStreamForce = 0.2f;
        public float maxStreamForce = 1f;

        //public LimitBorder[] limitBorder;

        private List<List<BoidController>> entitiesByRace = new List<List<BoidController>>();
        private List<StreamPoint> streamPoints;
        private float currentStreamForce;

        void Start()
        {
            AkSoundEngine.SetRTPCValue("density", 0, gameObject);
            AkSoundEngine.PostEvent("Play_musBlend", gameObject);

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
                    x = UnityEngine.Random.Range(-entitySpawnSurface.x / 2, entitySpawnSurface.x / 2);
                    y = UnityEngine.Random.Range(-entitySpawnSurface.y / 2, entitySpawnSurface.y / 2);
                    BoidController boid = Instantiate(races[r].prefab, new Vector3(x, y, 0), Quaternion.identity, ancester).GetComponent<BoidController>();
                    racepopulation.Add(boid);
                }
                entitiesByRace.Add(racepopulation);
            }
        }

        void Update()
        {
            currentStreamForce = minStreamForce + (maxStreamForce - minStreamForce) / Mathf.Max(20 - followersCount, 1);
            followersCount = 0;

            UpdateAllBoids();
            UpdateOtherOne();
            //UpdatePlayer();

            if (followersCount > minFollowerCount)
            {
                minFollowerCount = Mathf.Min(13, followersCount);
            }
            //Debug.Log("RTPC value : " + Mathf.Max((followersCount) * 2, minFollowerCount));
            AkSoundEngine.SetRTPCValue("density", Mathf.Max((followersCount) * 2, minFollowerCount), gameObject);
            //Debug.Log("Followers = " + followersCount);
        }

        private void UpdatePlayer()
        {
            Vector3 pos = player.transform.position;
            Vector3 streaming = Vector3.zero; // courants

            // Courants
            foreach (var point in streamPoints)
            {
                if ((point.transform.position - pos).sqrMagnitude < streamDetectionRange * streamDetectionRange)
                {
                    streaming += point.Direction;
                }

                player.residualStreamVelocity = streaming;
            }
        }

        private void UpdateAllBoids()
        {
            for (int r = 0; r < races.Length; ++r)
            {
                foreach (var boid in entitiesByRace[r])
                {
                    UpdateOneBoid(boid, 
                        races[r].likes_A1, races[r].likes_A2, 
                        races[r].likes_B1, races[r].likes_B2, 
                        races[r].likes_C1, races[r].likes_C2);
                }
            }
        }

        private void UpdateOneBoid(BoidController boid, 
            bool likesA1, bool likesA2, bool likesB1, bool likesB2, bool likesC1, bool likesC2)
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

            ConsiderRace(0, likesA1, pos, ref repulsion, ref imitation, ref barycenter, ref n);
            ConsiderRace(1, likesA2, pos, ref repulsion, ref imitation, ref barycenter, ref n);
            ConsiderRace(2, likesB1, pos, ref repulsion, ref imitation, ref barycenter, ref n);
            ConsiderRace(3, likesB2, pos, ref repulsion, ref imitation, ref barycenter, ref n);
            ConsiderRace(4, likesC1, pos, ref repulsion, ref imitation, ref barycenter, ref n);
            ConsiderRace(5, likesC2, pos, ref repulsion, ref imitation, ref barycenter, ref n);

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
                                + streaming * currentStreamForce;
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

        private void UpdateOtherOne()
        {
            Vector3 pos = theOtherOne.transform.position;

            if (theOtherOne.isFollowingPlayer)
            {
                theOtherOne.targetVelocity = Vector3.zero;
                Vector3 cohesion = Vector3.zero;
                Vector3 repulsion = Vector3.zero;
                Vector3 imitation = Vector3.zero;
                Vector3 streaming = Vector3.zero; // courants
                Vector3 barycenter = Vector3.zero;
                theOtherOne.isFollowing = false;
                int n = 0;

                cohesion = (player.transform.position - pos).normalized;
                imitation = player.velocity.normalized;

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

                CheckLevelBorders(theOtherOne, pos);

                // Apply to boid velocity
                theOtherOne.targetVelocity = cohesion * followForce
                                    + repulsion * repulsionForce
                                    + imitation * imitationForce
                                    //+ follow * followForce
                                    + streaming * currentStreamForce * 10;
            }
            else // is not following player
            {
                UpdateOneBoid(theOtherOne, true, true, true, true, true, true);
            }
        }

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