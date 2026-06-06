using UnityEngine;

public class MoodManager : MonoBehaviour
{
    public static MoodManager Instance;

    public Light directionalLight;

    public MoodData happy;
    public MoodData sadness;
    public MoodData fear;
    public MoodData peace;


    private void Awake()
    {
        Instance = this;
    }

    public void SetMood(MoodType mood)
    {
        switch (mood)
        {
            case MoodType.Happy:
                ApplyMood(happy);
                break;

            case MoodType.Sadness:
                ApplyMood(sadness);
                break;

            case MoodType.Fear:
                ApplyMood(fear);
                break;

            case MoodType.Peace:
                ApplyMood(peace);
                break;
        }
    }

    private void ApplyMood(MoodData data)
    {
        RenderSettings.skybox = data.skybox;

        RenderSettings.ambientLight =
            data.ambientColor;

        RenderSettings.ambientIntensity =
            data.ambientIntensity;

        directionalLight.color =
            data.directionalColor;

        directionalLight.intensity =
            data.directionalIntensity;

        DynamicGI.UpdateEnvironment();
    }
}