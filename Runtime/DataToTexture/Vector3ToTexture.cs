using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace UltraCombos.VFXToolBox
{
    [ExecuteInEditMode]
    public class Vector3ToTexture : MonoBehaviour
    {
        [LabelText("Fixed Buffer Count")]
        public bool m_bFixedBufferCount = false;
        [ShowIf("m_bFixedBufferCount"), LabelText("Count"), Indent]
        public int m_fixedBufferCount = 20;

        [Space]
        public List<Vector3> m_vectors = new List<Vector3>();
        public RenderTexture Result { get { return m_result; } }

        [SerializeField, HideInInspector] ComputeShader m_shader;
        ComputeBuffer m_buffer;
        [SerializeField, ReadOnly] RenderTexture m_result;

        int m_prevCount = 0;
        Vector3[] m_tempArray;

        public void ForceUpdate()
        {
            Update();
        }

        void Release()
        {
            m_buffer?.Release();
            m_buffer?.Dispose();
            m_buffer = null;

            if (m_result != null)
                m_result?.Release();
        }

        bool Check()
        {
            if (m_vectors == null || m_vectors.Count == 0)
            {
                Release();
                return false;
            }

            if (m_vectors.Count != m_prevCount || m_buffer == null || m_tempArray == null)
            {
                m_prevCount = m_vectors.Count;
                Release();
                m_result = RenderTextureUtil.Allocate(m_vectors.Count, 1, RenderTextureFormat.ARGBFloat, FilterMode.Point);
                m_buffer = new ComputeBuffer(m_vectors.Count, sizeof(float)*3);
                m_tempArray = new Vector3[m_vectors.Count];
                return true;
            }

            return true;
        }

        bool StaticCheck()
        {
            if (m_buffer == null || m_buffer.count != m_fixedBufferCount || m_tempArray == null)
            {
                Release();
                m_result = RenderTextureUtil.Allocate(m_fixedBufferCount, 1, RenderTextureFormat.ARGBFloat, FilterMode.Point);
                m_buffer = new ComputeBuffer(m_fixedBufferCount, sizeof(float)*3);
                m_tempArray = new Vector3[m_fixedBufferCount];
                return true;
            }

            return true;
        }

        private void Update()
        {
            if (m_bFixedBufferCount ? StaticCheck() : Check())
            {
                for (int i = 0; i < m_tempArray.Length; i++)
                {
                    m_tempArray[i] = Vector3.one*1000;
                    if (m_vectors.Count > i)
                        m_tempArray[i] = m_vectors[i];
                }

                int _kernel = m_shader.FindKernel("CopyFloat3Data");
                m_buffer.SetData(m_tempArray);
                m_shader.SetTexture(_kernel, "m_result", m_result);
                m_shader.SetBuffer(_kernel, "m_float3Buffer", m_buffer);
                m_shader.Dispatch(_kernel, m_tempArray.Length, 1, 1);
            }
        }

        private void OnDestroy()
        {
            Release();
        }
    }
}