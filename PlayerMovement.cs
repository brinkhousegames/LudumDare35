using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour 
{
	public float moveSpeed;
	public AudioSource runningSound;
	//public float rotateSpeed;

	float horInput;
	float vertInput;
	float mouseRot;

	Rigidbody2D rb;
	PlayerDamage damage;

	bool runningSoundPlaying;
	bool areRunning;

	void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		damage = FindObjectOfType<PlayerDamage>();
	}

	void Update()
	{
		//if we aren't dead... 
		if(damage.playerHealth > 0 && Time.timeScale != 0)
		{
			//CACHE MOVEMENT INPUT
			horInput = Input.GetAxis("Horizontal");
			vertInput = Input.GetAxis("Vertical");

			if(!areRunning)
			{
				if(	horInput != 0 || vertInput != 0 )
				{
					areRunning = true;
					print("we are running!");
					StartCoroutine("RunSound");
				}
			}
			if(areRunning)
			{
				if(horInput == 0 && vertInput == 0 )
				{
					areRunning = false;
					print("stopped running!");
					StopCoroutine("RunSound");
				}
			}

			//ROTATION FROM MOUSE
			RotateToMouse();
		}
		else
		{
			horInput = 0;
			vertInput = 0;
		}
	
	}

	IEnumerator RunSound()
	{
		runningSoundPlaying = true;
		runningSound.Play();
		print("footstep");
		yield return new WaitForSeconds(.4f);
		StartCoroutine("RunSound");
	}

	void FixedUpdate()
	{
		//PHYSICS MOVEMENT 
		rb.AddForce( new Vector2(horInput * moveSpeed, vertInput * moveSpeed) );
	}

	void RotateToMouse()
	{
		Vector3 mousePos = Input.mousePosition;
		mousePos.z = 0f;

		Vector3 objectPos = Camera.main.WorldToScreenPoint (transform.position);
		mousePos.x = mousePos.x - objectPos.x;
		mousePos.y = mousePos.y - objectPos.y;

		float angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
	}

}
