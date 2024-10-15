using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class TextureStitcher : MonoBehaviour
{
    public enum Mode{
        Vertical,
        Horizontal
    }

    public enum Anchor
    {
        LEFTUP,
        LEFTDOWN
    }


    [TitleGroup("System Parameter")]
    [SerializeField]
    bool m_updateOnce = false;

    [SerializeField, HideIf("@UnityEngine.Application.isPlaying == true")]
    Mode m_mode;

    [SerializeField, HideIf("@UnityEngine.Application.isPlaying == true")]
    Anchor m_anchorPoint;

    [SerializeField, HideIf("@UnityEngine.Application.isPlaying == true")]
    int m_numForEachRowOrColume;

    [SerializeField, HideIf("@UnityEngine.Application.isPlaying == true")]
    Vector2Int m_resolution;
    public Vector2Int Resolution { get { return m_resolution; } set { m_resolution = value; } }

    [SerializeField, HideIf("@UnityEngine.Application.isPlaying == true")]
    Texture[] m_sources;
    public Texture[] Sources { get { return m_sources; } set { m_sources = value; } }

    [SerializeField,Space]
    Texture m_stampTex;
    public Texture StampTex { set { m_stampTex = value; } get { return m_stampTex; } }
    [SerializeField]
    bool m_stampUVFlip_w = false;
    [SerializeField]
    bool m_stampUVFlip_h = false;

    [SerializeField, HideInInspector]
    ComputeShader m_shader;

    [TitleGroup("Event")]
    public UnityEvent<RenderTexture> OnEvent = new UnityEvent<RenderTexture>();

    [SerializeField, ReadOnly]
    RenderTexture m_result;
    public RenderTexture Result { get { return m_result; } }

    public void Init()
    {
        if (m_result != null)
        {
            m_result?.Release();
            m_result = null;
        }

        if (m_resolution.x != 0 && m_resolution.y != 0)
        {
            m_result = new RenderTexture(m_resolution.x, m_resolution.y, 0, RenderTextureFormat.ARGBFloat);
            m_result.enableRandomWrite = true;
            m_result.Create();

            OnEvent?.Invoke(m_result);
        }
    }

    private void Awake()
    {
        Init();
    }

    bool m_b = false;
    // Update is called once per frame
    void Update()
    {
        if ((m_updateOnce && !m_b) || !m_updateOnce)
        {
            m_b = true;
            int _index = 0;

            Vector2 _startPointPos = new Vector2(0,0);
            switch(m_anchorPoint)
            {
                case Anchor.LEFTDOWN:
                    _startPointPos = new Vector2(0, 0);
                    break;
                case Anchor.LEFTUP:
                    _startPointPos = new Vector2(0, m_resolution.y-1);
                    break;
            }

            for (int i = 0; i < m_sources.Length; i++)
            {
                m_shader.SetTexture(m_shader.FindKernel("Core"), "m_source", m_sources[i]);
                m_shader.SetTexture(m_shader.FindKernel("Core"), "m_result", m_result);
                m_shader.SetInt("m_anchorMode", (int)m_anchorPoint);
                m_shader.SetVector("m_startPointPos", _startPointPos);
                m_shader.Dispatch(m_shader.FindKernel("Core"),
                    m_sources[i].width,
                    m_sources[i].height,
                     1);

                if (m_mode == Mode.Horizontal)
                {
                    if (_index < m_numForEachRowOrColume-1)
                    {
                        _startPointPos.x += m_sources[i].width;
                        _index++;
                    }
                    else
                    {
                        _index = 0;
                        _startPointPos.x = 0;
                        if (m_anchorPoint == Anchor.LEFTDOWN)
                            _startPointPos.y += m_sources[i].height;
                        else
                            _startPointPos.y -= m_sources[i].height;
                    }
                }
                else
                {
                    if (_index < m_numForEachRowOrColume - 1)
                    {
                        if (m_anchorPoint == Anchor.LEFTDOWN)
                            _startPointPos.y += m_sources[i].height;
                        else
                            _startPointPos.y -= m_sources[i].height;
                        _index++;
                    }
                    else
                    {
                        _index = 0;
                        _startPointPos.x += m_sources[i].width;
                        if (m_anchorPoint == Anchor.LEFTDOWN)
                            _startPointPos.y = 0;
                        else
                            _startPointPos.y = m_resolution.y - 1;
                    }
                }
            }

            if (m_stampTex != null)
            {
                m_shader.SetBool("m_stampUVFlip_w", m_stampUVFlip_w);
                m_shader.SetBool("m_stampUVFlip_h", m_stampUVFlip_h);
                m_shader.SetTexture(m_shader.FindKernel("Stamp"), "m_source", m_stampTex);
                m_shader.SetTexture(m_shader.FindKernel("Stamp"), "m_result", m_result);
                m_shader.Dispatch(m_shader.FindKernel("Stamp"), m_resolution.x, m_resolution.y, 1);
            }
        }
    }

    private void OnDestroy()
    {
        if(m_result!=null)
            m_result.Release();
    }

    private void OnValidate()
    {
        if (m_numForEachRowOrColume < 1)
            m_numForEachRowOrColume = 1;
    }

}
