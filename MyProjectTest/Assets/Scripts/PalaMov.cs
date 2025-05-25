using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class PalaMov : MonoBehaviour
{
    public float scaleChange = 3.0f; //POWER-UP de augmentar
    public GameObject ball; // assigna la pilota manualment al inspector

    public KeyCode left;
    public KeyCode right;

    public AudioClip PowerUpSound;
    public AudioMixerGroup sfxMixerGroup;

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
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Augmentar"))
        {
            if (PowerUpSound != null && sfxMixerGroup != null)
            {
                AudioSource.PlayClipAtPoint(PowerUpSound, transform.position, 1.0f);
            }   
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

            Destroy(other.gameObject);

        }
        else if (other.CompareTag("Disminuir"))
        {
            if (PowerUpSound != null && sfxMixerGroup != null)
            {
                AudioSource.PlayClipAtPoint(PowerUpSound, transform.position, 1.0f);
            }
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

            if (PowerUpSound != null && sfxMixerGroup != null)
            {
                AudioSource.PlayClipAtPoint(PowerUpSound, transform.position, 1.0f);
            }
            if (ballScript != null)
            {
                ballScript.ActivatePowerBall();
            }

            Destroy(other.gameObject);
        }

        else if (other.CompareTag("ExtraBall"))
        {
            if (PowerUpSound != null && sfxMixerGroup != null)
            {
                AudioSource.PlayClipAtPoint(PowerUpSound, transform.position, 1.0f);
            }

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
            if (PowerUpSound != null && sfxMixerGroup != null)
            {
                AudioSource.PlayClipAtPoint(PowerUpSound, transform.position, 1.0f);
            }

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
            if (PowerUpSound != null && sfxMixerGroup != null)
            {
                AudioSource.PlayClipAtPoint(PowerUpSound, transform.position, 1.0f);
            }

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
            if (PowerUpSound != null && sfxMixerGroup != null)
            {
                AudioSource.PlayClipAtPoint(PowerUpSound, transform.position, 1.0f);
            }

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
            if (PowerUpSound != null && sfxMixerGroup != null)
            {
                AudioSource.PlayClipAtPoint(PowerUpSound, transform.position, 1.0f);
            }

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
            if (PowerUpSound != null && sfxMixerGroup != null)
            {
                AudioSource.PlayClipAtPoint(PowerUpSound, transform.position, 1.0f);
            }

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

    public bool ultimaBall()
    {
        /*
        GameObject[] balls = GameObject.FindGameObjectsWithTag("Ball");
        if (balls.Length == 1)
        {
            Vector3 pos = balls[0].transform.position;
            if (
                pos.z > -11f && pos.z < 10f &&
                pos.x > -11f && pos.x < 10f
            )
            {
                // L'única bola està dins dels límits -> retorna true
                return true;
            }
        }
        // No és l'última bola dins dels límits
        return false;
        */
        return true;
    }



    /*
    public void CheckBallsLeftAndHandleLives()
    {
        // Mira quantes boles queden al mapa
        int ballsLeft = GameObject.FindGameObjectsWithTag("Ball").Length;

        if (ballsLeft <= 1) // Aquesta bola encara no s'ha destruït, així que és la darrera
        {
            vides--;
            if (vides > 0)
            {
                // Instancia una nova bola "normal"
                Vector3 spawnPos = transform.position + ballStartOffset;
                GameObject newBall = Instantiate(ball, spawnPos, Quaternion.identity);
                BallMov ballMov = newBall.GetComponent<BallMov>();
                if (ballMov != null)
                {
                    ballMov.isLaunched = false;
                    ballMov.isExtraBall = false;
                    ballMov.isImant = false;
                    ballMov.isPowerBall = false;
                    ballMov.direccion = Vector3.zero;
                    ballMov.speed = ballMov.speed; // assegura’t de tenir aquesta variable!
                }
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("GameOver");
            }
        }
        // Si encara queden altres boles, no fem res més (només desapareix la que ha caigut)
    }




    
    // PalaMov.cs
    public void LoseLifeAndRespawnBall()
    {
        vides--;

        if (vides > 0)
        {
            Vector3 spawnPos = transform.position + ballStartOffset;
            GameObject newBall = Instantiate(ball, spawnPos, Quaternion.identity);

            BallMov ballMov = newBall.GetComponent<BallMov>();
            if (ballMov != null)
            {
                ballMov.isLaunched = false;
                ballMov.isExtraBall = false;
                ballMov.isImant = false;
                ballMov.isPowerBall = false;
                ballMov.direccion = Vector3.zero;
                ballMov.speed = ballMov.speedInicial; // assegura’t de tenir aquesta variable!
                if (ballMov.normalMaterial != null)
                    newBall.GetComponent<Renderer>().material = ballMov.normalMaterial;
                // Restaura qualsevol altre variable de powerup aquí!
            }
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("GameOver");
        }
    }
    */
}