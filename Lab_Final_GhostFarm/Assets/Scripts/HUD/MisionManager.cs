using UnityEngine;

public class MisionManager : MonoBehaviour
{
    public static MisionManager Instance { get; private set; }
    [SerializeField] private HUDController hud;

    private string tareaActual = "";

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void SetTarea(string descripcion)
    {
        tareaActual = descripcion;
        hud.ActualizarTextoMision(tareaActual);
    }

    public void LimpiarTarea() => SetTarea("");
}