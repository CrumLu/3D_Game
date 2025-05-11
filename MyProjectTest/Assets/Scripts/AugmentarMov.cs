using UnityEngine;

public class AugmentarMov : MonoBehaviour
{
    public float fallSpeed = 10.0f;

    void Update()
    {
        transform.Translate(0, 0, -fallSpeed * Time.deltaTime);
    }
}