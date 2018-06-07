using UnityEngine;
using System;

[ExecuteInEditMode]
public class DepthOfField : MonoBehaviour {

    [HideInInspector]
    public Shader dofShader;

    [NonSerialized]
    Material dofMaterial;

    [Range(0f, 100f)]
    public float startOffset = 5f;

    [Range(0f, 1000f)]
    public float maxDistance = 20f;

    [Range(0.0f, 10.0f)]
    public float bokehRadius = 2.5f;

    const int circleOfConfusionPass = 0;
    const int bokehPass = 1;
    const int bokehBlurPass = 2;
    const int blendPass = 3;
    
	//[ImageEffectOpaque]
	void OnRenderImage(RenderTexture _source, RenderTexture _destination) {

        maxDistance = Mathf.Max(maxDistance, startOffset);

        if(dofMaterial == null) {
            dofMaterial = new Material(dofShader);
            dofMaterial.hideFlags = HideFlags.HideAndDontSave;
        }

        dofMaterial.SetFloat("_StartOffset", startOffset);
        dofMaterial.SetFloat("_MaxDistance", maxDistance);
        dofMaterial.SetFloat("_BokehRadius", bokehRadius);

        RenderTexture _coc = RenderTexture.GetTemporary(
            _source.width, _source.height, 0,
            RenderTextureFormat.RHalf, RenderTextureReadWrite.Linear
        );

       // RenderTexture _dof0 = RenderTexture.GetTemporary(_source.width / 2, _source.height/2, 0, _source.format);
        RenderTexture _dof0 = RenderTexture.GetTemporary(_source.width, _source.height, 0, _source.format);
        //RenderTexture _dof1 = RenderTexture.GetTemporary(_source.width / 2, _source.height / 2, 0, _source.format);

        
        Graphics.Blit(_source, _coc, dofMaterial, circleOfConfusionPass);

        dofMaterial.SetTexture("_CoCTex", _coc);
        Graphics.Blit(_source, _dof0, dofMaterial, bokehPass);

        //Graphics.Blit(_dof0, _dof1, dofMaterial, bokehBlurPass);
        Graphics.Blit(_dof0, _destination, dofMaterial, bokehBlurPass);
        //dofMaterial.SetTexture("_FarTex", _dof1);

       // Graphics.Blit(_source, _destination, dofMaterial, blendPass);

        RenderTexture.ReleaseTemporary(_coc);
        RenderTexture.ReleaseTemporary(_dof0);
       // RenderTexture.ReleaseTemporary(_dof1);
    }

 
}
