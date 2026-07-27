using System.Collections;
using UnityEngine;

public class FlashbackTrigger : MonoBehaviour
{
    [SerializeField] private FlashbackData data;
    private bool activo = false;

    public void Disparar()
    {
        if (activo) return;
        activo = true;
        SceneDirector.Instance.IniciarFlashback(data.mantenerControlJugador);
        VisualModeController.Instance.EntrarModoMemoria(false);

        if (!data.salidaManual)
            StartCoroutine(SalidaPorTiempo());
    }

    private IEnumerator SalidaPorTiempo()
    {
        yield return new WaitForSeconds(data.duracion);
        Finalizar();
    }

    public void Finalizar()
    {
        if (!activo) return;
        activo = false;
        StopAllCoroutines();
        VisualModeController.Instance.SalirModoMemoria(false);
        SceneDirector.Instance.TerminarEvento();
    }
}