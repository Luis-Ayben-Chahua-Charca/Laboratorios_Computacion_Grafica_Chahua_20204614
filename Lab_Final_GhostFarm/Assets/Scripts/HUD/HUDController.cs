using UnityEngine;
public class HUDController : MonoBehaviour
{
    [SerializeField] private CanvasGroup panelMisiones;
    [SerializeField] private CanvasGroup panelItem;
    [SerializeField] private float duracionFade = 0.3f;

    public void SetMisionesVisible(bool visible) => StartCoroutine(Fade(panelMisiones, visible));
    public void SetItemVisible(bool visible) => StartCoroutine(Fade(panelItem, visible));

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