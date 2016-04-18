using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour 
{
	public GameObject laserGun;
	public float laserGunDamage = 5f;
	public AudioSource laserGunSound;
	public AudioSource vaccuumGunSound;
	public AudioSource soulCollectedSound;
	public AudioSource whoopsSound;
	public AudioSource evilSpiritDeathSound;

	public bool canFire = true;

	Transform enemy = null;

	bool isFiring; 

	Collider2D vaccuumTrigger;
	ParticleSystem vaccuumParticles;

	PlayerDamage damage;

	void Start () 
	{
		vaccuumTrigger = GetComponent<Collider2D>();
		vaccuumParticles = GetComponentInChildren<ParticleSystem>();
		damage = FindObjectOfType<PlayerDamage>();

		DeactivateVaccuum();
		laserGun.SetActive(false);
	}

	void Update () 
	{
		if(damage.playerHealth > 0 && canFire)
		{
			//listen for player input
			if(Input.GetButtonDown("Vaccuum"))
			{
				InitiateVaccuum();
				DeactivateLaser();
			}
			if(Input.GetButtonUp("Vaccuum"))
			{
				DeactivateVaccuum();
			}

			if(Input.GetButtonDown("Laser"))
			{
				InitiateLaser();
				DeactivateVaccuum();
			}
			if(Input.GetButtonUp("Laser"))
			{
				DeactivateLaser();
			}
		}
		else
		{
			DeactivateVaccuum();
			DeactivateLaser();
		}
	}

	void InitiateVaccuum()
	{
		//enable vaccuum
		isFiring = true;
		vaccuumTrigger.enabled = true;
		ParticleSystem.EmissionModule emissions = vaccuumParticles.emission;
		emissions.enabled = true;
		vaccuumGunSound.Play();
	}

	void DeactivateVaccuum()
	{
		//disable vaccuum
		isFiring = false;
		vaccuumTrigger.enabled = false;
		ParticleSystem.EmissionModule emissions = vaccuumParticles.emission;
		emissions.enabled = false;
		CancelInvoke();
		vaccuumGunSound.Stop();
	}

	void InitiateLaser()
	{
		//enable laser
		isFiring = true;
		laserGun.SetActive(true);

		StartCoroutine("PlayLaser");

	}

	IEnumerator PlayLaser()
	{
		while(isFiring = true)
		{
			laserGunSound.Play();
			yield return new WaitForSeconds(.10f);
		}
	}

	void DeactivateLaser()
	{
		//disable laser
		isFiring = false;
		laserGun.SetActive(false);
		PlayLaser();

		StopCoroutine("PlayLaser");
		//CancelInvoke();
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if( other.tag == "Good Spirit")
		{
			enemy = other.transform;
			InvokeRepeating("AbsorbSoul", 0f, .01f);
		}
	}

	void AbsorbSoul()
	{
		if(enemy.transform.localScale.x >= 0)
		{
			float scaleSpeed = 1f * Time.deltaTime;
			enemy.transform.localScale -= new Vector3(scaleSpeed, scaleSpeed, scaleSpeed);
			enemy.transform.position = Vector2.MoveTowards(enemy.transform.position, transform.position, (1f * Time.deltaTime) );
		}
		else
		{
			CancelInvoke();
			Destroy(enemy.gameObject);
			DeactivateVaccuum();
			StatsManager.soulsCollected++;
			soulCollectedSound.Play();
		}
	}

	public void KilledGoodSoul()
	{
		whoopsSound.Play();
	}

	public void KilledEvilSoul()
	{
		print("killed spirit");
		evilSpiritDeathSound.Play();
	}
}
