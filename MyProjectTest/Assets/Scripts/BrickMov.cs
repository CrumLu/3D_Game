using UnityEngine;

public class BrickMov : MonoBehaviour
{
    public GameObject powerUpPrefab;

    private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Ball"))
            {
                // Apareix el power-up amb una probabilitat, o sempre
                Instantiate(powerUpPrefab, transform.position, Quaternion.identity);

                Destroy(gameObject);
            }
        }
}
