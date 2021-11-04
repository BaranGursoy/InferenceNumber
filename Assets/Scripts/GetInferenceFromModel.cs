using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Unity.Barracuda;
using UnityEngine;
using UnityEngine.UI;

public class GetInferenceFromModel : MonoBehaviour
{
    public Texture2D texture;
    
    public NNModel modelAsset;
    
    private Model _runTimeModel;

    private IWorker _engine;

    [SerializeField] private Text text;

    [SerializeField] private DrawOnTexture drawOnTexture;

    [Serializable]
    public struct Prediction
    {
        public int predictedValue;
        public float[] predicted;

        public void SetPrediction(Tensor t, Text text)
        {
            predicted = t.AsFloats();
            predictedValue = Array.IndexOf(predicted, predicted.Max());
            Debug.Log($"Predicted: {predictedValue}");
            text.text = "Predicted: " + predictedValue;
        }
    }

    public Prediction prediction;

    private void Start()
    {
        _runTimeModel = ModelLoader.Load(modelAsset);
        _engine = WorkerFactory.CreateWorker(_runTimeModel, WorkerFactory.Device.CPU);
        prediction = new Prediction();
    }

    private void Update()
    {
        //Inference();
    }

    private void OnDestroy()
    {
        _engine?.Dispose();
    }

    public void Inference()
    {
            var texture = drawOnTexture.TextureScaleDown();
        // making a tensor out of a grayscale texture
            var channelCount = 1; // 1 = grayscale, 3 = color, 4 = color + alpha
            var inputX = new Tensor(texture, channelCount);

            Tensor outputY = _engine.Execute(inputX).PeekOutput();
            inputX.Dispose();
            
            prediction.SetPrediction(outputY, text);
            drawOnTexture.TextureScaleUp();
    }
}
