using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TextureAttributeStore))]
public class TextureAttributesStoreEditor : Editor
{

    public SerializedProperty _predictedRoughness;
    public SerializedProperty _predictedFrequency;
    public SerializedProperty _drawFrequency;
    public SerializedProperty _intensityMax;
    public SerializedProperty _intensityMin;

    private void Awake()
    {
        SetProperties();
    }

    private void SetProperties()
    {
        _predictedRoughness = serializedObject.FindProperty("predictedRoughness");
        _predictedFrequency = serializedObject.FindProperty("predictedFrequency");
        _drawFrequency = serializedObject.FindProperty("drawFrequency");
        _intensityMax = serializedObject.FindProperty("intensityMax");
        _intensityMin = serializedObject.FindProperty("intensityMin");
    }

    public override void OnInspectorGUI()
    {
        SetProperties();

        EditorGUILayout.LabelField(name, EditorStyles.boldLabel);

        EditorGUI.BeginDisabledGroup(true);

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField(new GUIContent("NN Values", "The values reported by the neural network."), EditorStyles.boldLabel);

        EditorGUI.EndDisabledGroup();

        if (GUILayout.Button(new GUIContent("Reset Frequency", "Resets the draw frequency to values computed."), GUILayout.Width(120)))
        {
            _drawFrequency.floatValue = _predictedFrequency.floatValue;
        }

        EditorGUILayout.EndHorizontal();

        EditorGUI.BeginDisabledGroup(true);

        EditorGUILayout.FloatField("Predicted Roughness",_predictedRoughness.floatValue);
        EditorGUILayout.FloatField("Predicted Frequency",_predictedFrequency.floatValue);

        EditorGUI.EndDisabledGroup();

        _drawFrequency.floatValue = EditorGUILayout.Slider("Draw Frequency", _drawFrequency.floatValue, 20, 80);
        _intensityMin.floatValue = EditorGUILayout.Slider("Minimum Intensity", _intensityMin.floatValue, 0, 1);
        _intensityMax.floatValue = EditorGUILayout.Slider("Maximum Intensity", _intensityMax.floatValue, 0, 1);
    }

}
