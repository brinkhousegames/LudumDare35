using UnityEngine;
using System.Collections;

public class StatsManager : MonoBehaviour 
{
	public static int soulsEncountered;
	public static int soulsCollected;
	public static int goodSoulsDestroyed;
	public static int evilSoulsDestroyed;
	public static int bathroomBreaks;
	public static int cactusesPrickled;

	public static int bestPreviousScore;

	public static int waveNumber;

	UIManager uiMan;

	void OnEnable()
	{
		soulsCollected = 0;
		goodSoulsDestroyed = 0;
		evilSoulsDestroyed = 0;
		bathroomBreaks = 0;
		cactusesPrickled = 0;
		waveNumber = 0;
	}
	void Disable()
	{
		soulsCollected = 0;
		goodSoulsDestroyed = 0;
		evilSoulsDestroyed = 0;
		bathroomBreaks = 0;
		cactusesPrickled = 0;
	}

	void Start()
	{
		uiMan = FindObjectOfType<UIManager>();


		if(PlayerPrefs.HasKey("HighScore") == false)
		{
			PlayerPrefs.SetInt("HighScore", 0);
			uiMan.DisableBestScore();
			Debug.Log("StatsManager -- Disabling best score");
		}
		
		if(PlayerPrefs.GetInt("HighScore") == 0)
		{
			
		}
		else
		{
			
			uiMan.EnableBestScore(PlayerPrefs.GetInt("HighScore"));
		}

		//show initial wave UI when we start
		if(waveNumber == 0)
		{
			Invoke("IncrementWaveNonStatic", 2f);
		}
	}

	public static void AddSoul(int quantity)
	{
		soulsCollected += quantity;
	}

	public static void CheckForHighScore()
	{
		if(soulsCollected > bestPreviousScore)
		{
			bestPreviousScore = soulsCollected;
			PlayerPrefs.SetInt("HighScore", bestPreviousScore);
		}
	}

	void IncrementWaveNonStatic()
	{
		PlayerDamage player = FindObjectOfType<PlayerDamage>();
		if(player.playerHealth > 0)
		{
			IncrementWave();
		}
	}

	public static void IncrementWave()
	{
		waveNumber++;
		UIManager uiMan = FindObjectOfType<UIManager>();
		uiMan.IncrementWaveUI(waveNumber);

		//start spawning spirits
		SpiritSpawner spawner = FindObjectOfType<SpiritSpawner>();
		spawner.StartCoroutine("Spawn");
	}
}


