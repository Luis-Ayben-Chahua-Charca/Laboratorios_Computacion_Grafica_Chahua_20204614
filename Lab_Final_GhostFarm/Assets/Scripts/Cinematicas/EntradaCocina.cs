using UnityEngine;

// Trigger simple en la puerta/umbral de la cocina. No orquesta nada de la
// cinemática en sí — solo avisa que el jugador llegó, vía el mismo patrón de
// eventos que ya usás en todos lados (OnObjetivoCompletado). CinematicaCocina
// escucha este evento y arranca la escena guionada.
public class EntradaCocina : MonoBehaviour
{
    private bool disparado = false;

    private void OnTriggerEnter(Collider other)
    {
        if (disparado || !other.CompareTag("Player")) return;
        disparado = true;
        MisionManager.Instance.CompletarObjetivo(MisionId.IrCocina);
    }
}