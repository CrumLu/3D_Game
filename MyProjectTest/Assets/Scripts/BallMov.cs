using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMov : MonoBehaviour
{
    // Variables de imant
    public bool isImant = false;
    private Vector3 imantOffset;

    // Variables de ExtaBall
    public bool isExtraBall = false; // si és true, no es queda enganxada a la pala
    public Vector3 originPosition;

    // Variables de moviment
    public float speed = 5.0f;
    private Rigidbody rb;

    // Variables del POWERBALL
    private Coroutine powerBallCoroutine;
    public bool isPowerBall = false;
    public Material powerBallMaterial;
    public Material normalMaterial;

    //Ball enganxada a la Pala
    private bool isLaunched = false;
    private Transform palaTransform;
    private Vector3 offsetToPala = new Vector3(0, 0, 0.75f); // ajustable segons la mida

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        GameObject pala = GameObject.FindGameObjectWithTag("Pala");
        if (pala != null)
        {
            palaTransform = pala.transform;
            rb.linearVelocity = Vector3.zero;
            //isLaunched = startLaunched; // només estarà a false per la primera bola
        }
        else
        {
            Debug.LogError("No s'ha trobat cap objecte amb tag 'Pala'");
        }
    }

    void Update()
    {
        if (!isLaunched && palaTransform != null)
        {
            if (!isExtraBall)
            {
                if (isImant)
                {
                    transform.position = palaTransform.position + imantOffset;
                }
                else
                {
                    transform.position = palaTransform.position + offsetToPala;
                }
            }

            if (Input.GetKeyDown(KeyCode.Space) || isExtraBall)
            {
                if (isImant)
                {
                    float offset = imantOffset.x;
                    float width = 2f; // ajustable segons la mida real de la pala
                    float normalizedOffset = Mathf.Clamp(offset / (width / 2f), -1f, 1f);
                    Vector3 dir = new Vector3(normalizedOffset, 0, 1).normalized;
                    rb.linearVelocity = dir * speed;
                }
                else if (isExtraBall)
                {
                    float x = Random.Range(-1f, 1f);
                    float z = Random.Range(0.5f, 1f);
                    Vector3 randomDir = new Vector3(x, 0, z).normalized;
                    rb.linearVelocity = randomDir * speed;
                }
                else
                {
                    rb.linearVelocity = new Vector3(-1, 0, 1).normalized * speed;
                }

                isLaunched = true;
                isImant = false;
            }

        }
    }


    void FixedUpdate()
    {
        if (isLaunched)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * speed;
        }
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
        // 🔁 Activa PowerBall a totes les boles
        GameObject[] balls = GameObject.FindGameObjectsWithTag("Ball");
        foreach (GameObject b in balls)
        {
            BallMov ball = b.GetComponent<BallMov>();
            if (ball != null)
            {
                ball.isPowerBall = true;
                if (ball.powerBallMaterial != null)
                    b.GetComponent<Renderer>().material = ball.powerBallMaterial;
            }
        }

        // 🔁 Converteix bricks a triggers
        GameObject[] bricks = GameObject.FindGameObjectsWithTag("Brick");
        foreach (GameObject brick in bricks)
        {
            Collider col = brick.GetComponent<Collider>();
            if (col != null)
                col.isTrigger = true;
        }

        yield return new WaitForSeconds(5f);

        // 🔁 Desactiva PowerBall a totes les boles
        balls = GameObject.FindGameObjectsWithTag("Ball");
        foreach (GameObject b in balls)
        {
            BallMov ball = b.GetComponent<BallMov>();
            if (ball != null)
            {
                ball.isPowerBall = false;
                if (ball.normalMaterial != null)
                    b.GetComponent<Renderer>().material = ball.normalMaterial;
            }
        }

        // 🔁 Torna els bricks a col·lisions normals
        bricks = GameObject.FindGameObjectsWithTag("Brick");
        foreach (GameObject brick in bricks)
        {
            Collider col = brick.GetComponent<Collider>();
            if (col != null)
                col.isTrigger = false;
        }
    }

    public void ActivateImant(Vector3 collisionPoint)
    {
        isLaunched = false;
        isImant = true;
        rb.linearVelocity = Vector3.zero;

        if (palaTransform != null)
            imantOffset = collisionPoint - palaTransform.position;
    }

}