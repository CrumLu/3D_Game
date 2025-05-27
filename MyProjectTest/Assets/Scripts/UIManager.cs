using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
	public TextMeshProUGUI tvides;

    public TextMeshProUGUI tscore;

	public TextMeshProUGUI tmaxscore;

    public void UpdateLives(int vides)
	{
		if (tvides != null && vides != -1)
		{
			tvides.text = "Vidas: " + vides.ToString();
		}
	}

    public void UpdateScore(int scoreActual)
    {
        if (tscore != null)
            tscore.text = "Puntos: " + scoreActual.ToString();
    }

    public void UpdateMaxScore(int maxScore)
	{
        if (tmaxscore != null)
            tmaxscore.text = "Record de puntos: " + maxScore.ToString();
    }

    void Start()
	{
		if (GameManager.instance != null)
		{
			UpdateLives(GameManager.instance.vides - 1); // Actualitza la UI de vides
            UpdateScore(GameManager.instance.scoreActual); // Actualitza la UI de puntuació
			UpdateMaxScore(GameManager.instance.maxScore); // Actualitza la UI del màxim de puntuació
        }
		else
		{
			Debug.LogWarning("No s'ha trobat GameManager per actualitzar les vides.");
		}
	}
}
