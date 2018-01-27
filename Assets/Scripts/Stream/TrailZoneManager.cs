using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlackBalls
{
    public class TrailZoneManager : MonoBehaviour
    {
        public List<TrailBoidZone> trails = new List<TrailBoidZone>();

        private void Update()
        {
            for (int i = trails.Count - 1; i >= 0; --i)
            {
                trails[i].force -= Time.deltaTime;

                if (trails[i].force <= 0)
                {
                    TrailBoidZone t = trails[i];
                    trails.RemoveAt(i);
                    Destroy(t.gameObject);
                }
                else
                {
                    trails[i].transform.localScale += Vector3.one * 0.01f;
                    trails[i].range += 0.01f;
                }
            }
        }
    }
}