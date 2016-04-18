using UnityEngine;
using System.Collections;

public class CameraTrack : MonoBehaviour 
{
	public Transform target;
	public float yOffset;
	public float xOffset;

	void Update()
	{
		TargetPlayer();
	}

	void TargetPlayer()
	{
		Vector3 adjustedPosition = new Vector3( target.position.x - xOffset, target.position.y - yOffset, transform.position.z ); 
		transform.position = adjustedPosition; 
	}

}
