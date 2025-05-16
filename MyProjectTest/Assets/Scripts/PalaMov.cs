using System.Collections;
using UnityEngine;

public class PalaMov : MonoBehaviour
{
    public float scaleChange = 5.0f; //POWER-UP de augmentar
    public GameObject ball; // assigna la pilota manualment al inspector

    public KeyCode left;
    public KeyCode right;

    public float size;

    private float originalSize;
    private Coroutine revertCoroutine;


    enum State
    {
        left,
        right,
        stop
    }

    State state;

    void Start()
    {
        originalSize = transform.localScale.x;
    }

    void Update()
    {
        size = transform.localScale.x;

        if (Input.GetKey(left))
        {
            state = State.left;
        }
        else if (Input.GetKey(right))
        {
            state = State.right;
        }
        else
        {
            state = State.stop;
        }

        Move();
    }

    void Move()
    {
        float moveSpeed = 10f * Time.deltaTime;
        float newX = transform.position.x;
        float limit = 8 - size / 2;

        if (state == State.left && newX > -limit)
        {
            transform.Translate(-moveSpeed, 0, 0);
        }
        else if (state == State.right && newX < limit)
        {
            transform.Translate(moveSpeed, 0, 0);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Augmentar"))
        {
            // Augmenta escala
            Vector3 scale = transform.localScale;
            scale.x += scaleChange;
            transform.localScale = scale;

            // Recalcula l�mit m�xim basat en el nou tamany
            size = scale.x;
            float limit = 8f - size / 2f;

            // Limita la posici� actual
            float clampedX = Mathf.Clamp(transform.position.x, -limit, limit);
            transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);

            // Reinicia el temporitzador si ja hi ha un actiu
            if (revertCoroutine != null)
            {
                StopCoroutine(revertCoroutine);
            }
            revertCoroutine = StartCoroutine(RevertAfterTime(5f)); // 5 segons

            Destroy(other.gameObject);
        }
        else if (other.CompareTag("PowerBall"))
        {
            BallMov ballScript = ball.GetComponent<BallMov>();
            if (ballScript != null)
            {
                ballScript.ActivatePowerBall();
            }

            Destroy(other.gameObject);
        }

        else if (other.CompareTag("ExtraBall"))
        {
            GameObject[] balls = GameObject.FindGameObjectsWithTag("Ball");

            foreach (GameObject b in balls)
            {
                Rigidbody rb = b.GetComponent<Rigidbody>();
                BallMov ballMov = b.GetComponent<BallMov>();

                bool isPowerBall = ballMov.isPowerBall;
                bool isIman = ballMov.isImant;

                if (rb != null && ballMov != null)
                {
                    Vector3 dir = rb.linearVelocity.normalized;
                    float speed = ballMov.speed;

                    // Crea dues noves boles amb angles diferents
                    //CreateExtraBall(b.transform.position, Quaternion.Euler(0, 0, 0) * dir, speed);
                    CreateExtraBall(b.transform.position, Quaternion.Euler(0, 20, 0) * dir, speed, isPowerBall, isIman);
                    CreateExtraBall(b.transform.position, Quaternion.Euler(0, -20, 0) * dir, speed, isPowerBall, isIman);
                }
            }

            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Imant"))
        {
            GameObject[] balls = GameObject.FindGameObjectsWithTag("Ball");

            foreach (GameObject b in balls)
            {
                BallMov ballScript = b.GetComponent<BallMov>();
                if (ballScript != null)
                {
                    ballScript.ActivateImant();
                }
            }

            Destroy(other.gameObject);
        }
        else if (other.CompareTag("NormalBall"))
        {
            GameObject[] balls = GameObject.FindGameObjectsWithTag("Ball");

            foreach (GameObject b in balls)
            {
                BallMov ballScript = b.GetComponent<BallMov>();
                if (ballScript != null)
                {
                    ballScript.normalBall = true;
                }

            }
            Destroy(other.gameObject);
        }
    }

    void CreateExtraBall(Vector3 position, Vector3 direction, float speed, bool isPowerBall, bool isIman)
    {
        GameObject newBall = Instantiate(ball, position, Quaternion.identity);
        Rigidbody rb = newBall.GetComponent<Rigidbody>();
        BallMov ballMov = newBall.GetComponent<BallMov>();

        if (rb != null && ballMov != null)
        {
            //ballMov.startLaunched = true;
            ballMov.isPowerBall = isPowerBall;
            ballMov.isExtraBall = true; // per evitar que es quedi enganxada a la pala
            ballMov.originPosition = position;
            ballMov.isImant = isIman;

            rb.linearVelocity = direction.normalized * speed;

            if (!isPowerBall) newBall.GetComponent<Renderer>().material = ballMov.normalMaterial;
            else newBall.GetComponent<Renderer>().material = ballMov.powerBallMaterial;
        }
    }

    // Temporitzador
    IEnumerator RevertAfterTime(float delay)
    {
        yield return new WaitForSeconds(delay);

        Vector3 scale = transform.localScale;
        scale.x = originalSize;
        transform.localScale = scale;

        // Reajusta posici� si cal
        float limit = 8f - originalSize / 2f;
        float clampedX = Mathf.Clamp(transform.position.x, -limit, limit);
        transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);

        size = originalSize;
    }
}