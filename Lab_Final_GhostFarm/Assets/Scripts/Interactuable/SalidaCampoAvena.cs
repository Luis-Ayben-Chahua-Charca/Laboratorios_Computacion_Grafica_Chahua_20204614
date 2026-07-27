using UnityEngine;

public class SalidaCampoAvena : MonoBehaviour
{
    [SerializeField] private CampoAvena campo;

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        campo.EscalarObsesion();
    }
}