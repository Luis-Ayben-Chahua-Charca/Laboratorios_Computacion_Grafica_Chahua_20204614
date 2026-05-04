using UnityEngine;
using TMPro;

public class Proyectiles : MonoBehaviour
{
    [SerializeField] CannonController cannonController;
    private TMP_Text texto;

    void Start()
    {
        texto = GetComponent<TMP_Text>();
    }

    void Update()
    {
        if (cannonController != null && texto != null)
        {
            texto.text = "Disparos: " + cannonController.ProyectilesDisponibles;
        }
    }
}
