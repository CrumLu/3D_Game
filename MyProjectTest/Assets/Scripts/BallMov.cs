using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMov : MonoBehaviour
{
    public float speed = 5f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Movimiento inicial hacia adelante
        int directionZ = Random.Range(0, 2) == 0 ? 1 : -1;
        rb.linearVelocity = new Vector3(0, 0, speed * directionZ);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Pala"))
        {
            Vector3 hitPoint = collision.contacts[0].point;
            Vector3 palaCenter = collision.transform.position;

            float offset = hitPoint.x - palaCenter.x;
            float width = collision.collider.bounds.size.x;

            float normalizedOffset = offset / (width / 2);

            Vector3 newDir = new Vector3(normalizedOffset, 0, 1).normalized;

            rb.linearVelocity = newDir * speed;
        }
    }
}
