using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class RenderFisheye : MonoBehaviour {
    public RenderTexture cubemapLeft;
    public RenderTexture cubemapRight;
    public RenderTexture equirectL;
    public RenderTexture equirectR;
    public RenderTexture equirectRT;
    public Camera cam;
    public Shader shader;
    public float alpha = 4.0f;
    public float chi = 0.0f;
    public float focalLength = 1.0f;

    private Material _material;
    private Material material {
        get {
            if (_material == null) {
                _material = new Material(shader);
                _material.hideFlags = HideFlags.HideAndDontSave;
            }
            return _material;
        }
    }

    void Start () {
    }

    private void Update()
    {
        cam.stereoSeparation = 0.065f;
        cam.RenderToCubemap(cubemapLeft, 63, Camera.MonoOrStereoscopicEye.Left);
        cam.RenderToCubemap(cubemapRight, 63, Camera.MonoOrStereoscopicEye.Right);
        cubemapLeft.ConvertToEquirect(equirectL);
        cubemapRight.ConvertToEquirect(equirectR);

        Graphics.CopyTexture(equirectL, 0, 0, 0, 0, equirectL.width, equirectL.height, equirectRT, 0, 0, 0, 0);
        Graphics.CopyTexture(equirectR, 0, 0, 0, 0, equirectR.width, equirectR.height, equirectRT, 0, 0, equirectL.width, 0);
    }

    private void OnDisable() {
        if (_material != null)
            DestroyImmediate(_material);
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
#if EM
        Vector2 scale = new Vector2(1.0f, 1);
        Vector2 offset = new Vector2(0.0f, 0);
        Graphics.Blit(equirectL, equirectRT, scale, offset);
        Vector2 offset2 = new Vector2(0.5f, 0);
        Graphics.Blit(equirectR, equirectRT, scale, offset2);

        //Debug.Log("OnRenderImage: Fisheye");
        if (shader != null) {
       	   material.SetTexture("_Cube", cubemapLeft);
       	   material.SetFloat("_Alpha", alpha);
       	   material.SetFloat("_Chi", chi);
       	   material.SetFloat("_FocalLength", focalLength);
            Graphics.Blit(source, destination, material);
       } else {
           Graphics.Blit(source, destination);
       }
#endif
    }
}
