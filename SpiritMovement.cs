using UnityEngine;
using System.Collections;

public class SpiritMovement : MonoBehaviour 
{
	public float speed;

	Transform player;

	bool hasCalledRotation;

	void Start()
	{
		player = GameObject.FindGameObjectWithTag("Player").transform;
		transform.localScale = Vector3.zero;
	}

	void Update () 
	{
		//inflate the soul to it's proper size
		if(transform.localScale.x <= 1)
		{
			float scaleSpeed = 1f * Time.deltaTime;
			transform.localScale += new Vector3(scaleSpeed, scaleSpeed, scaleSpeed);
		}

		//MOVE TOWARDS PLAYER
		if(player == null) 
		{
			return;
			//go off in a random direction
			//transform.Translate( Vector3.up * speed * Time.deltaTime, Space.Self );
		}
		else
		{
			if(gameObject.tag == "Good Spirit")
			{
				//try to escape
				if(hasCalledRotation == false)
				{
					hasCalledRotation = true;
					StartCoroutine("GetRotation");
				}
				transform.Translate( Vector3.up * speed * Time.deltaTime, Space.Self );
			}
			if(gameObject.tag == "Evil Spirit")
			{
				//target player
				transform.position = Vector2.MoveTowards(transform.position, player.position, (speed * Time.deltaTime) );
			}

		}
	}

	IEnumerator GetRotation()
	{
		int randomRot = Random.Range(0,359);

		transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, randomRot);

		yield return new WaitForSeconds(Random.Range(1f, 3f));

		StartCoroutine("GetRotation");
	}
}
