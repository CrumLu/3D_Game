using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject ballPrefab; // Assigna el prefab de la bola a l’inspector
    public GameObject pala;       // Assigna el GameObject de la pala a l’inspector

    public static GameManager instance;
    public int vides = 3;

    private bool LevelCompleted = false;
    private string[] SceneOrder = { "Level01", "Level02", "Level03", "Level04", "Level05", "WinScreen" };


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    // Cridat per BallMov quan una bola cau
    public void OnBallLost(GameObject ball)
    {
        ball.SetActive(false);

        int ballsActives = 0;
        GameObject[] balls = GameObject.FindGameObjectsWithTag("Ball");
        foreach (GameObject b in balls)
            if (b.activeInHierarchy) ballsActives++;

        if (ballsActives == 0)
        {
            vides--;
            //UpdateLivesUI();
            UIManager ui = FindObjectOfType<UIManager>();
            if (ui != null)
            {
                ui.UpdateLives(vides-1); // Actualitza la UI de vides
            }
            else
            {
                Debug.LogWarning("No s'ha trobat UIManager per actualitzar les vides.");
            }

            if (vides <= 0)
            {
                SceneManager.LoadScene("GameOver");
                vides = 3;
                if (ui != null)
                {
                    ui.UpdateLives(vides - 1); // Actualitza la UI de vides
                }
                else
                {
                    Debug.LogWarning("No s'ha trobat UIManager per actualitzar les vides.");
                }
            }
            else
                RespawnBall();
        }
    }

    void RespawnBall()
    {
        // Trobar la pala al joc per saber on col·locar la nova bola
        GameObject paddle = GameObject.FindGameObjectWithTag("Pala");
        Vector3 spawnPos = paddle.transform.position + new Vector3(0, -0.4f, 0.75f);
        // ^ Offset similar al que fa servir BallMov per posicionar la bola sobre la pala

        // Instanciar una nova bola a partir del prefab
        GameObject newBall = Instantiate(ballPrefab, spawnPos, Quaternion.identity);
        newBall.tag = "Ball";  // Assegura el tag correcte (si no ve ja configurat al prefab)

        // Inicialitzar l'estat de la nova bola
        BallMov ballScript = newBall.GetComponent<BallMov>();
        ballScript.isLaunched = false;   // Encara no llançada (queda enganxada a la pala)
        ballScript.isExtraBall = false;  // És una bola normal, no una extra generada per power-up
        ballScript.isImant = false;      // Power-up imant desactivat
        ballScript.isPowerBall = false;  // Power-up power-ball desactivat
        ballScript.speed = 7.0f;         // Velocitat base (assegurar que torna a la normalitat)
                                         // (Altres propietats com vides o materials es poden inicialitzar segons convingui)

        // (Opcional) Actualitzar referències al GameManager o PalaMov si cal
        PalaMov paddleScript = paddle.GetComponent<PalaMov>();
        if (paddleScript != null)
        {
            paddleScript.ball = ballPrefab;  // Assegura que la referència segueix apuntant al prefab
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) 
        {
            SceneManager.LoadScene("Level01");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SceneManager.LoadScene("Level02");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SceneManager.LoadScene("Level03");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SceneManager.LoadScene("Level04");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SceneManager.LoadScene("Level05");
        }

        GameObject[] bricks = GameObject.FindGameObjectsWithTag("Brick");
        Debug.Log("Bloques restantes: " + bricks.Length);

        if (!LevelCompleted && bricks.Length == 0)
        {
            LevelCompleted = true;
            Invoke("LoadNextLevel", 1.5f);
        }
    }

    void LoadNextLevel()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        int index = System.Array.IndexOf(SceneOrder, currentScene);
        if (index >= 0 && index < SceneOrder.Length - 1)
        {
            SceneManager.LoadScene(SceneOrder[index + 1]);
        }
        else
        {
            Debug.Log("Última escena alcanzada o no encontrada.");
        }
    }
}