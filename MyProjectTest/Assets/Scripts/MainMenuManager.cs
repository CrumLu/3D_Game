using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
	public void StartGame()
	{
		SceneManager.LoadScene("Level01");
	}

	public void OpenInstructions()
	{
		SceneManager.LoadScene("Instructions");
	}

	public void BackToMenu() 
	{
		SceneManager.LoadScene("MainMenu");
	}
}
