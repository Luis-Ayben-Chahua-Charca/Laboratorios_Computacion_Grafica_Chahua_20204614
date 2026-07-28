using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StageLoader : MonoBehaviour
{
    public static StageLoader Instance { get; private set; }

    [Header("Etapas disponibles (F1 = etapas[0], F2 = etapas[1], ...)")]
    [SerializeField] private StageData[] etapas;

    [Header("Referencias de escena")]
    [SerializeField] private CharacterController controladorJugador;
    [SerializeField] private Transform jugador;
    [SerializeField] private SkyboxController skyboxController;
    [SerializeField] private ManoJugador manoJugador;
    [SerializeField] private ObjetoAgarrable hozEnEscena;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Update()
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        for (int i = 0; i < etapas.Length && i < 9; i++)
        {
            if (Input.GetKeyDown(KeyCode.F1 + i))
                CargarEtapa(etapas[i]);
        }
#endif
    }

    public void CargarEtapa(StageData etapa)
    {
        if (etapa == null) return;

        TeleportarJugador(etapa.posicionJugador, etapa.rotacionJugadorEuler);

        skyboxController.SetCondicion(etapa.condicionCielo);

        var principales = etapa.misionesPrincipales
            .Select(o => new Objetivo { id = o.id, descripcion = o.descripcion })
            .ToList();
        var secundarias = etapa.misionesSecundarias
            .Select(o => new Objetivo { id = o.id, descripcion = o.descripcion })
            .ToList();
        MisionManager.Instance.ForzarEstado(principales, secundarias);

        foreach (var item in etapa.itemsIniciales)
            InventarioRecursos.Instance.ForzarCantidad(item.tipo, item.cantidad);

        // FIX: antes solo se sabía EQUIPAR la hoz (jugadorTieneHoz = true).
        // Si veníamos de otra etapa donde el jugador la tenía en mano y
        // saltábamos a una etapa con jugadorTieneHoz = false, se quedaba
        // pegada a la cámara para siempre — nada la soltaba. Ahora el "false"
        // también se maneja explícitamente.
        if (etapa.avena.jugadorTieneHoz)
        {
            if (hozEnEscena != null) manoJugador.EquiparSilencioso(hozEnEscena);
        }
        else
        {
            manoJugador.SoltarSilencioso();
        }

        // FIX: antes esto llamaba a CampoAvena.Instance.ForzarEstadoDebug(...)
        // por nombre, con 3 parámetros sueltos que había que mantener
        // sincronizados con el StageData a mano. Ahora se busca cualquier
        // componente que implemente IEstadoDebug (sin importar de qué clase
        // sea) y se le pasa la etapa completa — cada sistema de escena nuevo
        // (cerca, cocina, lámpara...) solo necesita implementar la interfaz,
        // sin que este archivo vuelva a crecer.
        var sistemasDebug = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include)
            .OfType<IEstadoDebug>();
        foreach (var sistema in sistemasDebug)
            sistema.AplicarEstadoDebug(etapa);

        // Por si el salto se dispara estando pausado o en cinemática.
        SceneDirector.Instance.TerminarEvento();
    }

    private void TeleportarJugador(Vector3 posicion, Vector3 rotacionEuler)
    {
        controladorJugador.enabled = false;
        jugador.position = posicion;
        jugador.rotation = Quaternion.Euler(rotacionEuler);
        controladorJugador.enabled = true;
    }
}