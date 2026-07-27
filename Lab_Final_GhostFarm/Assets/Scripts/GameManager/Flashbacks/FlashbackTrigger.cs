using System.Collections;
using UnityEngine;

public class FlashbackTrigger : MonoBehaviour
{
    [SerializeField] private FlashbackData data;
    private bool yaSeActivo = false;

    public void Disparar()
    {
        if (yaSeActivo) return;
        yaSeActivo = true;
        StartCoroutine(EjecutarFlashback());
    }

    private IEnumerator EjecutarFlashback()
    {
        SceneDirector.Instance.IniciarFlashback();
        VisualModeController.Instance.EntrarModoMemoria(false);

        yield return new WaitForSeconds(data.duracion);

        VisualModeController.Instance.SalirModoMemoria(false);
        SceneDirector.Instance.TerminarEvento();
    }
}