using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMov : MonoBehaviour
{
    // Variables de moviment
    public float speed = 5.0f;
    private Rigidbody rb;

    // Variables del POWERBALL
    private Coroutine powerBallCoroutine;
    public bool isPowerBall = false;
    public Material powerBallMaterial;
    public Material normalMaterial;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        int directionZ = Random.Range(0, 2) == 0 ? 1 : -1;
        rb.linearVelocity = new Vector3(0, 0, speed * directionZ);
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = rb.linearVelocity.normalized * speed;
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

    //Gestión del POWERBALL
    public void ActivatePowerBall()
    {
        if (powerBallCoroutine != null)
        {
            StopCoroutine(powerBallCoroutine);
        }

        powerBallCoroutine = StartCoroutine(PowerBallDuration());
    }

    IEnumerator PowerBallDuration()
    {
        isPowerBall = true;

        // Canvia al material del POWERBALL
        if (powerBallMaterial != null) GetComponent<Renderer>().material = powerBallMaterial;

        // Converteix tots els bricks en triggers
        GameObject[] bricks = GameObject.FindGameObjectsWithTag("Brick");
        foreach (GameObject brick in bricks)
        {
            Collider col = brick.GetComponent<Collider>();
            if (col != null)
            {
                col.isTrigger = true;
            }
        }

        yield return new WaitForSeconds(5f); // el temps que dura el POWERBALL

        isPowerBall = false;

        // Restaura el material normal
        if (normalMaterial != null)
            GetComponent<Renderer>().material = normalMaterial;

        // Torna els bricks restants a col·lisors normals
        bricks = GameObject.FindGameObjectsWithTag("Brick");
        foreach (GameObject brick in bricks)
        {
            Collider col = brick.GetComponent<Collider>();
            if (col != null)
            {
                col.isTrigger = false;
            }
        }
    }
}
