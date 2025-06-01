using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
	public void StartGame()
	{
		//GameManager.instance.IniciarNovaPartida();
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

	public void GoTOCredits()
    {
        SceneManager.LoadScene("Credits");
    }

	public void QuitGame()
    {
        // Si s'està executant en l'editor, només atura la reproducció
		#if UNITY_EDITOR
					UnityEditor.EditorApplication.isPlaying = false;
		#else
			Application.Quit();
		#endif
    }
}
