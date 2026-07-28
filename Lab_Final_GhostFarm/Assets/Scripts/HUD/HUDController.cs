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

    private Dictionary<string, GameObject> filas = new Dictionary<string, GameObject>();

    private Dictionary<string, SlotItemUI> slotsActivos = new Dictionary<string, SlotItemUI>();

    // FIX: filas que están actualmente desvaneciéndose (fade-out tras completarse).
    // RenderizarPrincipales no debe destruirlas de golpe: si lo hace, la corrutina
    // DesvanecerYQuitar se despierta después con una referencia ya destruida y
    // tira MissingReferenceException.
    private HashSet<GameObject> filasEnDesvanecimiento = new HashSet<GameObject>();

    void Awake() => ActualizarVisibilidadOpcional();

    // FIX: la etiqueta "Opcional" solo debe verse mientras haya al menos una
    // misión secundaria activa. Se llama cada vez que contenedorSecundarios
    // gana o pierde un hijo (agregar, escalar a principal, o desvanecerse tras completarse).
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
        // FIX: antes esto destruía TODOS los hijos de contenedorPrincipales sin
        // excepción, incluida cualquier fila que en ese mismo frame estuviera
        // en medio de su fade-out de "completado" (ver filasEnDesvanecimiento).
        // Esas filas se autodestruyen solas al terminar su corrutina.
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

    // NUEVO: simétrico a RenderizarPrincipales, pero para la lista de
    // secundarios. Usado por MisionManager.ForzarEstado (salto de etapas) —
    // el flujo normal del juego sigue usando AgregarFilaSecundaria de a una.
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

    public void MarcarCompletado(string id, bool esSecundario)
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

        // FIX: si otra cosa ya destruyó esta fila mientras esperábamos
        // (por ejemplo un RenderizarPrincipales de una versión vieja del script,
        // o cualquier otro camino futuro), salimos sin tocar nada en vez de
        // tirar MissingReferenceException.
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
            var entrada = filas.FirstOrDefault(kv => kv.Value == fila);
            if (entrada.Key != null) filas.Remove(entrada.Key);
            Destroy(fila);

            if (eraSecundaria)
            {
                yield return null; // esperar a que Destroy() se aplique antes de contar los hijos restantes
                ActualizarVisibilidadOpcional();
            }
        }
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

    public void EscalarFilaAPrincipal(string id)
    {
        if (!filas.TryGetValue(id, out GameObject fila)) return;
        fila.transform.SetParent(contenedorPrincipales, false);
        fila.GetComponentInChildren<TMP_Text>().color = Color.red;
        ActualizarVisibilidadOpcional();
    }
}