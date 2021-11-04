using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class DrawOnTexture : MonoBehaviour
{
    public Texture2D baseTexture;

    [SerializeField] private int brushRadius;
    [SerializeField] private Material material;

    private void Start()
    {
        ResetCanvas();
    }

    private void Update()
    {
        DoDrawing();
    }

    private void OnDestroy()
    {
        ResetCanvas();
    }

    /// <summary>
    /// Allowing to draw on texture
    /// </summary>
    /// <exception cref="Exception"></exception>
    private void DoDrawing()
    {
        if (Camera.main == null)
        {
            throw new Exception("Cannot find main camera");
        }

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
        var fillColorArray = new Color[baseTexture.width * baseTexture.height];
 
        for(var i = 0; i < fillColorArray.Length; ++i)
        {
            fillColorArray[i] = Color.black;
        }
  
        baseTexture.SetPixels(fillColorArray);
        baseTexture.Apply();
        TextureScaleUp();
    }

    private List<Vector2> GetBrushCoordinates(Vector2 hitPoint)
    {
        List<Vector2> coordinates = new List<Vector2>();

        hitPoint = new Vector2(hitPoint.x * baseTexture.width, hitPoint.y * baseTexture.height);
        
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
            baseTexture.SetPixel((int)coordinate.x, (int)coordinate.y, Color.white);
        }
        baseTexture.Apply();
    }

    public void TextureScaleUp()
    { 
        Resize(256, 256);
    }
    
    public Texture2D TextureScaleDown()
    {
        Resize(28, 28);

        return baseTexture;
    }
    
    void Resize(int targetX,int targetY)
    {
        RenderTexture rt=new RenderTexture(targetX, targetY,8);
        RenderTexture.active = rt;
        Graphics.Blit(baseTexture,rt);
        baseTexture=new Texture2D(targetX,targetY);
        baseTexture.ReadPixels(new Rect(0,0,targetX,targetY),0,0);
        material.mainTexture = baseTexture;
        baseTexture.Apply();
    }
}
