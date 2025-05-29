using UnityEngine;
using System.Collections;

public class CameraIntroController : MonoBehaviour
{
    public Transform pivot;
    public float introDuration = 5.0f; // Fes-la més llarga per més suavitat
    public int spiralTurns = 1;        // Nombre de voltes completes (ajusta segons vulguis)
    public float startHeightOffset = 30f; // Quant més amunt que la final (prova amb 25-40)
    public float startRadiusOffset = 20f; // Distància extra (a XZ) respecte la final

    public Vector3 finalPosition = new Vector3(0.0307231f, 17.9938f, -10.6936f);
    public Vector3 finalEulerAngles = new Vector3(47.327f, -0.527f, 0.062f);


    public bool introFinished = false;

    void Start()
    {
        StartCoroutine(SpiralIntro());
    }

    IEnumerator SpiralIntro()
    {
        float timer = 0f;
        Vector3 center = pivot.position;

        // Calcula radi i angle finals (en XZ) respecte al pivot
        Vector3 deltaFinal = finalPosition - center;
        Vector2 xzFinal = new Vector2(deltaFinal.x, deltaFinal.z);
        float angleFinal = Mathf.Atan2(xzFinal.x, xzFinal.y); // radians
        float radiusFinal = xzFinal.magnitude;
        float yFinal = finalPosition.y;

        // Posa càmera al punt inicial de la corba (a dalt i més lluny, però alineada amb angle)
        float radiusStart = radiusFinal + startRadiusOffset;
        float heightStart = yFinal + startHeightOffset;
        float angleStart = angleFinal - spiralTurns * 2 * Mathf.PI;

        while (timer < introDuration)
        {
            float t = timer / introDuration;
            // Interpola angle de angleStart a angleFinal (fa les voltes)
            float angle = Mathf.Lerp(angleStart, angleFinal, t);
            // Interpola radi i altura
            float radius = Mathf.Lerp(radiusStart, radiusFinal, t);
            float y = Mathf.Lerp(heightStart, yFinal, t);

            // Calcula posició al voltant del pivot
            float x = center.x + Mathf.Sin(angle) * radius;
            float z = center.z + Mathf.Cos(angle) * radius;

            transform.position = new Vector3(x, y, z);

            // Rotació: interpolació suau entre mirar el centre i la rotació final
            if (t < 0.92f)
            {
                transform.rotation = Quaternion.LookRotation(center - transform.position);
            }
            else
            {
                // Fes transició suau només en l’últim 8% cap a la rotació final exacte
                float t2 = Mathf.InverseLerp(0.92f, 1.0f, t);
                Quaternion look = Quaternion.LookRotation(center - transform.position);
                Quaternion finalRot = Quaternion.Euler(finalEulerAngles);
                transform.rotation = Quaternion.Slerp(look, finalRot, t2);
            }

            timer += Time.deltaTime;
            yield return null;
        }

        // Corregeix exactament a la posició i rotació finals
        transform.position = finalPosition;
        transform.rotation = Quaternion.Euler(finalEulerAngles);

        introFinished = true;
    }
}
