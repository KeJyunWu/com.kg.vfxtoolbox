using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UltraCombos.VFXToolBox
{
    public class Texture3DCombiner : MonoBehaviour
    {
        const int READ = 0;
        const int WRITE = 1;
        const int NUM_THREADS = 8;

        public RenderTexture Result { get { return m_result[READ]; } }

        [SerializeField]
        public List<RenderTexture> m_textures = new List<RenderTexture>();

        [SerializeField]
        RenderTexture[] m_result;

        [SerializeField]
        ComputeShader m_shader;

        int m_initKernel;
        int m_coreKernel;

        void CheckRT()
        {
            if (m_result == null || m_result.Length == 0)
            {
                try
                {
                    m_result = new RenderTexture[2];
                    m_result[READ] = Common.CreateRT(m_textures[0].width, m_textures[0].height, 0, m_textures[0].volumeDepth, RenderTextureFormat.ARGBFloat, FilterMode.Point);
                    m_result[WRITE] = Common.CreateRT(m_textures[0].width, m_textures[0].height, 0, m_textures[0].volumeDepth, RenderTextureFormat.ARGBFloat, FilterMode.Point);
                }
                catch { }
            }
            else
            {
                if (m_result[READ] == null || m_result[READ].width != m_textures[0].width)
                {
                    for (var i = 0; i < m_result.Length; i++)
                    {
                        if (m_result[i] != null)
                            m_result[i].Release();
                    }

                    m_result[READ] = Common.CreateRT(m_textures[0].width, m_textures[0].height, 0, m_textures[0].volumeDepth, RenderTextureFormat.ARGBFloat, FilterMode.Point);
                    m_result[WRITE] = Common.CreateRT(m_textures[0].width, m_textures[0].height, 0, m_textures[0].volumeDepth, RenderTextureFormat.ARGBFloat, FilterMode.Point);
                }
            }
        }


        private void Start()
        {
            m_initKernel = m_shader.FindKernel("Init");
            m_coreKernel = m_shader.FindKernel("Core");
        }

        // Start is called before the first frame update
        // Update is called once per frame
        void Update()
        {
            if (m_textures != null && m_textures.Count != 0)
            {
                CheckRT();

                if (m_result[READ] != null)
                {
                    int m_dispatchX = Mathf.ClosestPowerOfTwo(m_result[READ].width);
                    int m_dispatchY = Mathf.ClosestPowerOfTwo(m_result[READ].height);
                    int m_dispatchZ = Mathf.ClosestPowerOfTwo(m_result[READ].volumeDepth);

                    m_shader.SetTexture(m_initKernel, "m_output", m_result[READ]);
                    m_shader.Dispatch(m_initKernel, m_dispatchX / NUM_THREADS, m_dispatchY / NUM_THREADS, m_dispatchZ / NUM_THREADS);

                    for (var i = 0; i < m_textures.Count; i++)
                    {
                        m_shader.SetTexture(m_coreKernel, "m_intput1", m_result[READ]);
                        m_shader.SetTexture(m_coreKernel, "m_intput2", m_textures[i]);
                        m_shader.SetTexture(m_coreKernel, "m_output", m_result[WRITE]);
                        m_shader.Dispatch(m_coreKernel, m_dispatchX / NUM_THREADS, m_dispatchY / NUM_THREADS, m_dispatchZ / NUM_THREADS);
                        Common.Swap(m_result);
                    }
                }
            }
        }

        private void OnDestroy()
        {
            if (m_result != null)
            {
                for (var i = 0; i < m_result.Length; i++)
                    m_result[i].Release();
            }
        }
    }
}