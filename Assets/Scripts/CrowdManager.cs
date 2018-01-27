using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlackBalls
{
    public class CrowdManager : MonoBehaviour
    {
        public GameObject entityPrefab;
        public int entityCount;
        public Vector2 entitySpawnSurface;

        public LimitBorder[] limitBorder;

        private 

        void Start()
        {
            foreach (var border in limitBorder)
            {
                border.OnEntityEnter += OnEntityIsDestroyed;
            }

            float x, y;
            for (int i = 0; i < entityCount; i++)
            {
                x = Random.Range(-entitySpawnSurface.x / 2, entitySpawnSurface.x / 2);
                y = Random.Range(-entitySpawnSurface.y / 2, entitySpawnSurface.y / 2);
                Instantiate(entityPrefab, new Vector3(x, y, 0), Quaternion.identity, transform);
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnEntityIsDestroyed()
        {
            float x = Random.Range(-entitySpawnSurface.x / 2, entitySpawnSurface.x / 2);
            float y = Random.Range(-entitySpawnSurface.y / 2, entitySpawnSurface.y / 2);
            Instantiate(entityPrefab, new Vector3(x, y, 0), Quaternion.identity, transform);
        }
    }
}