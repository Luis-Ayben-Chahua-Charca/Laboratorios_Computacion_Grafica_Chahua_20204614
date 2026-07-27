using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class HUDController : MonoBehaviour
{
    [Header("Paneles generales")]
    [SerializeField] private CanvasGroup panelMisiones;
    [SerializeField] private CanvasGroup panelItem;
    [SerializeField] private float duracionFade = 0.3f;

    [Header("Misiones")]
    [SerializeField] private Transform contenedorPrincipales;
    [SerializeField] private Transform contenedorSecundarios;
    [SerializeField] private GameObject filaObjetivoPrefab;

    [Header("Items")]
    [SerializeField] private Transform contenedorItems;
    [SerializeField] private GameObject slotItemPrefab;
    [SerializeField] private IconoRecurso[] iconosRecursos;

    [Header("Crosshair")]
    [SerializeField] private CrosshairUI crosshair;

    private Dictionary<string, GameObject> filas = new Dictionary<string, GameObject>();

    private Dictionary<string, SlotItemUI> slotsActivos = new Dictionary<string, SlotItemUI>();


    // ---------- Paneles (fade) ----------

    public void SetMisionesVisible(bool visible) => StartCoroutine(Fade(panelMisiones, visible));
    public void SetItemVisible(bool visible) => StartCoroutine(Fade(panelItem, visible));

    private IEnumerator Fade(CanvasGroup grupo, bool visible)
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

    // ---------- Misiones (objetivos) ----------

    public void RenderizarPrincipales(List<Objetivo> objetivos)
    {
        foreach (Transform hijo in contenedorPrincipales) Destroy(hijo.gameObject);
        foreach (var obj in objetivos)
        {
            var fila = Instantiate(filaObjetivoPrefab, contenedorPrincipales);
            fila.GetComponentInChildren<TMP_Text>().text = $"- {obj.descripcion}";
            filas[obj.id] = fila;
        }
    }

    public void AgregarFilaSecundaria(Objetivo obj)
    {
        var fila = Instantiate(filaObjetivoPrefab, contenedorSecundarios);
        fila.GetComponentInChildren<TMP_Text>().text = $"- {obj.descripcion}";
        filas[obj.id] = fila;
    }

    public void MarcarCompletado(string id, bool esSecundario)
    {
        if (!filas.TryGetValue(id, out GameObject fila)) return;
        var texto = fila.GetComponentInChildren<TMP_Text>();
        texto.text = $"<s>{texto.text}</s>";
        StartCoroutine(DesvanecerYQuitar(fila));
    }

    private IEnumerator DesvanecerYQuitar(GameObject fila)
    {
        yield return new WaitForSeconds(1f);
        var grupo = fila.GetComponent<CanvasGroup>();
        float t = 0;
        while (t < 0.5f)
        {
            t += Time.deltaTime;
            grupo.alpha = 1 - (t / 0.5f);
            yield return null;
        }
        filas.Remove(filas.FirstOrDefault(kv => kv.Value == fila).Key);
        Destroy(fila);
    }

    // ---------- Items ----------

    public void ActualizarItem(string tipo, int cantidad)
    {
        if (slotItemPrefab == null) { Debug.LogError("HUDController: falta asignar slotItemPrefab"); return; }
        if (contenedorItems == null) { Debug.LogError("HUDController: falta asignar contenedorItems"); return; }

        if (cantidad <= 0)
        {
            if (slotsActivos.TryGetValue(tipo, out var slotExistente))
            {
                Destroy(slotExistente.gameObject);
                slotsActivos.Remove(tipo);
            }
            return;
        }

        if (!slotsActivos.TryGetValue(tipo, out var slot))
        {
            var obj = Instantiate(slotItemPrefab, contenedorItems);
            slot = obj.GetComponent<SlotItemUI>();
            if (slot == null) { Debug.LogError("El prefab instanciado no tiene el componente SlotItemUI en su raíz", obj); return; }
            slotsActivos[tipo] = slot;
        }

        var data = System.Array.Find(iconosRecursos, i => i.tipo == tipo);
        if (data == null) Debug.LogWarning($"No hay ícono configurado en iconosRecursos para el tipo '{tipo}'");

        slot.SetData(data?.icono, cantidad);
    }

    // ---------- Crosshair ----------

    public void SetCrosshairForzado(bool ocultar) => crosshair.ForzarOculto(ocultar);

    public void ActualizarTextoFila(string id, string texto)
    {
        if (filas.TryGetValue(id, out GameObject fila))
            fila.GetComponentInChildren<TMP_Text>().text = $"- {texto}";
    }

}