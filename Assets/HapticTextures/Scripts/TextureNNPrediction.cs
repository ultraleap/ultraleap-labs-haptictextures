using Unity.Barracuda;
using UnityEngine;
using UnityEditor.Scripting.Python;
using System;
using UnityEditor;

[ExecuteAlways]
public class TextureNNPrediction
{

    public static bool calculating = false;

    public static float predictedRoughness = 0f;
    
    public static float drawFrequency = 0f;

    public static Action<float,float> OnGeneratedFeatures;

    public static void SetFeatures()
    {
        calculating = false;
        drawFrequency = (float)Math.Round(Mathf.Lerp(20, 80, predictedRoughness), 2);
        OnGeneratedFeatures?.Invoke(predictedRoughness, drawFrequency);
        OnGeneratedFeatures = null;
    }

    #region Python
    public static void GenerateFeatures(string pathToImage)
    {
        try
        {
            string modelPath = Application.dataPath + AssetDatabase.GetAssetPath(Resources.Load<NNModel>("NNModel/model")).Substring(6);
            calculating = true;
            PythonRunner.RunString(@"
            import sys;
            import clr;
            sys.path.append(r'Assets/site-packages/texnet/')
            import texnet as tn;
            import onnxruntime;
            import numpy as np;

            feature_gen = tn.GenerateFeatures(r'"+pathToImage+ @"', image_size = 256);
            im_list = feature_gen.create_image_list();
            ms, _is, hs = feature_gen.create_matrix_list(im_list, [1, 2, 4, 6, 8, 10, 15, 20], [0, 45, 90, 135]);
            im, ms, hs = feature_gen.remap_features(im_list, ms, hs);

            onnx_model = r'"+modelPath+@"';
        
            session = onnxruntime.InferenceSession(onnx_model, None);

            input_name = session.get_inputs();
            output_name = session.get_outputs()[0].name

            result = session.run([output_name], {
                input_name[0].name: im.reshape(1, 256,256,1).astype(np.float32),
                input_name[1].name: ms.reshape(1, 256,256,1).astype(np.float32), 
                input_name[2].name: hs.reshape(1,8).astype(np.float32)
            });

            clr.TextureNNPrediction.predictedRoughness = float(str(result[0]).strip('[[]]'));
            clr.TextureNNPrediction.SetFeatures();
            ");
        }
        catch(Exception ex)
        {
            Debug.LogError(ex);
        }
    }

    #endregion
}
