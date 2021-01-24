using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

[ExecuteInEditMode]
public class NoiseGenerator : MonoBehaviour
{
    [SerializeField]
    Vector2Int m_resolution = new Vector2Int(1000, 1000);

    [Header("Base")] //3
    [Range(0,1)] public float m_globalUV_ScrollSpeed = 1;
    [Range(0, 100)] public float m_effect_Base_ResultMul = 1;
    [Range(0, 100)] public float m_effect_Base_NoiseScale = 1;

    [Space] //3
    [Range(0, 1)] public float m_effect_Base_UVTwirl_ScrollSpeed = 0.01f;
    [Range(0, 10)] public float m_effect_Base_UVTwirl_Intensity = 1;
    public Vector2 m_effect_Base_UVTwirl_Center = Vector2.zero;

    [Space] //4
    [Range(0, 1)] public float m_effect_Base_UVWarp_ScrollSpeed = 0.02f;
    [Range(0, 1)] public float m_effect_Base_UVWarp_Intensity = 1;
    [Range(0, 20)] public float m_effect_Base_UVWarp_NoiseScale = 16.6f;
    [Range(0, 20)] public float m_effect_Base_UVWarp2_NoiseScale = 0.46f;

    [Header("Wave")] //4
    [Range(0, 20)] public float m_effect_Wave_ResultMul  = 1;
    [Range(0, 10)] public float m_effect_Wave_Scale = 4.46f;
    [Range(0, 1)] public float m_effect_Wave_ScrollSpeed = 0.079f;
    [Range(0, 10)] public float m_effect_Wave_ResultPower = 3;
    public Vector2 m_effect_Wave_PivotOffset = Vector2.zero;

    [Header("Scan")] //6
    [Range(0, 10)] public float m_effect_Scan_ResultMul = 3;
    [Range(0, 1)] public float m_effect_Scan_ModeLerp  = 1;
    [Range(0, 10)] public float m_effect_Scan_ResultPower = 3;
    [Range(0, 10)] public float m_effect_Scan_UVWarpScale = 0.1f;
    [Range(0, 10)] public float m_effect_Scan_Scale = 0.1f;
    [Range(-1, 1)] public float m_effect_Scan_ScrollSpeed = 0.1f;

    [Header("fBM")] //5
    [Range(0, 10)] public float m_effect_FBM_ResultMul = 0;
    [Range(0, 5)] public float m_effect_FBM_Frequency = 1;
    [Range(0, 5)] public int m_effect_FBM_Octaves = 1;
    [Range(0, 5)] public float m_effect_FBM_Lacunarity = 1;
    [Range(0, 5)] public float m_effect_FBM_Gain = 1;

    [Header("Mask")] //5
    [Range(0, 5)] public float m_effect_Mask_ResultMul = 1;
    [Range(1, 200)] public float m_effect_Mask_PixelsNum = 3;
    [Range(0, 30)] public float m_effect_Mask_NoiseScale = 3;
    [Range(0, 1)] public float m_effect_Mask_NoiseScrollSpeed = 1;
    [Range(0, 10)] public float m_effect_Mask_ResultPower = 3;

    [SerializeField]
    RenderTexture m_internalResult;
    public RenderTexture Result { get { return m_internalResult; } }

    Material m_mat;
    public Material Mat { get { return m_mat; } }

    [SerializeField]
    Shader m_shader;

    public VisualEffect m_effect;

    RenderTexture CreateRT(Vector2Int _resolution, RenderTextureFormat _format)
    {
        RenderTexture _texture = new RenderTexture(_resolution.x, _resolution.y, 0, _format);
        _texture.enableRandomWrite = true;
        _texture.Create();
        return _texture;
    }

    void CheckResource()
    {
        if (m_internalResult == null)
            m_internalResult = CreateRT(m_resolution, RenderTextureFormat.ARGBFloat);

        if(m_internalResult.width != m_resolution.x || m_internalResult.height != m_resolution.y)
        {
            if (m_internalResult != null)
                m_internalResult.Release();
            m_internalResult = CreateRT(m_resolution, RenderTextureFormat.ARGBFloat);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (m_shader == null)
            return;

        CheckResource();

        if (m_mat == null)
            m_mat = new Material(m_shader);

        if (m_mat != null)
        {
            m_mat.SetFloat("_TextureRatio", (float)m_resolution.x/ m_resolution.y);
            m_mat.SetFloat("_ScriptTime", Time.time);

            m_mat.SetFloat("_GlobalUV_ScrollSpeed", m_globalUV_ScrollSpeed);
            m_mat.SetFloat("_Effect_Base_ResultMul", m_effect_Base_ResultMul);
            m_mat.SetFloat("_Effect_Base_NoiseScale", m_effect_Base_NoiseScale);

            m_mat.SetFloat("_Effect_Base_UVTwirl_ScrollSpeed", m_effect_Base_UVTwirl_ScrollSpeed);
            m_mat.SetFloat("_Effect_Base_UVTwirl_Intensity", m_effect_Base_UVTwirl_Intensity);
            m_mat.SetVector("_Effect_Base_UVTwirl_Center", m_effect_Base_UVTwirl_Center);

            m_mat.SetFloat("_Effect_Base_UVWarp_ScrollSpeed", m_effect_Base_UVWarp_ScrollSpeed);
            m_mat.SetFloat("_Effect_Base_UVWarp_Intensity", m_effect_Base_UVWarp_Intensity);
            m_mat.SetFloat("_Effect_Base_UVWarp_NoiseScale", m_effect_Base_UVWarp_NoiseScale);
            m_mat.SetFloat("_Effect_Base_UVWarp2_NoiseScale", m_effect_Base_UVWarp2_NoiseScale);

            m_mat.SetFloat("_Effect_Wave_ResultMul", m_effect_Wave_ResultMul);
            m_mat.SetFloat("_Effect_Wave_Scale", m_effect_Wave_Scale);
            m_mat.SetFloat("_Effect_Wave_ScrollSpeed", m_effect_Wave_ScrollSpeed);
            m_mat.SetFloat("_Effect_Wave_ResultPower", m_effect_Wave_ResultPower);
            m_mat.SetVector("_Effect_Wave_PivotOffset", m_effect_Wave_PivotOffset);

            m_mat.SetFloat("_Effect_Scan_ResultMul", m_effect_Scan_ResultMul);
            m_mat.SetFloat("_Effect_Scan_ModeLerp", m_effect_Scan_ModeLerp);
            m_mat.SetFloat("_Effect_Scan_ResultPower", m_effect_Scan_ResultPower);
            m_mat.SetFloat("_Effect_Scan_UVWarpScale", m_effect_Scan_UVWarpScale);
            m_mat.SetFloat("_Effect_Scan_Scale", m_effect_Scan_Scale);
            m_mat.SetFloat("_Effect_Scan_ScrollSpeed", m_effect_Scan_ScrollSpeed);

            m_mat.SetFloat("_Effect_FBM_ResultMul", m_effect_FBM_ResultMul);
            m_mat.SetFloat("_Effect_FBM_Frequency", m_effect_FBM_Frequency);
            m_mat.SetFloat("_Effect_FBM_Octaves", m_effect_FBM_Octaves);
            m_mat.SetFloat("_Effect_FBM_Lacunarity", m_effect_FBM_Lacunarity);
            m_mat.SetFloat("_Effect_FBM_Gain", m_effect_FBM_Gain);

            m_mat.SetFloat("_Effect_Mask_ResultMul", m_effect_Mask_ResultMul);
            m_mat.SetFloat("_Effect_Mask_PixelsNum", m_effect_Mask_PixelsNum);
            m_mat.SetFloat("_Effect_Mask_NoiseScale", m_effect_Mask_NoiseScale);
            m_mat.SetFloat("_Effect_Mask_NoiseScrollSpeed", m_effect_Mask_NoiseScrollSpeed);
            m_mat.SetFloat("_Effect_Mask_ResultPower", m_effect_Mask_ResultPower);
        }

        Graphics.Blit( null, m_internalResult, m_mat, 0);

        if (m_effect != null)
            m_effect.SetTexture("NoiseMap", m_internalResult);
    }
}
