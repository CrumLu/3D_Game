using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
	public TextMeshProUGUI tvides;

    public TextMeshProUGUI tscore;

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
            tscore.text = "Score: " + scoreActual;
    }

    void Start()
	{
		if (GameManager.instance != null)
		{
			UpdateLives(GameManager.instance.vides - 1); // Actualitza la UI de vides
		}
		else
		{
			Debug.LogWarning("No s'ha trobat GameManager per actualitzar les vides.");
		}
	}
}
