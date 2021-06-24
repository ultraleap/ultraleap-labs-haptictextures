using UnityEngine;

[CreateAssetMenu(fileName = "Texture Store", menuName = "Haptic Texture/Texture Store", order = 0)]
public class TextureAttributeStore : ScriptableObject
{
    public float predictedRoughness = 0, predictedFrequency = 0;

    public float drawFrequency = 0;

    public float intensityMin = 0, intensityMax = 1;
}