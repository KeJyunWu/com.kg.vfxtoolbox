using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class TextureStitcher : MonoBehaviour
{
    [TitleGroup("System Parameter")]
    [SerializeField, HideIf("@UnityEngine.Application.isPlaying == true")]
    Vector2Int m_resolution;
    public Vector2Int Resolution { get { return m_resolution; } }

    [SerializeField, HideIf("@UnityEngine.Application.isPlaying == true")]
    Texture[] m_sources;
    public Texture[] Sources { get { return m_sources; } }

    [SerializeField]
    Texture m_mask;

    [SerializeField, HideInInspector]
    ComputeShader m_shader;

    [TitleGroup("Event")]
    public UnityEvent<RenderTexture> OnEvent = new UnityEvent<RenderTexture>();

    [SerializeField, ReadOnly]
    RenderTexture m_result;

    // Start is called before the first frame update
    void Start()
    {
        m_result = new RenderTexture(m_resolution.x, m_resolution.y, 0, RenderTextureFormat.ARGBFloat);
        m_result.enableRandomWrite = true;
        m_result.Create();

        OnEvent?.Invoke(m_result);
    }

    // Update is called once per frame
    void Update()
    {
        int _index = 0;
        Vector2 _startPointPos = new Vector2(0,0);
        for (int i=0; i< m_sources.Length; i++)
        {
            m_shader.SetTexture(m_shader.FindKernel("Core"), "m_source", m_sources[i]);
            m_shader.SetTexture(m_shader.FindKernel("Core"), "m_result", m_result);
            m_shader.SetVector("m_startPointPos", _startPointPos);
            m_shader.Dispatch( m_shader.FindKernel("Core"),
                (int)Mathf.Clamp(m_sources[i].width, 0, m_resolution.x - _startPointPos.x),
                 (int)Mathf.Clamp(m_sources[i].height, 0, m_resolution.y),
                 1);
            _index ++;
            _startPointPos.x += m_sources[i].width;
        }

        if (m_mask != null)
        {
            m_shader.SetTexture(m_shader.FindKernel("Mask"), "m_source", m_mask);
            m_shader.SetTexture(m_shader.FindKernel("Mask"), "m_result", m_result);
            m_shader.Dispatch(m_shader.FindKernel("Mask"), m_resolution.x, m_resolution.y, 1);
        }
    }

    private void OnDestroy()
    {
        m_result?.Release();
    }
}
