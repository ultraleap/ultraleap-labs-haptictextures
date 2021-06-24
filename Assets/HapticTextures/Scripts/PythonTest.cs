using UnityEngine;
using UnityEditor;

public class PythonTest : MonoBehaviour
{
    [ContextMenu("Run Meeeee")]
    void GetTextureFromObject()
    {
        Renderer r = GetComponent<Renderer>();
        TextureNNPrediction.GenerateFeatures(Application.dataPath + AssetDatabase.GetAssetPath(r.sharedMaterial.mainTexture).Substring(6));
    }    
}