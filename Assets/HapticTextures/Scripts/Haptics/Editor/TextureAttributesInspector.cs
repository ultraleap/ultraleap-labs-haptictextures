using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TextureAttributes))]
public class TextureAttributesInspector : Editor
{
    private TextureAttributes _attributesObject;

    private string currentPath = "";
    private string currentName = "";

    private void Awake()
    {
        SetProperties();
    }

    private void SetProperties()
    {
        _attributesObject = (TextureAttributes)serializedObject.targetObject; 
    }

    public override void OnInspectorGUI()
    {
        SetProperties();

        EditorGUILayout.BeginHorizontal();
        _attributesObject.texture = (TextureAttributeStore)EditorGUILayout.ObjectField("Texture Attributes",_attributesObject.texture, typeof(TextureAttributeStore),false);

        if (_attributesObject.texture == null)
        {
            GenerateTextureButton();
            EditorGUILayout.EndHorizontal();
        }
        else
        {
            RegenerateTextureButton();
            EditorGUILayout.EndHorizontal();
            ShowAttributeStore();
        }

        EditorGUI.BeginDisabledGroup(true);
        DrawDefaultInspector();
        EditorGUI.EndDisabledGroup();

        serializedObject.ApplyModifiedProperties();
    }

    private void GenerateTextureButton()
    {
        if (GUILayout.Button(new GUIContent("Calculate", "Uses the current main texture of the renderer to calculate a roughness value."), GUILayout.Width(100)))
        {
            GenerateTexture();
        }
    }

    private void RegenerateTextureButton()
    {
        if (GUILayout.Button(new GUIContent("Recalculate", "Uses the current main texture of the renderer to calculate a roughness value."), GUILayout.Width(100)))
        {
            GenerateTexture();
        }
    }

    private void ShowAttributeStore()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUI.BeginDisabledGroup(true);

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField(new GUIContent("NN Values","The values reported by the neural network."),EditorStyles.boldLabel);

        EditorGUI.EndDisabledGroup();

        if (GUILayout.Button(new GUIContent("Reset Frequency","Resets the draw frequency to values computed."),GUILayout.Width(120)))
        {
            _attributesObject.texture.drawFrequency = _attributesObject.texture.predictedFrequency;
        }

        EditorGUILayout.EndHorizontal();

        EditorGUI.BeginDisabledGroup(true);

        EditorGUILayout.FloatField("Predicted Roughness", _attributesObject.texture.predictedRoughness);
        EditorGUILayout.FloatField("Predicted Frequency", _attributesObject.texture.predictedFrequency);

        EditorGUI.EndDisabledGroup();

        _attributesObject.texture.drawFrequency = EditorGUILayout.Slider("Draw Frequency",_attributesObject.texture.drawFrequency, 20, 80);

        _attributesObject.texture.intensityMin = EditorGUILayout.Slider("Minimum Intensity", _attributesObject.texture.intensityMin, 0, 1);
        _attributesObject.texture.intensityMax = EditorGUILayout.Slider("Maximum Intensity", _attributesObject.texture.intensityMax, 0, 1);

        EditorGUILayout.EndVertical();
    }

    private void GenerateTexture()
    {
        Renderer r = _attributesObject.GetComponent<Renderer>();
        if (r == null)
        {
            EditorUtility.DisplayDialog("No Renderer", "No renderer set on current object.", "Ok");
        }
        else
        {
            if (r.sharedMaterial == null || r.sharedMaterial.mainTexture == null)
            {
                EditorUtility.DisplayDialog("No Material or Texture", "Current renderer does not have a material or main texture set.", "Ok");
            }
            else
            {
                TextureNNPrediction.OnGeneratedFeatures += OnGenerateFeature;
                currentPath = AssetDatabase.GetAssetPath(r.sharedMaterial.mainTexture);
                currentName = r.sharedMaterial.mainTexture.name;
                TextureNNPrediction.GenerateFeatures(Application.dataPath + currentPath.Substring(6));
            }
        }
    }

    private void OnGenerateFeature(float prediction, float frequency)
    {
        TextureAttributeStore tAS = ScriptableObject.CreateInstance<TextureAttributeStore>();
        tAS.name = currentName + " Texture Store";
        tAS.predictedRoughness = prediction;
        tAS.predictedFrequency = frequency;
        tAS.drawFrequency = frequency;
        AssetDatabase.CreateAsset(tAS, currentPath.Substring(0, currentPath.LastIndexOf('/')) + "/" + tAS.name + ".asset");
        _attributesObject.texture = tAS;
        EditorGUIUtility.PingObject(tAS);
    }

}
