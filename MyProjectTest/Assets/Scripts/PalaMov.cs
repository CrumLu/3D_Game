using UnityEngine;

public class PalaMov : MonoBehaviour
{
    public float scaleChange = 0.5f;
    public GameObject ball; // assigna la pilota manualment al inspector

    public KeyCode left;
    public KeyCode right;

    public float size;

    enum State
    {
        left,
        right,
        stop
    }

    State state;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
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
            Vector3 scale = transform.localScale;
            scale.x += scaleChange;
            transform.localScale = scale;

            Destroy(other.gameObject);
        }
    }
}
