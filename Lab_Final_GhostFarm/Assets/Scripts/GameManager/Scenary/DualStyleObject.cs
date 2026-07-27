using UnityEngine;

public class DualStyleObject : MonoBehaviour, IDualStyle
{
    [SerializeField] private GameObject visualCartoon;
    [SerializeField] private GameObject visualRealista;


    public void SetModo(bool memoria)
    {
        visualCartoon.SetActive(memoria);
        visualRealista.SetActive(!memoria);
    }
}