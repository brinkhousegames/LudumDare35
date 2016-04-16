using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BalloonController : MonoBehaviour 
{
	public float liftAmount = 5f; //vertical lift
	public float moveSpeed = 5f;
	public float rotateSpeed = 2f;
	public float maxTimeAtEdge; //how long player can be at top or bottom of screen without crashing
	public float centerScreenPos; //the position where the balloon flies on screen during the opening dialogue
	private float origTime; 
	private int origTextSize;

	public Text highAltitudeText;
	public Text lowAltitudeText;
	public AudioSource flameSound;
	public AudioSource bonusSound;
	public AudioSource BirdSound;
	public ParticleSystem flame; //flame particles
	public GameObject balloonBasket; //our basket gameobject
	public Slider fuelLevel; //our fuel slider UI
	public Slider altitudeBar;
	public float fuelBonus; //how much fuel do we earn from picking up fuel?
	public float fuelLossRate; //how quickly we lose fuel

	Rigidbody thisRigidbody;
	GameManager gameManager;
	ObstacleSpawner spawner;

	public bool useFuel; //for testing purposes, dont use fuel
	public bool playerCanControl; //have we handed the control to the player?
	public bool setHeight; //after balloon enters screen, set our altitude max height correctly. this ensures we dont crash early
	bool playingSound; //flame sound checking
	bool crashed; //did we crash by hitting something or running out of fuel?
	
	private bool lowFuel; //returns true when we've reached low fuel

	// Use this for initialization
	void Start () 
	{
		spawner = GameObject.FindObjectOfType<ObstacleSpawner>();
		gameManager = GameObject.FindObjectOfType<GameManager>();
		//start with basket rigidbody kinematic 
		balloonBasket.GetComponent<Rigidbody>().isKinematic = true;
		thisRigidbody = GetComponent<Rigidbody>();

		origTime = maxTimeAtEdge;
		origTextSize = highAltitudeText.fontSize; //cache our original text size
	}

	void Update()
	{
		//at very beginning, our max altitude is higher as balloon enters. after 3 seconds we allow it to normalize
		if(PlayerPrefs.HasKey("Shown Dialogue") && playerCanControl) //set as long as we aren't watching dialogue
			Invoke ("SetAltitude", 4f);
	}

	void SetAltitude()
	{
		setHeight = true;
	}

	// Update is called once per frame
	void FixedUpdate () 
	{
		//if we spend too long at either max or min height, we crash!
		if(setHeight && transform.position.y >= altitudeBar.maxValue || transform.position.y <= altitudeBar.minValue && !crashed)
		{
			//start crash countdown timer
			maxTimeAtEdge -= Time.deltaTime;

			//for each second, flash warning on appropriate altitude text
			if(altitudeBar.value == altitudeBar.maxValue)
			{
				int seconds = (int)maxTimeAtEdge + 1;
				highAltitudeText.text = "Descend! \n" + seconds.ToString();
			}
		
			if(altitudeBar.value == altitudeBar.minValue)
			{
				int seconds = (int)maxTimeAtEdge + 1;
				lowAltitudeText.text = "Ascend! \n" + seconds.ToString();
			}

			//if we run out of time, crash
			if(maxTimeAtEdge <= 0)
			{
				crashed = true;
				gameManager.GameOver("Crashed!");
			}
		}
		else //if we are no longer at the edge, reset the timer and our texts
		{
			maxTimeAtEdge = origTime;

			highAltitudeText.text = null;
			lowAltitudeText.text = null;
		}
			
		//match our altitude gauge with the balloons height
		altitudeBar.value = transform.position.y;

		//cap our balloon from exiting the top of screen
		if(transform.position.y >= altitudeBar.maxValue && setHeight)
		{
			transform.position = new Vector3(transform.position.x, altitudeBar.maxValue, transform.position.z);
		}

		//slight spin to balloon
		transform.Rotate(Vector3.up, -rotateSpeed * Time.deltaTime);

		//during the opening dialogue, fly at center screen
		if(!playerCanControl && transform.position.y <= centerScreenPos)
		{
			Vector3 pos = transform.position;
			pos.y = Mathf.Lerp (pos.y, centerScreenPos, 2f * Time.deltaTime);
			transform.position = pos;
		}

		//after the opening dialogue has played, give player control
		if(playerCanControl)
		{
			if(!PlayerPrefs.HasKey("Shown Dialogue"))
				setHeight = true;

			//if mouse down and we have fuel and haven't crashed, add lift
			if(Input.GetMouseButton(0) && fuelLevel.value > 0 && !crashed)
			{
				if(playingSound)
				{
					//continue
				}
				else
				{
					flameSound.Play();
					playingSound = true;
				}

				//check if we are low on fuel, if so spawn one
				if(fuelLevel.value < 20 && !lowFuel)
				{
					ObstacleSpawner spawner = GameObject.FindObjectOfType<ObstacleSpawner>();
					spawner.SpawnFuel();
					lowFuel = true;
					print ("spawned fuel");
				}
				if(fuelLevel.value >= 60)
					lowFuel = false;

				//add lift
				thisRigidbody.AddForce(0, liftAmount * Time.deltaTime, 0, ForceMode.Impulse);
				//decrement fuel level while lifting
				if(useFuel)
					fuelLevel.value -= fuelLossRate * Time.deltaTime;
				//enable the flame particle during lift
				flame.enableEmission = true;
			}
			else //if we stopped adding lift...
			{
				flame.enableEmission = false;
				flameSound.Stop ();
				playingSound = false;

				if(fuelLevel.value <= 0) //if we ran out of fuel, we crash
				{
					crashed = true;
					gameManager.GameOver("Ran Out of Fuel!");
				}
			}

			//left/right movement NOTE: Should this be omited?
			if(!crashed)
			{
				if(Input.GetAxis("Horizontal") > 0) //right
				{
					thisRigidbody.AddForce(moveSpeed * Time.deltaTime, 0, 0, ForceMode.Force);
				}
				if(Input.GetAxis("Horizontal") < 0) //left
				{
					thisRigidbody.AddForce(-moveSpeed * Time.deltaTime, 0, 0, ForceMode.Force);
				}
			}

			if(crashed) //if we crashed, add a tumble to the balloon as it drops
			{
				thisRigidbody.AddTorque(-Vector3.forward * -.3f * Time.deltaTime);
				thisRigidbody.AddRelativeTorque(Vector3.right * .3f * Time.deltaTime);
			}
	}
	}
	//Trigger detection

	void OnTriggerEnter(Collider other)
	{
		if(!gameManager.gameOver) //if we haven't caused game over, continue checking against collisions
		{
			switch(other.tag)
			{
			case "Bird":
				BirdSound.Play();
				Destroy(other.gameObject);
				crashed = true;
				gameManager.GameOver("Bird Strike!");
				break;
				
			case "Fuel":
				bonusSound.Play();
				fuelLevel.value += fuelBonus;
				Destroy (other.gameObject, .2f);
				gameManager.FuelBonus("Fuel Added!");
				spawner.fuel++;
				break;

			case "Biplane":
				crashed = true;
				//detach the basket from the balloon
				balloonBasket.transform.parent = null;
				//reenable collider and rigidbody now that the colliders aren't overlapping
				balloonBasket.GetComponent<CapsuleCollider>().enabled = true;
				balloonBasket.GetComponent<Rigidbody>().isKinematic = false;
				gameManager.GameOver("Bill Bransen wins again.");
				break;
			}
		}
	}

	//Collision Detection

	void OnCollisionEnter(Collision other)
	{
		if(!gameManager.gameOver) //if we haven't caused game over, continue checking against collisions
		{
			switch(other.collider.tag)
			{
			case "Building":
				crashed = true;
				//detach the basket from the balloon
				balloonBasket.transform.parent = null;
				//reenable collider and rigidbody now that the colliders aren't overlapping
				balloonBasket.GetComponent<CapsuleCollider>().enabled = true;
				balloonBasket.GetComponent<Rigidbody>().isKinematic = false;
				gameManager.GameOver("That's a building.");
				break;
			}
		}
	}
}
