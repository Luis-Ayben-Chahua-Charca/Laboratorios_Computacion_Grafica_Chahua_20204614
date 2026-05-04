using UnityEngine;

public class Destructible : MonoBehaviour
{
    public static GameManager instance;
    public bool esPrincipal = false;
    private bool yaContado = false;

    private float inclinacionLimite = 45f; // grados para considerar "caído"

    void Update()
    {
        if (!yaContado)
        {
            float angulo = Vector3.Angle(Vector3.up, transform.up);

            if (angulo > inclinacionLimite)
            {
                yaContado = true;

                if (esPrincipal)
                {
                    GameManager.instance.SumarPrincipal();
                }
                else
                {
                    GameManager.instance.RegistrarSecundario();
                }
            }
        }
    }
}