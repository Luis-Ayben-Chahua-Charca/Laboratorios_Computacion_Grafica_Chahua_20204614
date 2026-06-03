using UnityEngine;

public class MoodManager : MonoBehaviour
{
    public static MoodManager Instance;

    public Light directionalLight;

    public MoodData comfort;
    public MoodData sadness;
    public MoodData fear;

    private void Awake()
    {
        Instance = this;
    }

    public void SetMood(MoodType mood)
    {
        switch (mood)
        {
            case MoodType.Happy:
                ApplyMood(comfort);
                break;

            case MoodType.Sadness:
                ApplyMood(sadness);
                break;

            case MoodType.Fear:
                ApplyMood(fear);
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