using UnityEngine;

// Usa el estado Cinematica de SceneDirector (ya existía, nunca se había
// usado) — a diferencia de un Flashback, acá el jugador no tiene NINGÚN
// control, es una escena 100% guionada con cámara fija.
public class CinematicaCocina : MonoBehaviour
{
    [Header("Cámaras (corte directo, sin blend — no hace falta Cinemachine)")]
    [SerializeField] private Camera camaraCinematica;
    [SerializeField] private Camera camaraJugador;

    [Header("Personajes (Animator ya existente en cada modelo)")]
    [SerializeField] private Animator animatorAbuela;
    [SerializeField] private Animator animatorAbuelo;
    [SerializeField] private string estadoAbuela = "Armature|Servir";
    [SerializeField] private string estadoAbuelo = "Armature|Esperar";

    [Header("Duración y diálogo final")]
    [SerializeField] private float duracion = 8f;
    [SerializeField] private DialogoData dialogoFinal;

    private bool activa = false;

    void OnEnable() => MisionManager.OnObjetivoCompletado += HandleObjetivoCompletado;
    void OnDisable() => MisionManager.OnObjetivoCompletado -= HandleObjetivoCompletado;

    void HandleObjetivoCompletado(MisionId id)
    {
        if (id == MisionId.IrCocina) Disparar();
    }

    public void Disparar()
    {
        if (activa) return;
        activa = true;

        SceneDirector.Instance.IniciarCinematica();
        VisualModeController.Instance.EntrarModoMemoria(false);
        CambiarCamara(usarCinematica: true);

        if (animatorAbuela != null) animatorAbuela.Play(estadoAbuela);
        if (animatorAbuelo != null) animatorAbuelo.Play(estadoAbuelo);

        Invoke(nameof(Finalizar), duracion);
    }

    private void Finalizar()
    {
        // NUEVO: el cambio a Noche se hace ANTES de salir del modo recuerdo,
        // mismo truco que en CercaRota — así el blend de luz va directo de
        // "memoria" a "noche", sin pasar por la condición anterior del día.
        SkyboxController.Instance.SetCondicion(CondicionCielo.Noche);
        VisualModeController.Instance.SalirModoMemoria(false);
        CambiarCamara(usarCinematica: false);

        DialogoController.Instance.MostrarDialogo(dialogoFinal, TerminarCinematica);
    }

    private void TerminarCinematica()
    {
        SceneDirector.Instance.TerminarEvento();
        activa = false;
    }

    private void CambiarCamara(bool usarCinematica)
    {
        // FIX: solo se togglea el componente Camera, no el AudioListener —
        // dejá el AudioListener únicamente en la cámara del jugador (no le
        // pongas uno a la cámara de la cinemática), para que Unity nunca
        // tenga dos AudioListener activos a la vez.
        if (camaraCinematica != null) camaraCinematica.enabled = usarCinematica;
        if (camaraJugador != null) camaraJugador.enabled = !usarCinematica;
    }
}