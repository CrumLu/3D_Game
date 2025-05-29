using UnityEngine;
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI nextLevelText;

    public TextMeshProUGUI tvides;

    public TextMeshProUGUI tscore;

	public TextMeshProUGUI tmaxscore;

    public TextMeshProUGUI tpowerup;
    private Coroutine powerUpCoroutine = null; // Guarda la corutina actual

    public void DefaultPowerUpText()
    {
        if (tpowerup != null)
        {
            tpowerup.text = "P-UP: ";
        }
    }

    public void UpdatePowerUp(string powerUpName)
    {
        if (tpowerup != null)
        {
            tpowerup.text = "P-UP: " + powerUpName;

            // Atura la corutina anterior si existeix
            if (powerUpCoroutine != null)
                StopCoroutine(powerUpCoroutine);

            powerUpCoroutine = StartCoroutine(ClearPowerUpTextAfterDelay(2f));
        }
    }

    private IEnumerator ClearPowerUpTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (tpowerup != null)
            tpowerup.text = "P-UP: ";
        powerUpCoroutine = null; // Reseteja la referència
    }


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

    public void ShowNextLevelText()
    {
        if (nextLevelText != null)
            nextLevelText.gameObject.SetActive(true);
    }

    public void HideNextLevelText()
    {
        if (nextLevelText != null)
            nextLevelText.gameObject.SetActive(false);
    }

    public void ShowNextLevelTextFade(float fadeDuration = 0.7f)
    {
        if (nextLevelText != null)
        {
            nextLevelText.gameObject.SetActive(true);
            StartCoroutine(FadeInNextLevelText(fadeDuration));
        }
    }

    private IEnumerator FadeInNextLevelText(float duration)
    {
        Color c = nextLevelText.color;
        c.a = 0f;
        nextLevelText.color = c;

        float timer = 0f;
        while (timer < duration)
        {
            float t = timer / duration;
            c.a = Mathf.Lerp(0, 1, t);
            nextLevelText.color = c;
            timer += Time.deltaTime;
            yield return null;
        }
        // Assegura alpha = 1
        c.a = 1f;
        nextLevelText.color = c;
    }


    public void HideNextLevelTextFade(float fadeDuration = 0.5f)
    {
        if (nextLevelText != null)
            StartCoroutine(FadeOutNextLevelText(fadeDuration));
    }

    private IEnumerator FadeOutNextLevelText(float duration)
    {
        Color c = nextLevelText.color;
        float startAlpha = c.a;

        float timer = 0f;
        while (timer < duration)
        {
            float t = timer / duration;
            c.a = Mathf.Lerp(startAlpha, 0, t);
            nextLevelText.color = c;
            timer += Time.deltaTime;
            yield return null;
        }
        // Assegura alpha = 0
        c.a = 0f;
        nextLevelText.color = c;
        nextLevelText.gameObject.SetActive(false);
    }
}
