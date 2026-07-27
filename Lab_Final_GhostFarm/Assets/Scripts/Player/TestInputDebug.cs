using UnityEngine;

public class TestInputDebug : MonoBehaviour
{
    [Header("Cambio de Modo")]
    [SerializeField] private KeyCode cambioRecuerdo = KeyCode.K;
    [SerializeField] private VisualModeController visualModeController; 
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(cambioRecuerdo))
            VisualModeController.Instance.EntrarModoMemoria(false);
    }
}
