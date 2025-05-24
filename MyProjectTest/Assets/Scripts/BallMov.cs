using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMov : MonoBehaviour
{
    // Direccion de la Ball
    public Vector3 direccion;
    
    private float collisionCooldown = 0.02f;  // Temps mínim entre col·lisions
    private float lastCollisionTime = -1f;


    // Variables de SlowBall
    private Coroutine slowBallCoroutine;
    private float slowSpeed = 3f;
    private float baseSpeed = 5f; // velocitat normal

    // Variables de FastBall
    private Coroutine fastBallCoroutine;
    private float targetSpeed = 7f; // Velocitat final després de l'efecte
    private float fastSpeed = 12f;

    // Variables de imant
    public bool isImant = false;
    private Vector3 imantOffset;
    private bool siguePala = false;
    private Coroutine imantCoroutine;

    // Variables de ExtaBall
    public bool isExtraBall = false; // si és true, no es queda enganxada a la pala
    public Vector3 originPosition;

    // Variables de moviment
    public float speed = 7.0f;
    private Rigidbody rb;

    // Variables del POWERBALL
    public bool normalBall = false;
    private Coroutine powerBallCoroutine;
    public bool isPowerBall = false;
    public Material powerBallMaterial;
    public Material normalMaterial;

    //Ball enganxada a la Pala
    private bool isLaunched = false;
    private Transform palaTransform;

    private Vector3 offsetToPala = new Vector3(0, -0.4f, 0.75f); // ajustable segons la mida



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
                transform.position = palaTransform.position + offsetToPala;
            }

            if (Input.GetKeyDown(KeyCode.Space) || isExtraBall)
            {
                if (isExtraBall)
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
                //isImant = true;
            }
        }
        // Si la bola és imant, s'enganxa a la pala
        else if (siguePala)
        {
            transform.position = palaTransform.position + imantOffset;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (isExtraBall)
                {
                    float x = Random.Range(-1f, 1f);
                    float z = Random.Range(0.5f, 1f);
                    Vector3 randomDir = new Vector3(x, 0, z).normalized;
                    rb.linearVelocity = randomDir * speed;
                }
                else
                {

                    // Determina la direcció en funció de la posició relativa de la bola respecte a la pala
                    float offset = imantOffset.x;

                    Vector3 newDir;
                    if (offset < -0.1f)
                        newDir = new Vector3(-1, 0, 1);
                    else if (offset > 0.1f)
                        newDir = new Vector3(1, 0, 1);
                    else
                        newDir = new Vector3(0, 0, 1);

                    rb.linearVelocity = newDir.normalized * speed;

                }
                siguePala = false; // Deixa de seguir la pala
            }
        }
    }


    void FixedUpdate()
    {
        if (isLaunched)
        {
            direccion = rb.linearVelocity.normalized;
            direccion = EnforceMinRebound(direccion, 0.2f, 0.2f, 0.7f);
            rb.linearVelocity = direccion * speed;
        }
    }





    private void OnCollisionEnter(Collision collision)
    {
        if (isLaunched)
        {
            if (collision.gameObject.CompareTag("Brick"))
            {
                // Gestió de cooldown per evitar dobles col·lisions
                if (Time.time - lastCollisionTime < collisionCooldown)
                {
                    // Massa aviat després de l'última col·lisió; ignora aquesta
                    return;
                }

                lastCollisionTime = Time.time; // Actualitza el temps de l'última col·lisió

                if (isPowerBall)
                {
                    Destroy(collision.gameObject);
                    rb.linearVelocity = direccion * speed;
                    return;
                }
                else
                {
                    // Gestiona rebot normal
                    ContactPoint contacto = collision.GetContact(0);
                    Vector3 normal = contacto.normal;

                    if (Mathf.Abs(normal.x) > Mathf.Abs(normal.z))
                        direccion.x *= -1;
                    else
                        direccion.z *= -1;

                    rb.linearVelocity = direccion * speed;

                    Destroy(collision.gameObject);
                }
            }
            else if (collision.gameObject.CompareTag("Pala"))
            {
                if (isImant)
                {
                    Vector3 hitPoint = collision.GetContact(0).point;
                    imantOffset = hitPoint - palaTransform.position;
                    siguePala = true;
                }
                else
                {
                    Vector3 hit = collision.contacts[0].point;
                    Vector3 palaCenter = collision.transform.position;
                    float offset = hit.x - palaCenter.x;
                    float width = collision.collider.bounds.size.x;
                    float normalizedOffset = offset / (width / 2);

                    Vector3 newDir = new Vector3(normalizedOffset, 0, 1).normalized;
                    newDir = EnforceMinRebound(newDir, 0.2f, 0.2f, 0.6f);
                    rb.linearVelocity = newDir * speed;
                }
            }
            else if (collision.gameObject.CompareTag("Paret"))
            {
                Vector3 velocity = rb.linearVelocity;
                if (Mathf.Abs(velocity.z) < 1f)
                {
                    Vector3 hit = collision.contacts[0].point;
                    Vector3 palaCenter = collision.transform.position;
                    float offset = hit.x - palaCenter.x;
                    float width = collision.collider.bounds.size.x;
                    float normalizedOffset = offset / (width / 2);

                    direccion = new Vector3(normalizedOffset, 0, 1).normalized;
                    direccion = EnforceMinRebound(direccion, 0.2f, 0.2f, 0.6f);
                    rb.linearVelocity = direccion * speed;
                }
            }
        }
    }




    //Gestión del POWERBALL
    public void ActivatePowerBall()
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
    }

    public void DeactivatePowerBall()
    {
        isPowerBall = false;

        if (powerBallCoroutine != null)
        {
            StopCoroutine(powerBallCoroutine);
            powerBallCoroutine = null;
        }

        if (normalMaterial != null)
        {
            GetComponent<Renderer>().material = normalMaterial;
        }

        // Restaura els bricks a col·lisions normals
    }



    //Gestión del IMANT
    public void ActivateImant()
    {
        isImant = true;

        if (imantCoroutine != null)
            StopCoroutine(imantCoroutine);

        imantCoroutine = StartCoroutine(ImantDuration());
    }


    IEnumerator ImantDuration()
    {
        yield return new WaitForSeconds(10f);
        isImant = false;
    }

    /*

    //Gestión del FASTBALL
    public void ActivateFastBall()
    {
        if (fastBallCoroutine != null)
            StopCoroutine(fastBallCoroutine);

        // Desactiva efectes que interfereixen
        isImant = false;
        siguePala = false;

        // Assigna la nova velocitat inicial
        speed = fastSpeed;

        // Llença la bola si estava enganxada
        if (!isLaunched)
        {
            Vector3 launchDir = new Vector3(0, 0, 1);
            rb.linearVelocity = launchDir.normalized * speed;
            isLaunched = true;
        }

        // Inicia la reducció progressiva de velocitat
        fastBallCoroutine = StartCoroutine(FastBallDecay());
    }

    IEnumerator FastBallDecay()
    {
        float duration = 15f;
        float elapsed = 0f;
        float initialSpeed = speed;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            speed = Mathf.Lerp(fastSpeed, targetSpeed, elapsed / duration);
            yield return null;
        }

        speed = targetSpeed;
    }

    //Gestión del SLOWBALL
    public void ActivateSlowBall()
    {
        if (slowBallCoroutine != null)
            StopCoroutine(slowBallCoroutine);

        // Desactiva efectes incompatibles
        DeactivatePowerBall();
        isImant = false;
        siguePala = false;

        // Aplica velocitat lenta inicial
        speed = slowSpeed;

        // Si estava enganxada, llença-la
        if (!isLaunched)
        {
            Vector3 launchDir = new Vector3(0, 0, 1);
            rb.linearVelocity = launchDir.normalized * speed;
            isLaunched = true;
        }

        slowBallCoroutine = StartCoroutine(SlowBallRampUp());
    }

    IEnumerator SlowBallRampUp()
    {
        float duration = 10f;
        float elapsed = 0f;
        float initialSpeed = speed;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            speed = Mathf.Lerp(slowSpeed, baseSpeed, elapsed / duration);
            yield return null;
        }

        speed = baseSpeed;
    }

    */

    public void ActivateFastBall()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();

        if (direccion == Vector3.zero)
            direccion = rb.linearVelocity.normalized;

        speed = speed * 1.5f;
        rb.linearVelocity = direccion * speed;
    }


    public void ActivateSlowBall()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();

        if (direccion == Vector3.zero)
            direccion = rb.linearVelocity.normalized;

        speed = speed / 1.5f;
        rb.linearVelocity = direccion * speed;
    }

    private Vector3 EnforceMinRebound(Vector3 dir, float minX, float minZ, float blendFactor)
    {
        Vector3 corrected = dir;

        if (Mathf.Abs(corrected.z) < minZ)
            corrected.z = minZ * Mathf.Sign(Random.Range(-1f, 1f));

        if (Mathf.Abs(corrected.x) < minX)
            corrected.x = minX * Mathf.Sign(Random.Range(-1f, 1f));

        // Interpolació suau: només s'aplica una part del canvi
        return Vector3.Slerp(dir, corrected.normalized, blendFactor).normalized;
    }
}