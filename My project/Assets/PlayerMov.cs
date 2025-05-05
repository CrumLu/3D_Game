using UnityEngine;

public class PlayerMov : MonoBehaviour
{
    public Rigidbody rb;

    public float forwardForce = 2000f;

    public float sidewaysForce = 500f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("PlayerMov script started");

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Movimiento adelante constante
        rb.AddForce(0, 0, forwardForce * Time.deltaTime);

        // Movimiento lateral derecho con más fuerza para probar
        if (Input.GetKey(KeyCode.D))
        {
            rb.AddForce(sidewaysForce * Time.deltaTime, 0, 0); // Ajusta si ahora funciona
        }

        if (Input.GetKey(KeyCode.A))
        {
            rb.AddForce(-sidewaysForce * Time.deltaTime, 0, 0); // Ajusta si ahora funciona
        }
    }

}