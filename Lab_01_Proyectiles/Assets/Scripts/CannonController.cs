using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class CannonController : MonoBehaviour
{
    // VARIABLES AQUÍ (dentro de la clase)
    public Transform baseCannon;
    public Transform barrel;

    public float rotationSpeed = 1500f;

    public float BlastPower = 5;

    public GameObject Cannonball;
    public Transform ShotPoint;

    private float verticalAngle = 0f;
    public float minAngle = 0;
    public float maxAngle = 40;

    private float horizontalAngle = 0f;
    public float minHorizontal = -60f;
    public float maxHorizontal = 60f;

    [Header("Audio Settings")]
    [SerializeField] public AudioSource disparador;
    public AudioClip disparo;

    [Header("Disparos Disponibles")]
    [SerializeField] public int ProyectilesDisponibles = 4;



    //  Declarar correctamente el evento como miembro de la clase
    public System.Action onSinDisparos;

    void Update()
    {
        Rotar();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (ProyectilesDisponibles > 0)
            {
                StartCoroutine(Disparar());

            } 
        }
    }

    private IEnumerator Disparar()
    {
        disparador.PlayOneShot(disparo);

        yield return new WaitForSeconds(0.3f);

        GameObject ball = Instantiate(Cannonball, ShotPoint.position, ShotPoint.rotation);
        Rigidbody rb = ball.GetComponent<Rigidbody>();
        rb.linearVelocity = ShotPoint.up * BlastPower;

        ProyectilesDisponibles--;

        if (ProyectilesDisponibles <= 0)
        {
            onSinDisparos?.Invoke();
        }
    }


    private void Rotar()
    {
        //  ROTACIÓN HORIZONTAL (base del cañón)
        if (Input.GetKey(KeyCode.A))
        {
            horizontalAngle -= rotationSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.D))
        {
            horizontalAngle += rotationSpeed * Time.deltaTime;
        }

        // Limitar ángulo horizontal
        horizontalAngle = Mathf.Clamp(horizontalAngle, minHorizontal, maxHorizontal);

        // Aplicar rotación absoluta
        baseCannon.localRotation = Quaternion.Euler(0, horizontalAngle, 0);

        //  ROTACIÓN VERTICAL (tubo del cañón)
        if (Input.GetKey(KeyCode.W))
        {
            verticalAngle -= rotationSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.S))
        {
            verticalAngle += rotationSpeed * Time.deltaTime;
        }

        // Limitar ángulo
        verticalAngle = Mathf.Clamp(verticalAngle, minAngle, maxAngle);

        // Aplicar rotación al tubo
        barrel.localRotation = Quaternion.Euler(verticalAngle, 0, 0);
    }

}