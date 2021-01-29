using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;

namespace UltraCombos.VFXToolBox
{
    [System.Serializable]
    public class Texture3DMixerData
    {
        public TextureType m_DataFormat;
        [HideIf("m_DataFormat", TextureType.Texture3D)]
        public RenderTexture m_sourceRT;
        [HideIf("m_DataFormat", TextureType.RenderTexture)]
        public Texture3D m_source3D;
    }

    public class Texture3DMixer : MonoBehaviour
    {
        const int READ = 0;
        const int WRITE = 1;
        const int NUM_THREADS = 8;

        public RenderTexture Result { get => m_swapBuffer[READ]; }

        [TitleGroup("Stsyem Parameter")]
        [SerializeField] Vector3Int m_resolution = new Vector3Int(128, 128, 128);
        public Vector3Int Resolution { get => m_resolution; set => m_resolution = value; }

        [SerializeField]
        List<Texture3DMixerData> m_sources = new List<Texture3DMixerData>();
        public List<Texture3DMixerData> Sources { get => m_sources; set => m_sources = value; }

        [TitleGroup("Debug")]
        public Material m_viewerMat;
        [ShowIf("m_viewerMat"), Indent, LabelText("Property Name")]
        public string m_viewerMatPropertyName = "_Texture3D";

        [SerializeField,HideInInspector]
        ComputeShader m_shader;
        int m_initKernel;
        int m_coreKernel;
        RenderTexture[] m_swapBuffer;

        void BufferCheck()
        {
            bool _b = false;
            if (m_swapBuffer == null)
            {
                m_swapBuffer = new RenderTexture[2];
                _b = true;
            }
            else
            {
                if (m_swapBuffer[READ].width != m_resolution.x)
                {
                    m_swapBuffer[READ]?.Release();
                    m_swapBuffer[WRITE]?.Release();
                    _b = true;
                }
            }
            if(_b)
            {
                m_swapBuffer[READ] = RenderTextureUtil.CreateRT(m_resolution.x, m_resolution.y, 0, m_resolution.z, RenderTextureFormat.ARGBFloat, FilterMode.Point);
                m_swapBuffer[WRITE] = RenderTextureUtil.CreateRT(m_resolution.x, m_resolution.y, 0, m_resolution.z, RenderTextureFormat.ARGBFloat, FilterMode.Point);
            }
        }

        private void Start()
        {
            m_initKernel = m_shader.FindKernel("Init");
            m_coreKernel = m_shader.FindKernel("Core");
        }

        void Update()
        {
            if (m_sources != null && m_sources.Count != 0)
            {
                BufferCheck();
                var _filter =
                    from _source in m_sources
                    where _source.m_DataFormat == TextureType.Texture3D ? (_source.m_source3D != null ) : (_source.m_sourceRT != null && _source.m_sourceRT.dimension == UnityEngine.Rendering.TextureDimension.Tex3D && _source.m_sourceRT.enableRandomWrite)
                    select _source;

                List<Texture3DMixerData> _data = _filter.ToList();

                if (m_swapBuffer[READ] != null)
                {
                    int m_dispatchX = Mathf.CeilToInt(m_swapBuffer[READ].width / NUM_THREADS);
                    int m_dispatchY = Mathf.CeilToInt(m_swapBuffer[READ].height / NUM_THREADS);
                    int m_dispatchZ = Mathf.CeilToInt(m_swapBuffer[READ].volumeDepth / NUM_THREADS);

                    m_shader.SetTexture(m_initKernel, "m_output", m_swapBuffer[READ]);
                    m_shader.Dispatch(m_initKernel, m_dispatchX, m_dispatchY, m_dispatchZ);

                    for (var i = 0; i < _data.Count; i++)
                    {
                        m_shader.SetTexture(m_coreKernel, "m_intput1", m_swapBuffer[READ]);

                        if (_data[i].m_DataFormat == TextureType.RenderTexture)
                            m_shader.SetTexture(m_coreKernel, "m_intput2", _data[i].m_sourceRT);
                        else
                            m_shader.SetTexture(m_coreKernel, "m_intput2", _data[i].m_source3D);
                        
                        m_shader.SetTexture(m_coreKernel, "m_output", m_swapBuffer[WRITE]);
                        m_shader.Dispatch(m_coreKernel, m_dispatchX, m_dispatchY, m_dispatchZ);
                        RenderTextureUtil.Swap(m_swapBuffer);
                    }
                }

                if (m_viewerMat)
                {
                    if (!m_viewerMat.HasProperty(m_viewerMatPropertyName))
                        Debug.LogError(string.Format("Material output doesn't have property {0}", m_viewerMatPropertyName));
                    else
                        m_viewerMat.SetTexture(m_viewerMatPropertyName, Result);
                }
            }
        }

        private void OnDestroy()
        {
            if (m_swapBuffer != null)
            {
                for (var i = 0; i < m_swapBuffer.Length; i++)
                    m_swapBuffer[i].Release();
            }
        }
    }
}