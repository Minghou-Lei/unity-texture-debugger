using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System;
using System.Collections.Generic;

[Serializable, VolumeComponentMenu("Debug/TextureDebugger")]
public class TextureDebugger : CustomPostProcessVolumeComponent, IPostProcessComponent

{
    //[Tooltip("Controls the intensity of the effect.")]
    //public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 1f);

    //public ClampedIntParameter GBufferIndex = new ClampedIntParameter(0, 0, 6);
    //public FloatParameter offsetX = new FloatParameter(0f);
    //public FloatParameter offsetY = new FloatParameter(0f);
    //public ClampedFloatParameter size = new ClampedFloatParameter(0.2f, 0f, 1f);
    public TexturesParameter tdiList = new TexturesParameter();

    private Rect _rect;
    private Texture _texture;
    private MaterialPropertyBlock mpb;
    private Material _material;

    public bool IsActive() => tdiList.value!=null;

    public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;


    public override void Setup()

    {
        if (Shader.Find("Custom/TextureDebugger") != null)
            _material = new Material(Shader.Find("Custom/TextureDebugger"));
        mpb = new MaterialPropertyBlock();
    }

    public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)

    {
        if (_material == null || tdiList.value.tdiList ==null)
            return;
        
        foreach (var input in tdiList.value.tdiList)
        {
            if(input.isActive!=true||input.TextureName==null)
                continue;
            _texture = Shader.GetGlobalTexture(input.TextureName);
            if (_texture == null)
                return;
            mpb.SetTexture("_InputTexture", _texture);
            _rect = new Rect(input.offsetX, input.offsetY, input.size * Screen.width, input.size * Screen.height);
            HDUtils.DrawFullScreen(cmd, _rect, _material, destination, mpb);
        }
        mpb.Clear();
    }

    
    public override void Cleanup() => CoreUtils.Destroy(_material);
}

[Serializable]
public class TextureDebuggerInput
{
    public string TextureName;
    public float offsetX = 0f;
    public float offsetY = 0f;
    public float size = 0.2f;
    public bool isActive = true;


    public TextureDebuggerInput(string textureName, float offsetX = 0f, float offsetY = 0f, float size = 0.2f)
    {
        TextureName = textureName;
        this.offsetX = offsetX;
        this.offsetY = offsetY;
        this.size = size;
    }

    public TextureDebuggerInput()
    {
        TextureName = "";
        this.offsetX = 0;
        this.offsetY = 0;
        this.size = 0.2f;
    }
}

[Serializable]
public class TextureDebuggerInputList
{
    public List<TextureDebuggerInput> tdiList;

    public TextureDebuggerInputList()
    {
        tdiList = new List<TextureDebuggerInput>();
    }
}

[Serializable]
public class TexturesParameter : VolumeParameter<TextureDebuggerInputList>
{
    public TextureDebuggerInputList TextureDebuggerInputList;

    public override bool overrideState
    {
        get => true;
    }

    public TexturesParameter()
    {
        TextureDebuggerInputList = new TextureDebuggerInputList();
    }
}