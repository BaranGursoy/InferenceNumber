using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class DrawOnTexture : MonoBehaviour
{
    [Header("Textures")]
    [SerializeField] private Texture2D predictionTexture;
    [SerializeField] private Texture2D drawTexture;

    [SerializeField] private int brushRadius;
    [SerializeField] private Material material;

    private void Start()
    {
        DrawTextureScaleUp();
        ResetCanvas();
        CheckCam();
    }

    private void Update()
    {
        DoDrawing();
    }

    private void OnDestroy()
    {
        ResetCanvas();
    }

    private void CheckCam()
    {
        if (Camera.main == null)
        {
            throw new Exception("Cannot find main camera");
            this.enabled = false;
        }
    }

    /// <summary>
    /// Allowing to draw on texture
    /// </summary>
    /// <exception cref="Exception"></exception>
    private void DoDrawing()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            ResetCanvas();
        }

        if (!Input.GetMouseButton(0) && !Input.GetMouseButton(1)) return;

        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (!Physics.Raycast(mouseRay, out hit)) return;

        if (hit.collider.transform != transform) return;

        Vector2 pixelUV = hit.textureCoord;
        SetBrushCoordinates(GetBrushCoordinates(pixelUV));
    }

    public void ResetCanvas()
    {
        ResetPredictionTexture();
        ResetDrawTexture();

        PredictionTextureScaleUp();
    }

    private void ResetPredictionTexture()
    {
        var fillColorArray = new Color[predictionTexture.width * predictionTexture.height];
 
        for(var i = 0; i < fillColorArray.Length; ++i)
        {
            fillColorArray[i] = Color.black;
        }
        
        predictionTexture.SetPixels(fillColorArray);
        predictionTexture.Apply();
    }

    private void ResetDrawTexture()
    {
        var fillColorArray = new Color[drawTexture.width * drawTexture.height];
 
        for(var i = 0; i < fillColorArray.Length; ++i)
        {
            fillColorArray[i] = Color.black;
        }
        drawTexture.SetPixels(fillColorArray);
        drawTexture.Apply();
    }
    
    private List<Vector2> GetBrushCoordinates(Vector2 hitPoint)
    {
        List<Vector2> coordinates = new List<Vector2>();

        hitPoint = new Vector2(hitPoint.x * drawTexture.width, hitPoint.y * drawTexture.height);
        
        var hitPointX = (int) hitPoint.x;
        var hitPointY = (int) hitPoint.y;

        for (int x = hitPointX - brushRadius; x <= hitPoint.x + brushRadius; x++)
        {
            for (int y = hitPointY - brushRadius; y <= hitPoint.y + brushRadius; y++)
            {
                var point = new Vector2(x, y);
                coordinates.Add(point);
            }
        }

        return coordinates;
    }

    private void SetBrushCoordinates(List<Vector2> coordinates)
    {
        foreach (var coordinate in coordinates)
        {
            drawTexture.SetPixel((int)coordinate.x, (int)coordinate.y, Color.white);
            predictionTexture.SetPixel((int)coordinate.x, (int)coordinate.y, Color.white);
        }
        drawTexture.Apply();
        predictionTexture.Apply();
    }

    public void PredictionTextureScaleUp()
    { 
        Resize(256, 256);
    }
    
    public void DrawTextureScaleUp()
    { 
        DrawResize(256, 256);
    }
    
    public Texture2D PredictionTextureScaleDown()
    {
        Resize(28, 28);

        return predictionTexture;
    }
    
    void Resize(int targetX,int targetY)
    {
        RenderTexture rt=new RenderTexture(targetX, targetY,8);
        RenderTexture.active = rt;
        Graphics.Blit(predictionTexture,rt);
        predictionTexture=new Texture2D(targetX,targetY);
        predictionTexture.ReadPixels(new Rect(0,0,targetX,targetY),0,0);
        predictionTexture.Apply();
    }
    
    void DrawResize(int targetX,int targetY)
    {
        RenderTexture rt=new RenderTexture(targetX, targetY,8);
        RenderTexture.active = rt;
        Graphics.Blit(drawTexture,rt);
        drawTexture=new Texture2D(targetX,targetY);
        drawTexture.ReadPixels(new Rect(0,0,targetX,targetY),0,0);
        material.mainTexture = drawTexture;
        drawTexture.Apply();
    }
}
