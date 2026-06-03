using UnityEngine;

public class ChangeMoodSystem : MonoBehaviour
{
    public float rayDistance = 5f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            CheckInteraction();
        }
    }

    private void CheckInteraction()
    {
        Ray ray = Camera.main.ViewportPointToRay(
            new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance))
        {
            MoodButton button =
                hit.collider.GetComponent<MoodButton>();

            if (button != null)
            {
                MoodManager.Instance.SetMood(button.mood);
            }
        }
    }
}