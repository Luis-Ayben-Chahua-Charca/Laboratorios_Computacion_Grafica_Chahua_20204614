using UnityEngine;

[CreateAssetMenu(fileName = "NuevoFlashback", menuName = "Granja/FlashbackData")]
public class FlashbackData : ScriptableObject
{
    public string idFlashback;
    public float duracion = 5f;
    public bool mantenerControlJugador = false;
    public bool salidaManual = false;

    [Header("Diálogo al iniciar (opcional)")]
    public DialogoData dialogoInicial;
}