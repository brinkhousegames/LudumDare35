using UnityEngine;
using System.Collections;

public class PlayerShadow : MonoBehaviour 
{
	Transform player;

	void Start()
	{
		player = GameObject.FindGameObjectWithTag("Player").transform;
	}

	void Update () 
	{
		transform.position = player.position;
	}
}
