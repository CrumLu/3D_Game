using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class PalaMov : MonoBehaviour
{
    public AudioClip PowerUpSound;
    public AudioMixerGroup sfxMixerGroup;


    public float scaleChange = 3.0f; //POWER-UP de augmentar
    public GameObject ball; // assigna la pilota manualment al inspector

    public KeyCode left;
    public KeyCode right;

    public Material imantMaterial;
    public Material normalMaterial;
    private Renderer rend;

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
        FindObjectOfType<UIManager>().HideNextLevelText();

        originalSize = transform.localScale.x;
        rend = GetComponent<Renderer>();
        if (normalMaterial != null)
            rend.material = normalMaterial;
        //palaTransform = this.transform;
    }

    void Update()
    {
        CameraIntroController controller = FindObjectOfType<CameraIntroController>();
        if (controller != null && controller.introFinished)
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

    public void ActivateImantVisual()
    {
        if (rend != null && imantMaterial != null)
        {
            rend.material = imantMaterial; 
        }
        StartCoroutine(ResetImantVisualAfterDelay(10f));
    }

    public void DeactivateImantVisual()
    {
        if (rend != null && normalMaterial != null)
        {
            rend.material = normalMaterial;
        }
    }

    IEnumerator ResetImantVisualAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        DeactivateImantVisual();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Augmentar"))
        {
            if (PowerUpSound != null && sfxMixerGroup != null)
            {
                AudioSource.PlayClipAtPoint(PowerUpSound, transform.position, 1.0f);
            }


            UIManager ui = FindObjectOfType<UIManager>();
            if (ui != null)
            {
                ui.UpdatePowerUp("Augmentar");
            }

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
            if (PowerUpSound != null && sfxMixerGroup != null)
            {
                AudioSource.PlayClipAtPoint(PowerUpSound, transform.position, 1.0f);
            }

            UIManager ui = FindObjectOfType<UIManager>();
            if (ui != null)
            {
                ui.UpdatePowerUp("Disminuir");
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
            if (PowerUpSound != null && sfxMixerGroup != null)
            {
                AudioSource.PlayClipAtPoint(PowerUpSound, transform.position, 1.0f);
            }

            UIManager ui = FindObjectOfType<UIManager>();
            if (ui != null)
            {
                ui.UpdatePowerUp("PowerBall");
            }

            BallMov ballScript = ball.GetComponent<BallMov>();
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

            UIManager ui = FindObjectOfType<UIManager>();
            if (ui != null)
            {
                ui.UpdatePowerUp("ExtraBall");
            }

            GameObject[] balls = GameObject.FindGameObjectsWithTag("Ball");

            foreach (GameObject b in balls)
            {
                Rigidbody rb = b.GetComponent<Rigidbody>();
                BallMov ballMov = b.GetComponent<BallMov>();

                bool isPowerBall = ballMov.isPowerBall;
                bool isIman = ballMov.isImant;
                bool godMode = ballMov.godMode;

                if (rb != null && ballMov != null)
                {
                    Vector3 dir = rb.linearVelocity.normalized;
                    float speed = ballMov.speed;

                    // Crea dues noves boles amb angles diferents
                    //CreateExtraBall(b.transform.position, Quaternion.Euler(0, 0, 0) * dir, speed);
                    CreateExtraBall(b.transform.position, Quaternion.Euler(0, 20, 0) * dir, speed, isPowerBall, isIman, godMode);
                    CreateExtraBall(b.transform.position, Quaternion.Euler(0, -20, 0) * dir, speed, isPowerBall, isIman, godMode);
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

            UIManager ui = FindObjectOfType<UIManager>();
            if (ui != null)
            {
                ui.UpdatePowerUp("Imant");
            }

            rend.material = imantMaterial; // Canvia el material de la pala a l'imant

            GameObject[] balls = GameObject.FindGameObjectsWithTag("Ball");

            foreach (GameObject b in balls)
            {
                BallMov ballScript = b.GetComponent<BallMov>();
                if (ballScript != null)
                {
                    ballScript.ActivateImant();
                }
            }
            PalaMov palaScript = GetComponent<PalaMov>();
            if (palaScript != null)
            {
                palaScript.ActivateImantVisual();
            }

            Destroy(other.gameObject);
        }
        else if (other.CompareTag("NormalBall"))
        {
            if (PowerUpSound != null && sfxMixerGroup != null)
            {
                AudioSource.PlayClipAtPoint(PowerUpSound, transform.position, 1.0f);
            }

            UIManager ui = FindObjectOfType<UIManager>();
            if (ui != null)
            {
                ui.UpdatePowerUp("NormalBall");
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

            UIManager ui = FindObjectOfType<UIManager>();
            if (ui != null)
            {
                ui.UpdatePowerUp("FastBall");
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

            UIManager ui = FindObjectOfType<UIManager>();
            if (ui != null)
            {
                ui.UpdatePowerUp("SlowBall");
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

            UIManager ui = FindObjectOfType<UIManager>();
            if (ui != null)
            {
                ui.UpdatePowerUp("NextLevel");
            }

            // Deixa estàtiques totes les boles (amb el tag "Ball")
            GameObject[] balls = GameObject.FindGameObjectsWithTag("Ball");
            foreach (GameObject ball in balls)
            {
                Rigidbody rb = ball.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                    rb.isKinematic = true; // Opcional: bloqueja la física totalment
                }
                
            }

            Destroy(other.gameObject); // Elimina la clau ara

            FindObjectOfType<UIManager>().ShowNextLevelTextFade();
            StartCoroutine(WaitAndLoadNextLevel(1.5f));
        }

        else if (other.CompareTag("VidaExtra"))
        {
            if (PowerUpSound != null && sfxMixerGroup != null)
            {
                AudioSource.PlayClipAtPoint(PowerUpSound, transform.position, 1.0f);
            }

            UIManager ui = FindObjectOfType<UIManager>();
            if (ui != null)
            {
                ui.UpdatePowerUp("VidaExtra");
            }

            GameManager gm = FindObjectOfType<GameManager>();

            if (gm != null)
            {
                gm.SumaVidas();
            }

            Destroy(other.gameObject);
        }
    }

    IEnumerator WaitAndLoadNextLevel(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        FindObjectOfType<UIManager>().HideNextLevelTextFade();

        string[] sceneOrder = { "Level01", "Level02", "Level03", "Level04", "Level05", "WinScreen" };
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        int index = System.Array.IndexOf(sceneOrder, currentScene);
        if (index >= 0 && index < sceneOrder.Length - 1)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneOrder[index + 1]);
        }
        else
        {
            Debug.LogWarning("Escena actual no trobada o ja a la darrera escena.");
        }
    }

    void CreateExtraBall(Vector3 position, Vector3 direction, float speed, bool isPowerBall, bool isIman, bool godMode)
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
            ballMov.godMode = godMode;

            rb.linearVelocity = ballMov.direccion * speed;

            if (!isPowerBall)
                newBall.GetComponent<Renderer>().material = ballMov.normalMaterial;
            else
                newBall.GetComponent<Renderer>().material = ballMov.powerBallMaterial;
        }
    }


}