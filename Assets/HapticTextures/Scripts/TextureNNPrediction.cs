using Unity.Barracuda;
using UnityEngine;
using UnityEditor.Scripting.Python;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEditor;

[ExecuteInEditMode]
public class TextureNNPrediction : MonoBehaviour
{
    private string modelPath;
    public NNModel modelAsset;
    private static Model m_RuntimeModel;

    public static float numberUno = 5f;

    public static double[,] imageArray, matrixArray;
    public static double[] haralickArray;

    // Temporary Method
    void OnValidate()
    {
        if(m_RuntimeModel == null || modelAsset == null)
        {
            modelAsset = Resources.Load<NNModel>("NNModel/model");
            m_RuntimeModel = ModelLoader.Load(modelAsset);
        }
    }

    private static float[][] Convert2DArray(double[,] twoDimensionalArray)
    {
        int rowsFirstIndex = twoDimensionalArray.GetLowerBound(0);
        int rowsLastIndex = twoDimensionalArray.GetUpperBound(0);
        int numberOfRows = rowsLastIndex - rowsFirstIndex + 1;

        int columnsFirstIndex = twoDimensionalArray.GetLowerBound(1);
        int columnsLastIndex = twoDimensionalArray.GetUpperBound(1);
        int numberOfColumns = columnsLastIndex - columnsFirstIndex + 1;

        float[][] jaggedArray = new float[numberOfRows][];
        for (int i = 0; i < numberOfRows; i++)
        {
            jaggedArray[i] = new float[numberOfColumns];

            for (int j = 0; j < numberOfColumns; j++)
            {
                jaggedArray[i][j] = (float)twoDimensionalArray[i + rowsFirstIndex, j + columnsFirstIndex];
            }
        }
        return jaggedArray;
    }

    private static System.Single[] Convert1DArray(double[] arrayToConvert)
    {
        System.Single[] floatArray = new System.Single[arrayToConvert.Length];

        for (int i = 0; arrayToConvert.Length>i; i++)
        {
            floatArray[i] = Convert.ToSingle(arrayToConvert[i]);
        }
        return floatArray;
    }

    public static void RunPrediction()
    {        
        var worker = WorkerFactory.CreateWorker(WorkerFactory.Type.CSharpRef, m_RuntimeModel);

        var image = Convert2DArray(imageArray);
        var matrix = Convert2DArray(matrixArray);
        var haralick = Convert1DArray(haralickArray);

        var imageTensor = new Tensor(new int[]{0, 256, 256,1}, image, "input_10");
        var matrixTensor = new Tensor(new int[]{0, 256, 256,1}, matrix, "input_11");
        var haralickTensor = new Tensor(new int[]{0, 1, 1, 8},  haralick, "input_12");

        Dictionary<string, Tensor> tensorDict = new Dictionary<string, Tensor>();

        tensorDict.Add("input_10", imageTensor);
        tensorDict.Add("input_11", matrixTensor);
        tensorDict.Add("input_12", haralickTensor);

        worker.Execute(tensorDict);

        var prediction = worker.PeekOutput("dense_14");

        float[] vals = prediction.AsFloats();

        Debug.Log(vals[0]);

        // use the static variable to do the tensor
    }

    #region Python

    [MenuItem("Texnet/Initialise")]
    public static void GenerateFeatures(string pathToFile){
        PythonRunner.RunString(@"
        import sys;
        import clr;
        from System import Array;
        sys.path.append(r'Assets/site-packages/texnet/')
        import texnet as tn;
        import onnxruntime;
        import numpy as np;

        feature_gen = tn.GenerateFeatures(r'"+pathToFile+ @"', image_size = 256);
        im_list = feature_gen.create_image_list();
        ms, _is, hs = feature_gen.create_matrix_list(im_list, [1, 2, 4, 6, 8, 10, 15, 20], [0, 45, 90, 135]);
        im, ms, hs = feature_gen.remap_features(im_list, ms, hs);

        onnx_model = r'C:/Users/DavidBeattie/Documents/workspace/haptictextures_opensource/Assets/HapticTextures/Resources/NNModel/model.onnx';
    
        session = onnxruntime.InferenceSession(onnx_model, None);

        input_name = session.get_inputs();
        output_name = session.get_outputs()[0].name

        result = session.run([output_name], {
            input_name[0].name: im.reshape(1, 256,256,1).astype(np.float32),
            input_name[1].name: ms.reshape(1, 256,256,1).astype(np.float32), 
            input_name[2].name: hs.reshape(1,8).astype(np.float32)
        });

        print(str(result[0]).strip('[[]]'));

        #clr.TextureNNPrediction.imageArray = im;
        #clr.TextureNNPrediction.matrixArray = ms;
        #clr.TextureNNPrediction.haralickArray = hs;

        print('Features Generated.');

        #clr.TextureNNPrediction.RunPrediction();
        ");
    }

    #endregion
}
