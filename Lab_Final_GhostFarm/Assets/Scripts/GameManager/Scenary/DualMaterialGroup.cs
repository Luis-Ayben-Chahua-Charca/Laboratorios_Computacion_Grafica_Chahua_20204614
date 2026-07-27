using UnityEngine;

public class DualMaterialGroup : MonoBehaviour, IDualStyle
{
    // ... lo que ya tenías (texturaCartoon, texturaRealista, MaterialPropertyBlock)

    public void SetModo(bool memoria) => SetModo(memoria ? true : false); // reusa tu método existente si se llama distinto, ajustá el nombre
}