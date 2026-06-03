using UnityEngine;

[System.Serializable]
public class MoodData
{
    public Material skybox;

    public Color ambientColor;

    [Range(0f, 5f)]
    public float ambientIntensity;

    public Color directionalColor;

    [Range(0f, 5f)]
    public float directionalIntensity;
}