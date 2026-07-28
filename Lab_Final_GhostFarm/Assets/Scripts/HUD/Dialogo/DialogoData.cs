using UnityEngine;

[System.Serializable]
public class LineaDialogo
{
    public string hablante;
    public Sprite retrato;
    [TextArea(2, 4)] public string texto;
}

[CreateAssetMenu(fileName = "NuevoDialogo", menuName = "Granja/Dialogo Data")]
public class DialogoData : ScriptableObject
{
    public LineaDialogo[] lineas;
}