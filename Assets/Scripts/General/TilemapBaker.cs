using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapBaker : MonoBehaviour
{
    public Tilemap tilemap;
    public SpriteRenderer outputRenderer;
    public int pixelsPerUnit = 16;

    public void Bake()
    {
        Bounds bounds = tilemap.localBounds;

        int width = Mathf.CeilToInt(bounds.size.x * pixelsPerUnit);
        int height = Mathf.CeilToInt(bounds.size.y * pixelsPerUnit);

        RenderTexture rt = new RenderTexture(width, height, 24);
        rt.filterMode = FilterMode.Point;

        GameObject camObj = new GameObject("TempBakeCam");
        Camera cam = camObj.AddComponent<Camera>();

        cam.orthographic = true;
        cam.orthographicSize = bounds.size.y / 2f;
        cam.transform.position = tilemap.transform.position + bounds.center + Vector3.back * 10f;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = Color.clear;
        cam.targetTexture = rt;
        cam.cullingMask = LayerMask.GetMask("GroundBake");
        cam.Render();

        RenderTexture.active = rt;
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tex.Apply();
        tex.filterMode = FilterMode.Point;

        RenderTexture.active = null;

        DestroyImmediate(camObj);
        rt.Release();

        Sprite sprite = Sprite.Create(tex, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), pixelsPerUnit);
        outputRenderer.sprite = sprite;
    }
}