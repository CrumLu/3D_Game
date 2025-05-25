using UnityEngine;
using UnityEngine.Audio;

public class BrickMov : MonoBehaviour
{
    public GameObject powerUpPrefab; // només un prefab a assignar
    public AudioClip breakSound;
    public AudioMixerGroup sfxMixerGroup;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            HandleDestruction();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            HandleDestruction();
        }
    }

    private void HandleDestruction()
    {
        if (breakSound != null && sfxMixerGroup != null)
        {
            AudioSource.PlayClipAtPoint(breakSound, transform.position);
        }
        // Instanciem el power-up si existeix
        if (powerUpPrefab != null)
        {
            Instantiate(powerUpPrefab, transform.position, Quaternion.identity);
        }

        // Notificar el LevelManager que aquest brick ha estat destruït
        LevelManager levelManager = FindObjectOfType<LevelManager>();
        if (levelManager != null)
        {
            levelManager.BrickDestroyed();
        }

        Destroy(gameObject);
    }
}
