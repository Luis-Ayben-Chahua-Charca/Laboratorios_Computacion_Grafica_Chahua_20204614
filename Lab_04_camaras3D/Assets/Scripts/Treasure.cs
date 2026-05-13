using UnityEngine;

public class Treasure : MonoBehaviour
{
    [SerializeField] private GameController gameController;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameController.WinGame();
        }
    }
}