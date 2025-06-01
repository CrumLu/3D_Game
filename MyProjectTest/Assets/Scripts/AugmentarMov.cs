using UnityEngine;

public class AugmentarMov : MonoBehaviour
{
    public float fallSpeed = 10.0f;

    void Update()
    {
        transform.Rotate(0, 200f * Time.deltaTime, 0);
        //transform.Translate(0, 0, -fallSpeed * Time.deltaTime);
        transform.Translate(Vector3.back * fallSpeed * Time.deltaTime, Space.World);
    }
}