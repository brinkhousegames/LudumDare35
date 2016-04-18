using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpiritSpawner : MonoBehaviour 
{
	public GameObject goodSpiritPrefab;
	public GameObject evilSpiritPrefab;
	public List<GameObject> spirits;
	public Transform[] spawnPoints;
	public int spiritsPerWave = 5;
	public int additionalSpiritsEachWave = 5;
	public float minTimeBetweenSpawns;
	public float maxTimeBetweenSpawns;
	public float startWaveDelay = 2f;
	public float timeBetweenWaves = 3f;
	public AudioSource spiritSound;

	void Start () 
	{
		
	}

	void Update () 
	{
	
	}

	public IEnumerator Spawn()
	{
		yield return new WaitForSeconds(startWaveDelay);

		int spiritsSpawned = 0;
		while ( spiritsSpawned < spiritsPerWave )
		{
			GameObject spirit = spirits[Random.Range(0, spirits.Count)];

			Instantiate(spirit, spawnPoints[Random.Range(0, spawnPoints.Length)].position, Quaternion.identity );
			StatsManager.soulsEncountered++;
			spiritsSpawned ++;
			spiritSound.Play();
			Debug.Log("Spirits Spawned: " + spiritsSpawned);

			float ranSpawnTime = Random.Range(minTimeBetweenSpawns, maxTimeBetweenSpawns);
			yield return new WaitForSeconds(ranSpawnTime);
		}

		if(spiritsSpawned == spiritsPerWave)
		{
			Debug.Log("all spirits spawned this wave");
			StatsManager.IncrementWave();

			//add more spirits each wave
			spiritsPerWave += additionalSpiritsEachWave;

			//randomly decide if we should add a good spirit to the array
			int ranChoose = Random.Range(0, 1);
			if(ranChoose == 1)
				spirits.Add(goodSpiritPrefab);

			//every third wave, add in an additional evil spirit to the array
			if(StatsManager.waveNumber % 3 == 0) 
			{
				Debug.Log("added evil spirit to List");
				spirits.Add(evilSpiritPrefab);

				//slightly decrease the time between spawns (getting progressively faster)
				minTimeBetweenSpawns -= .2f;
				maxTimeBetweenSpawns -= .2f;
			}
		}
	}
}
