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

        // NUEVO: si este flashback tiene diálogo asignado, se muestra antes
        // de arrancar el temporizador de salida — así el jugador no pierde
        // tiempo de "salidaPorTiempo" leyendo el diálogo. Si no hay diálogo,
        // DialogoController llama al callback de inmediato (ver DialogoController).
        DialogoController.Instance.MostrarDialogo(data.dialogoInicial, ContinuarTrasDialogo);
    }

    private void ContinuarTrasDialogo()
    {
        // Guard: si Finalizar() ya se llamó externamente mientras el diálogo
        // estaba en pantalla (poco probable, pero posible a futuro), no
        // arranquemos un temporizador sobre un flashback que ya terminó.
        if (!activo) return;

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