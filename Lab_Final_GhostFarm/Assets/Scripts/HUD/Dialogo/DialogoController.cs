using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogoController : MonoBehaviour
{
    public static DialogoController Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private GameObject panelDialogo;
    [SerializeField] private Image retratoImagen;
    [SerializeField] private TMP_Text nombreTexto;
    [SerializeField] private TMP_Text cuerpoTexto;
    [SerializeField] private KeyCode teclaContinuar = KeyCode.Space;

    private DialogoData dialogoActual;
    private int indiceActual;
    private Action alTerminar;
    private bool mostrando = false;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        if (panelDialogo != null) panelDialogo.SetActive(false);
    }

    void Update()
    {
        if (!mostrando) return;
        if (Input.GetKeyDown(teclaContinuar))
            AvanzarLinea();
    }

    // data == null es un caso válido (flashback sin diálogo) — en ese caso
    // se llama al callback inmediatamente, así el llamador (FlashbackTrigger)
    // no necesita chequear null por su cuenta.
    public void MostrarDialogo(DialogoData data, Action callbackAlTerminar)
    {
        if (data == null || data.lineas == null || data.lineas.Length == 0)
        {
            callbackAlTerminar?.Invoke();
            return;
        }

        dialogoActual = data;
        indiceActual = 0;
        alTerminar = callbackAlTerminar;
        mostrando = true;
        panelDialogo.SetActive(true);
        MostrarLineaActual();
    }

    private void MostrarLineaActual()
    {
        var linea = dialogoActual.lineas[indiceActual];
        if (nombreTexto != null) nombreTexto.text = linea.hablante;
        if (cuerpoTexto != null) cuerpoTexto.text = linea.texto;
        if (retratoImagen != null)
        {
            retratoImagen.sprite = linea.retrato;
            retratoImagen.enabled = linea.retrato != null;
        }
    }

    private void AvanzarLinea()
    {
        indiceActual++;
        if (indiceActual >= dialogoActual.lineas.Length)
        {
            mostrando = false;
            panelDialogo.SetActive(false);
            var callback = alTerminar;
            alTerminar = null;
            callback?.Invoke();
            return;
        }
        MostrarLineaActual();
    }
}