using UnityEngine;

// Componente genérico y reusable de "obsesión": cualquier escena con una
// misión secundaria opcional que debe escalar a obligatoria si el jugador
// se aleja sin completarla agrega este componente en el mismo GameObject
// que tiene el Collider de la zona (marcado como Is Trigger).
//
// Reemplaza el patrón que tenías repartido entre CampoAvena.EscalarObsesion()
// y SalidaCampoAvena.OnTriggerExit(). La escena dueña de la misión (ej.
// CampoAvena, y a futuro CampoCerca) solo llama a Activar() cuando arranca
// la secundaria y MarcarCompleta() cuando termina — este componente se
// encarga de detectar la salida y decidir si corresponde escalar.
[RequireComponent(typeof(Collider))]
public class ComponenteObsesion : MonoBehaviour
{
    [SerializeField] private MisionId idMisionSecundaria;
    [SerializeField] private string tagJugador = "Player";

    private bool activa = false;
    private bool completa = false;

    // Llamado por la escena cuando arranca la misión secundaria opcional.
    public void Activar()
    {
        activa = true;
        completa = false;
    }

    // Llamado por la escena cuando la secundaria se termina de forma normal
    // (sin necesidad de escalar). Evita que una salida posterior del área
    // la vuelva a escalar innecesariamente.
    public void MarcarCompleta()
    {
        completa = true;
    }

    // Llamado por la escena cuando la secundaria nunca llegó a empezar (ej.
    // un salto de etapa a un punto anterior de la misma escena). Deja este
    // componente en un estado neutro que nunca escala nada.
    public void Desactivar()
    {
        activa = false;
        completa = false;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(tagJugador)) return;
        if (!activa || completa) return;

        MisionManager.Instance.EscalarAPrincipal(idMisionSecundaria);
    }
}