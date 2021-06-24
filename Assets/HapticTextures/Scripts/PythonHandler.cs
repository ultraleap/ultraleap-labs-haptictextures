using UnityEditor.Scripting.Python;
using UnityEditor;

public class PythonHandler
{
    private string modelPath;
    
    [MenuItem("Texnet/Initialise")]
    static void Initialise(){
        PythonRunner.RunString(@"
        import sys;
        sys.path.append(r'Assets/site-packages/texnet/')
        import texnet as tn;
        import pandas as pd;
        import UnityEngine;
        import clr;
        ");
    }
    
    [MenuItem("Texnet/GenerateFeatures")]
    static void GenerateFeatures(){
        PythonRunner.RunString(@"
        feature_gen = tn.GenerateFeatures(r'C:/Users/DavidBeattie/Documents/workspace/texnet/test_images/Carpet013_1K_Color.jpg', image_size = 256);
        im_list = feature_gen.create_image_list();
        ms, _is, hs = feature_gen.create_matrix_list(im_list, [1, 2, 4, 6, 8, 10, 15, 20], [0, 45, 90, 135]);
        feature_df = pd.DataFrame({'image_list':im_list, 'matrix_list':ms, 'haralick_list':hs});
        mapped_features = feature_gen.remap_features(feature_df);
        print('Features Generated.');
        ");
    }

    [MenuItem("Texnet/CSharp")]
    static void TestCSharp(){
        PythonRunner.RunString(@"
        import clr;
        clr.TextureNNPrediction.numberUno = 10.0;
        clr.TextureNNPrediction.RunPrediction();
        ");
    }
}