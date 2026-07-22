using UnityEngine;
using UnityEngine.UI;

public class CrosshairUI : MonoBehaviour
{
    [SerializeField] private Interactor interactor;
    [SerializeField] private CanvasGroup grupo;
    [SerializeField] private Image imagenCrosshair;
    [SerializeField] private Color colorNormal = Color.white;
    [SerializeField] private Color colorInteractuable = Color.yellow;
    [SerializeField] private float tiempoOculto = 3f;

    private float tiempoSinDeteccion = 0f;
    private bool forzadoOculto = false;

    void Update()
    {
        if (forzadoOculto) return;

        bool hayInteractuable = interactor.Actual != null;
        imagenCrosshair.color = hayInteractuable ? colorInteractuable : colorNormal;
        tiempoSinDeteccion = hayInteractuable ? 0f : tiempoSinDeteccion + Time.deltaTime;
        grupo.alpha = tiempoSinDeteccion >= tiempoOculto ? 0f : 1f;
    }

    public void ForzarOculto(bool ocultar)
    {
        forzadoOculto = ocultar;
        if (ocultar) grupo.alpha = 0f;
    }
}