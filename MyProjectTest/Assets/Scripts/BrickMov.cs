using UnityEngine;
using UnityEngine.Audio;


public class BrickMov : MonoBehaviour
{
    public AudioClip breakSound;
    public AudioMixerGroup sfxMixerGroup;

    public GameObject powerUpPrefab; // nom�s un prefab a assignar
    public GameObject destroyParticlesPrefab;

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

        // Notificar el LevelManager que aquest brick ha estat destru�t
        LevelManager levelManager = FindObjectOfType<LevelManager>();
        if (levelManager != null)
        {
            levelManager.BrickDestroyed();
        }

        GameManager.instance.ActualizaPuntuacion(100); // 'puntuacio' pot ser 10, 100, etc.

        if (destroyParticlesPrefab != null)
        {
            Instantiate(destroyParticlesPrefab, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
}