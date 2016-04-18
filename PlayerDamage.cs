using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;
using UnityEngine.UI;

public class PlayerDamage : MonoBehaviour 
{
	public float playerHealth = 100;
	public float damagePerSecond = 5f;
	public float bathroomHealthBuff = 20;
	public Slider bathroomBuffSlider;
	public AudioSource ouchSound;
	public AudioSource deathSound;
	public AudioSource healthBuffSound;

	bool takingDamage;

	bool isCactusCooled = true;
	bool isBathroomCooled = true;

	VignetteAndChromaticAberration vignette;

	void Start()
	{
		vignette = FindObjectOfType<VignetteAndChromaticAberration>();
	}

	//check if an evil spirit has made contact with the collider on this object
	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.tag == "Evil Spirit" && !takingDamage) 
		{
			//start draining soul
			takingDamage = true;
			//StartCoroutine("DecreaseHealth");
			InvokeRepeating("DecreaseHealth", 0f, .6f);
		}

		//PLAYER CAN HEAL IN THE BATHROOM
		if(other.tag == "Bathroom" && isBathroomCooled && playerHealth < 100)
		{
			bathroomBuffSlider.value = bathroomBuffSlider.minValue;
			playerHealth += bathroomHealthBuff;
			healthBuffSound.Play();

			vignette.chromaticAberration -= bathroomHealthBuff;
			//if we happen to go negative, stay at zero
			if(vignette.chromaticAberration < 0)
				vignette.chromaticAberration = 0;

			//player cant shoot while in bathroom
			Weapon weapon = FindObjectOfType<Weapon>();
			weapon.canFire = false;
		}
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if(other.tag == "Evil Spirit")
		{
			//stop draining soul
			takingDamage = false;
			//StopCoroutine("DecreaseHealth");
			CancelInvoke();
			//StopCoroutine("Blackout");
		}

		//PLAYER CAN HEAL IN THE BATHROOM
		if(other.tag == "Bathroom")
		{
			StartCoroutine("CoolBathroom");

			StatsManager.bathroomBreaks++;

			//return players ability to fire
			Weapon weapon = FindObjectOfType<Weapon>();
			weapon.canFire = true;
		}
	}

	void OnCollisionExit2D(Collision2D other)
	{
		if(other.collider.tag == "Cactus" && isCactusCooled)
		{
			StatsManager.cactusesPrickled++;
			Invoke("Cooldown", 3f);
			playerHealth -= 5f;
			ouchSound.Play();
		}
	}

	IEnumerator CoolBathroom()
	{
		isBathroomCooled = false;
		//slowly start filling the buff
		if(bathroomBuffSlider.value < bathroomBuffSlider.maxValue)
		{
			bathroomBuffSlider.value += .01f;
			yield return new WaitForSeconds(.1f);
			StartCoroutine("CoolBathroom");
		}
		else //we are cooled down, stop routine and set flag
		{
			StopCoroutine("CoolBathroom");
			isBathroomCooled = true;
		}

	}

	void Cooldown()
	{
		isCactusCooled = true;
	}

	void DecreaseHealth()
	{
		UIManager uiMan = FindObjectOfType<UIManager>();

		playerHealth -= damagePerSecond;
		if(playerHealth > 10) //play ouch while we arent dead
			ouchSound.Play();

		Blackout();

		//we are dead
		if(playerHealth <= 0)
		{
			deathSound.Play();
			Rigidbody2D rb = FindObjectOfType<Rigidbody2D>();
			rb.isKinematic = true;

			StatsManager.CheckForHighScore();
			uiMan.TriggerGameOverUI();
			CancelInvoke();
		}
	}

	void Blackout()
	{
		if(vignette.chromaticAberration < 100)
		{
			vignette.chromaticAberration += damagePerSecond;
		}
	}
}
