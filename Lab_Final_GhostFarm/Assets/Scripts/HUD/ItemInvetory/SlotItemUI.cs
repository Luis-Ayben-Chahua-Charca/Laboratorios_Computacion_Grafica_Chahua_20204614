using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotItemUI : MonoBehaviour
{
    [SerializeField] private Image icono;
    [SerializeField] private TMP_Text texto;

    public void SetData(Sprite sprite, int cantidad)
    {
        icono.sprite = sprite;
        texto.text = $"x{cantidad}";
    }
}