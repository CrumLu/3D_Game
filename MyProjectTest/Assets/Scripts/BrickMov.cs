using UnityEngine;

public class BrickMov : MonoBehaviour
{
    public GameObject powerUpPrefab; // només un prefab a assignar

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            // Si té assignat un power-up, l’instancia
            if (powerUpPrefab != null)
            {
                Instantiate(powerUpPrefab, transform.position, Quaternion.identity);
            }

            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            // Si la pilota entra (en mode PowerBall), destruïm el brick
            if (powerUpPrefab != null)
            {
                Instantiate(powerUpPrefab, transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
        }
    }

}
