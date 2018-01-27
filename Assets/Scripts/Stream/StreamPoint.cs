using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StreamPoint : MonoBehaviour {

	public Vector3 Direction;

	public void OnDrawGizmos() {
		Gizmos.color = Color.blue;
		Vector3 halfDirection = Direction / 2f;
		Vector3 perpendicularQuarterDirection = new Vector3(-halfDirection.y, halfDirection.x) / 2f;
		Gizmos.DrawLine (transform.position - halfDirection, transform.position + halfDirection);
		Gizmos.DrawLine (transform.position + halfDirection, transform.position + perpendicularQuarterDirection);
		Gizmos.DrawLine (transform.position + halfDirection, transform.position - perpendicularQuarterDirection);
	}
}
