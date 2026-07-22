using UnityEngine;

public class ObjetoAgarrable : MonoBehaviour, IInteractable
{
    [SerializeField] private string nombre = "Hoz";
    public string TextoInteraccion => $"Recoger {nombre} [E]";

    public void Interactuar(GameObject jugador)
    {
        jugador.GetComponent<ManoJugador>()?.Equipar(this);
    }

    public void AlEquipar(Transform puntoDeAgarre)
    {
        var col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        transform.SetParent(puntoDeAgarre);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }
}