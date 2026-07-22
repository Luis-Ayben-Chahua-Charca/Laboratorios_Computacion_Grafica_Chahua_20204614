using UnityEngine;

public interface IInteractable
{
    string TextoInteraccion { get; } // ej. "Recoger hoz [E]"
    void Interactuar(GameObject jugador);
}