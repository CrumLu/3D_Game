using UnityEngine;

public class BrickMov : MonoBehaviour
{
    public GameObject powerUpPrefab; // nom�s un prefab a assignar

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

        Destroy(gameObject);
    }
}
