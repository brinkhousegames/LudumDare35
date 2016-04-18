using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameState : MonoBehaviour 
{

	
	// Update is called once per frame
	void Update () 
	{
		//PAUSE
		if(Input.GetKeyDown(KeyCode.P))
		{
			if(Time.timeScale == 1)
				Time.timeScale = 0;
			else
				Time.timeScale = 1;
		}

		//RESTART
		if(Input.GetKeyDown(KeyCode.R))
		{
			Replay();
		}
	}

	public void PlayLevel(int level)
	{
		SceneManager.LoadScene(level);
	}

	public void Replay()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex );
	}

	public void Quit()
	{
		Debug.Log("Quit");
		Application.Quit();
	}

	public void LoadMenu()
	{
		SceneManager.LoadScene(0);
	}
}
