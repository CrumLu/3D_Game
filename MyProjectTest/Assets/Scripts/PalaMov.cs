using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PalaMov : MonoBehaviour
{
    public float scaleChange = 3.0f; //POWER-UP de augmentar
    public GameObject ball; // assigna la pilota manualment al inspector

    public KeyCode left;
    public KeyCode right;

    public float size;

    private float originalSize;
    private Coroutine revertCoroutine;

    // Vides
    public int vides = 3;
    public Vector3 ballStartOffset = new Vector3(0, -0.45f, 0.75f);



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
        //palaTransform = this.transform;
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
        //Debug.Log("Comprobación: " + size);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Augmentar"))
        {
            // Augmenta escala
            Vector3 scale = transform.localScale;
            scale.x += scaleChange;
            transform.localScale = scale;

            //Debug.Log("New scale: " + scale.x);

            // Recalcula l�mit m�xim basat en el nou tamany
            size = scale.x;
            float limit = 8f - size / 2f;

            // Limita la posici� actual
            float clampedX = Mathf.Clamp(transform.position.x, -limit, limit);
            transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);

            Destroy(other.gameObject);

        }
        else if (other.CompareTag("Disminuir"))
        {
            // Disminueix escala
            Vector3 scale = transform.localScale;
            scale.x -= scaleChange;
            transform.localScale = scale;
            // Recalcula l�mit m�xim basat en el nou tamany
            size = scale.x;
            float limit = 8f - size / 2f;
            // Limita la posici� actual
            float clampedX = Mathf.Clamp(transform.position.x, -limit, limit);
            transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);

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
                    ballScript.DeactivatePowerBall(); // Nova línia
                    ballScript.normalBall = true;
                }
            }

            Destroy(other.gameObject);
        }

        // MILLORAR AMB EXTRA BALL I TOT
        else if (other.CompareTag("FastBall"))
        {
            GameObject[] balls = GameObject.FindGameObjectsWithTag("Ball");
            foreach (GameObject b in balls)
            {
                BallMov ballScript = b.GetComponent<BallMov>();
                if (ballScript != null)
                {
                    ballScript.ActivateFastBall();
                }
            }
            Destroy(other.gameObject);
        }

        else if (other.CompareTag("SlowBall"))
        {
            GameObject[] balls = GameObject.FindGameObjectsWithTag("Ball");

            foreach (GameObject b in balls)
            {
                BallMov ballScript = b.GetComponent<BallMov>();
                if (ballScript != null)
                {
                    ballScript.ActivateSlowBall();
                }
            }

            Destroy(other.gameObject);
        }
        else if (other.CompareTag("NextLevel"))
        {
            string[] sceneOrder = { "Level01", "Level02", "Level03", "Level04", "Level05", "Credits" };
            string currentScene = SceneManager.GetActiveScene().name;

            int index = System.Array.IndexOf(sceneOrder, currentScene);
            if (index >= 0 && index < sceneOrder.Length - 1)
            {
                // Carrega la següent escena de la llista
                SceneManager.LoadScene(sceneOrder[index + 1]);
            }
            else
            {
                Debug.LogWarning("Escena actual no trobada o ja a la darrera escena.");
            }

            Destroy(other.gameObject); // Destrueix la clau després d'usar-la
        }
    }

    void CreateExtraBall(Vector3 position, Vector3 direction, float speed, bool isPowerBall, bool isIman)
    {
        GameObject newBall = Instantiate(ball, position, Quaternion.identity);
        Rigidbody rb = newBall.GetComponent<Rigidbody>();
        BallMov ballMov = newBall.GetComponent<BallMov>();

        if (rb != null && ballMov != null)
        {
            ballMov.direccion = direction.normalized; // Inicialitza direcció pública
            ballMov.isPowerBall = isPowerBall;
            ballMov.isExtraBall = true;
            ballMov.originPosition = position;
            ballMov.isImant = isIman;

            rb.linearVelocity = ballMov.direccion * speed;

            if (!isPowerBall)
                newBall.GetComponent<Renderer>().material = ballMov.normalMaterial;
            else
                newBall.GetComponent<Renderer>().material = ballMov.powerBallMaterial;
        }
    }
}