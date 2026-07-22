using TMPro;
using UnityEngine;
public class HUDController : MonoBehaviour
{
    [SerializeField] private CanvasGroup panelMisiones;
    [SerializeField] private CanvasGroup panelItem;
    [SerializeField] private float duracionFade = 0.3f;
    [SerializeField] private CrosshairUI crosshair;
    [SerializeField] private TMP_Text textoMision;
    [SerializeField] private TMP_Text textoItem;
    public void ActualizarItem(string nombre, int cantidad) => textoItem.text = $"{nombre}: {cantidad}";

    public void ActualizarTextoMision(string texto) => textoMision.text = texto;
    public void SetMisionesVisible(bool visible) => StartCoroutine(Fade(panelMisiones, visible));
    public void SetItemVisible(bool visible) => StartCoroutine(Fade(panelItem, visible));
    public void SetCrosshairForzado(bool ocultar) => crosshair.ForzarOculto(ocultar);

    private System.Collections.IEnumerator Fade(CanvasGroup grupo, bool visible)
    {
        float inicio = grupo.alpha;
        float fin = visible ? 1f : 0f;
        float t = 0;
        while (t < duracionFade)
        {
            t += Time.deltaTime;
            grupo.alpha = Mathf.Lerp(inicio, fin, t / duracionFade);
            yield return null;
        }
        grupo.alpha = fin;
    }
}