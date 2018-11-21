using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DispDepth : MonoBehaviour {
    public Material mat;
    // Use this for initialization
    void Start () {
        GetComponent<Camera>().depthTextureMode |= DepthTextureMode.Depth;
    }

    // Update is called once per frame
    public void OnRenderImage(RenderTexture source, RenderTexture dest)
    {
        Graphics.Blit(source, dest, mat);
    }
}
