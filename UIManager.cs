using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIManager : MonoBehaviour 
{
	public Text soulsCollectedText;
	public GameObject gameOverPanel;
	public Text gameOverStatsText;
	public Text bestScoreText;
	public Text waveText;

	public Slider healthSlider;
	public Image healthIcon;

	public AudioSource waveSound;

	PlayerDamage playerDamage; 

	void Awake () 
	{
		soulsCollectedText.text = "SOULS COLLECTED: \n" + "- "+StatsManager.soulsCollected.ToString() + " -";

		gameOverPanel.SetActive(false);
		bestScoreText.text = "( BEST: " + StatsManager.bestPreviousScore.ToString() + " )";
	}

	void Start()
	{
		playerDamage = FindObjectOfType<PlayerDamage>();
		waveText.enabled = false;
		healthSlider.value = healthSlider.maxValue;
	}

	void Update()
	{
		//update souls collected count
		soulsCollectedText.text = "SOULS COLLECTED: \n" + "- "+StatsManager.soulsCollected.ToString() + " -";

		//update player health
		healthSlider.value = playerDamage.playerHealth;
		//if the players health is 0
		if(healthSlider.value == healthSlider.minValue)
		{
			//rotate x to death mode	
			healthIcon.rectTransform.rotation = Quaternion.Euler(healthIcon.rectTransform.rotation.x, healthIcon.rectTransform.rotation.y, healthIcon.rectTransform.rotation.z - 45f);
		}
	}

	public void TriggerGameOverUI()
	{
		gameOverPanel.SetActive(true);

		waveText.enabled = false;

		healthIcon.enabled = false;
		healthSlider.enabled = false;

		soulsCollectedText.enabled = false;
		bestScoreText.enabled = false;

		//SETUP ACTUAL GAME OVER STATS

		gameOverStatsText.text = 
			"WAVES SURVIVED:     " + StatsManager.waveNumber.ToString() +"\n"+
			"SOULS ENCOUNTERED:     " + StatsManager.soulsEncountered.ToString() +"\n"+
			"SOULS COLLECTED:     " + StatsManager.soulsCollected.ToString() +"\n"+ 
			"EVIL SOULS DESTROYED:     " + StatsManager.evilSoulsDestroyed.ToString() +"\n"+ 
			"BATHROOM BREAKS:     " + StatsManager.bathroomBreaks.ToString() +"\n"+
			"CACTUSES PRICKLED:     " + StatsManager.cactusesPrickled.ToString();
	}

	public void DisableBestScore()
	{
		bestScoreText.enabled = false;
		Debug.Log("disabling best score");
	}

	public void EnableBestScore(int value)
	{
		bestScoreText.enabled = true;
		bestScoreText.text = "( BEST: " + value.ToString() + " )";
	}

	public void IncrementWaveUI(int waveNumber)
	{
		waveText.text = "WAVE: " + waveNumber.ToString();
		waveText.enabled = true;
		waveSound.Play();
		Invoke("DisableWaveText", 3f);
	}

	void DisableWaveText()
	{
		waveText.enabled = false;
	}
}