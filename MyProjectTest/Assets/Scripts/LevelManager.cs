using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public GameObject specialPowerUpPrefab;

    private int totalBricks;
    private int destroyedBricks = 0;
    private bool specialPowerUpSpawned = false;

    void Start()
    {
        totalBricks = GameObject.FindGameObjectsWithTag("Brick").Length;
    }

    public void BrickDestroyed()
    {
        destroyedBricks++;

        float percentDestroyed = (float)destroyedBricks / totalBricks;

        if (!specialPowerUpSpawned && percentDestroyed >= 0.95f)
        {
            SpawnSpecialPowerUp();
        }
    }

    void SpawnSpecialPowerUp()
    {
        specialPowerUpSpawned = true;

        if (specialPowerUpPrefab != null)
        {
            Instantiate(specialPowerUpPrefab, new Vector3(0f, 10f, 0f), Quaternion.identity);
        }
        else
        {
            Debug.LogError("Falta assignar el prefab pel PowerUp especial.");
        }
    }
}
