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
    [SerializeField] private GameObject etiquetaOpcional; // el texto/encabezado "Opcional" del HUD

    [Header("Items")]
    [SerializeField] private Transform contenedorItems;
    [SerializeField] private GameObject slotItemPrefab;
    [SerializeField] private IconoRecurso[] iconosRecursos;

    [Header("Crosshair")]
    [SerializeField] private CrosshairUI crosshair;

    // FIX: antes era Dictionary<string, GameObject>. Con MisionId como enum,
    // la clave ahora es MisionId — más seguro, sin typos posibles.
    private Dictionary<MisionId, GameObject> filas = new Dictionary<MisionId, GameObject>();

    private Dictionary<TipoRecurso, SlotItemUI> slotsActivos = new Dictionary<TipoRecurso, SlotItemUI>();

    private HashSet<GameObject> filasEnDesvanecimiento = new HashSet<GameObject>();

    void Awake() => ActualizarVisibilidadOpcional();

    private void ActualizarVisibilidadOpcional()
    {
        if (etiquetaOpcional != null)
            etiquetaOpcional.SetActive(contenedorSecundarios.childCount > 0);
    }

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
        foreach (Transform hijo in contenedorPrincipales)
        {
            if (filasEnDesvanecimiento.Contains(hijo.gameObject)) continue;
            Destroy(hijo.gameObject);
        }

        foreach (var obj in objetivos)
        {
            var fila = Instantiate(filaObjetivoPrefab, contenedorPrincipales);
            fila.GetComponentInChildren<TMP_Text>().text = $"- {obj.descripcion}";
            filas[obj.id] = fila;
        }
    }

    public void RenderizarSecundarios(List<Objetivo> objetivos)
    {
        foreach (Transform hijo in contenedorSecundarios)
        {
            if (filasEnDesvanecimiento.Contains(hijo.gameObject)) continue;
            Destroy(hijo.gameObject);
        }

        foreach (var obj in objetivos)
        {
            var fila = Instantiate(filaObjetivoPrefab, contenedorSecundarios);
            fila.GetComponentInChildren<TMP_Text>().text = $"- {obj.descripcion}";
            filas[obj.id] = fila;
        }

        ActualizarVisibilidadOpcional();
    }

    public void AgregarFilaSecundaria(Objetivo obj)
    {
        var fila = Instantiate(filaObjetivoPrefab, contenedorSecundarios);
        fila.GetComponentInChildren<TMP_Text>().text = $"- {obj.descripcion}";
        filas[obj.id] = fila;
        ActualizarVisibilidadOpcional();
    }

    public void MarcarCompletado(MisionId id, bool esSecundario)
    {
        if (!filas.TryGetValue(id, out GameObject fila)) return;
        var texto = fila.GetComponentInChildren<TMP_Text>();
        texto.text = $"<s>{texto.text}</s>";
        filasEnDesvanecimiento.Add(fila);
        StartCoroutine(DesvanecerYQuitar(fila));
    }

    private IEnumerator DesvanecerYQuitar(GameObject fila)
    {
        yield return new WaitForSeconds(1f);

        if (fila == null) { filasEnDesvanecimiento.Remove(fila); yield break; }

        var grupo = fila.GetComponent<CanvasGroup>();
        if (grupo == null) { filasEnDesvanecimiento.Remove(fila); yield break; }

        float t = 0;
        while (t < 0.5f)
        {
            if (fila == null || grupo == null) { filasEnDesvanecimiento.Remove(fila); yield break; }
            t += Time.deltaTime;
            grupo.alpha = 1 - (t / 0.5f);
            yield return null;
        }

        filasEnDesvanecimiento.Remove(fila);

        if (fila != null)
        {
            bool eraSecundaria = fila.transform.parent == contenedorSecundarios;

            // FIX: antes se chequeaba "entrada.Key != null" para saber si
            // FirstOrDefault había encontrado algo. Eso funcionaba con string
            // (que puede ser null), pero MisionId es un enum — un tipo por
            // valor que nunca es null, así que esa comparación ya no
            // compila. Ahora se chequea "entrada.Value != null": si
            // FirstOrDefault no encontró ninguna entrada que apunte a esta
            // fila, el KeyValuePair por defecto tiene Value = null
            // (GameObject sí es un tipo por referencia), así que sigue
            // siendo una forma válida de detectar "no se encontró".
            var entrada = filas.FirstOrDefault(kv => kv.Value == fila);
            if (entrada.Value != null) filas.Remove(entrada.Key);
            Destroy(fila);

            if (eraSecundaria)
            {
                yield return null;
                ActualizarVisibilidadOpcional();
            }
        }
    }

    // ---------- Items ----------

    public void ActualizarItem(TipoRecurso tipo, int cantidad)
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

    public void ActualizarTextoFila(MisionId id, string texto)
    {
        if (filas.TryGetValue(id, out GameObject fila))
            fila.GetComponentInChildren<TMP_Text>().text = $"- {texto}";
    }

    public void EscalarFilaAPrincipal(MisionId id)
    {
        if (!filas.TryGetValue(id, out GameObject fila)) return;
        fila.transform.SetParent(contenedorPrincipales, false);
        fila.GetComponentInChildren<TMP_Text>().color = Color.red;
        ActualizarVisibilidadOpcional();
    }
}