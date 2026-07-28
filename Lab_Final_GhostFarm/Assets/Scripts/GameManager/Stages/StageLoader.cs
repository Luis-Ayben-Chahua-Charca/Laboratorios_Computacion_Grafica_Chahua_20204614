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
        // FIX/NOTA: esto queda encerrado en la directiva de compilación para
        // que el salto de etapas nunca esté disponible en un build final —
        // solo en el Editor o en builds de desarrollo (Development Build
        // tildado al compilar). No hace falta sacarlo antes de entregar.
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

        if (etapa.jugadorTieneHoz && hozEnEscena != null)
            manoJugador.EquiparSilencioso(hozEnEscena);

        if (CampoAvena.Instance != null)
            CampoAvena.Instance.ForzarEstadoDebug(etapa.avenaCortadosMedio, etapa.avenaMisionSecundariaActiva, etapa.avenaCorregidosParejo);

        // Por si el salto se dispara estando pausado o en cinemática.
        SceneDirector.Instance.TerminarEvento();
    }

    private void TeleportarJugador(Vector3 posicion, Vector3 rotacionEuler)
    {
        // FIX: un CharacterController tiene su propia física interna; asignar
        // transform.position directamente mientras está habilitado puede
        // generar colisiones raras o que la próxima llamada a Move() lo
        // "corrija" de vuelta. Apagarlo, mover, y reencenderlo es el patrón
        // seguro recomendado por Unity para teletransportar un CharacterController.
        controladorJugador.enabled = false;
        jugador.position = posicion;
        jugador.rotation = Quaternion.Euler(rotacionEuler);
        controladorJugador.enabled = true;
    }
}