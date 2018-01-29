using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StreamSkin : MonoBehaviour {

    public LineRenderer lineRenderer;
    public StreamPoint[] streamPoints;

    // Use this for initialization
    void Start()
    {
        streamPoints = this.gameObject.GetComponentsInChildren<StreamPoint>();
        lineRenderer = this.GetComponent<LineRenderer>();

        lineRenderer.positionCount = streamPoints.Length;

        for (int i = 0; i< lineRenderer.positionCount; i++)
        {
            lineRenderer.SetPosition(i, streamPoints[i].transform.position);
        }
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}
