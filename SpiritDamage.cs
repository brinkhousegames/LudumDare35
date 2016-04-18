using UnityEngine;
using System.Collections;

public class SpiritDamage : MonoBehaviour 
{
	public float health = 50; //TODO: Maybe this can become modular and get higher as time goes on?
	public GameObject deathParticles;

	Weapon weapon;

	void Start()
	{
		weapon = FindObjectOfType<Weapon>();
	}

	void OnParticleCollision(GameObject other) 
	{
		//do damage to the soul
		health -= weapon.laserGunDamage;
	}

	void Update()
	{
		if(health <= 0 && this.tag == "Good Spirit")
		{
			Instantiate(deathParticles, transform.position, Quaternion.identity);
			StatsManager.goodSoulsDestroyed++;
			Weapon weapon = FindObjectOfType<Weapon>();
			weapon.KilledGoodSoul();
			//penalize the player for killing a good soul
			if(StatsManager.soulsCollected > 0)
				StatsManager.soulsCollected--;
			
			Destroy(gameObject);

			//TODO: Lose points/souls, somehow this needs to be penalized
		}
		if(health <= 0 && this.tag == "Evil Spirit")
		{
			Instantiate(deathParticles, transform.position, Quaternion.identity);
			StatsManager.evilSoulsDestroyed++;
			Weapon weapon = FindObjectOfType<Weapon>();
			weapon.KilledEvilSoul();
			Destroy(gameObject);
		}
	}

//	void OnDisable()
//	{
//		if(this.tag == "Good Spirit")
//		{
//			//trigger the particles when good souls get sucked up
//			Instantiate(deathParticles, transform.position, Quaternion.identity);
//		}
//	}
}
