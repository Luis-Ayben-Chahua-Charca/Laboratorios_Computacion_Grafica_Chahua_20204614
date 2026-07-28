using UnityEngine;

// Hace que un animal (vaca, caballo, etc.) deambule dentro de los límites
// de una zona, eligiendo puntos al azar y caminando hacia ellos, alternando
// entre sus animaciones de Idle y Walk/WalkSlow ya existentes.
//
// A diferencia de SalidaCampoAvena/ComponenteObsesion, esto NO necesita
// eventos de trigger — solo lee zonaCorral.bounds (las coordenadas del
// collider) para saber dentro de qué rango puede elegir un destino. El
// collider de la zona puede ser trigger o no, da igual para este script.
public class AnimalDeambulante : MonoBehaviour
{
    [Header("Zona y movimiento")]
    [SerializeField] private Collider zonaCorral;
    [SerializeField] private float velocidad = 1.2f;
    [SerializeField] private float velocidadRotacion = 4f;
    [SerializeField] private float distanciaLlegada = 0.3f;

    [Header("Pausas entre caminatas")]
    [SerializeField] private float esperaMin = 2f;
    [SerializeField] private float esperaMax = 6f;

    [Header("Animator (nombres de estado, no de parámetro)")]
    [SerializeField] private Animator animator;
    [SerializeField] private string estadoIdle = "Armature|Idle";
    [SerializeField] private string estadoCaminar = "Armature|WalkSlow";

    private Vector3 destino;
    private float esperaRestante;
    private bool esperando = true;

    void Start()
    {
        if (zonaCorral == null) { Debug.LogError("AnimalDeambulante: falta asignar zonaCorral", this); enabled = false; return; }
        if (animator == null) { Debug.LogError("AnimalDeambulante: falta asignar animator", this); enabled = false; return; }
        ElegirNuevoDestino();
        EmpezarEspera();
    }

    void Update()
    {
        if (esperando)
        {
            esperaRestante -= Time.deltaTime;
            if (esperaRestante <= 0f)
            {
                esperando = false;
                animator.Play(estadoCaminar);
            }
            return;
        }

        Vector3 direccion = destino - transform.position;
        direccion.y = 0f;

        if (direccion.magnitude <= distanciaLlegada)
        {
            ElegirNuevoDestino();
            EmpezarEspera();
            return;
        }

        transform.position += direccion.normalized * velocidad * Time.deltaTime;

        Quaternion rotacionObjetivo = Quaternion.LookRotation(direccion);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotacionObjetivo, velocidadRotacion * Time.deltaTime);
    }

    private void EmpezarEspera()
    {
        esperando = true;
        esperaRestante = Random.Range(esperaMin, esperaMax);
        animator.Play(estadoIdle);
    }

    private void ElegirNuevoDestino()
    {
        Bounds b = zonaCorral.bounds;
        float x = Random.Range(b.min.x, b.max.x);
        float z = Random.Range(b.min.z, b.max.z);
        destino = new Vector3(x, transform.position.y, z);
    }
}