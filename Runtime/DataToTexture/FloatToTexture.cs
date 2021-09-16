using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace UltraCombos.VFXToolBox
{
    [ExecuteInEditMode]
    public class FloatToTexture : MonoBehaviour
    {
        [LabelText("Fixed Buffer Count")]
        public bool m_bFixedBufferCount = false;
        [ShowIf("m_bFixedBufferCount"), LabelText("Count"), Indent]
        public int m_fixedBufferCount = 20;
        public float m_defaultValue = 0;
        [Space]
        public List<float> m_floats = new List<float>();
        public RenderTexture Result { get { return m_result; } }

        [SerializeField, HideInInspector] ComputeShader m_shader;
        ComputeBuffer m_buffer;
        [SerializeField, ReadOnly] RenderTexture m_result;

        int m_prevCount = 0;
        float[] m_tempArray;

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
            if (m_floats == null || m_floats.Count == 0)
            {
                Release();
                return false;
            }

            if (m_floats.Count != m_prevCount || m_buffer == null || m_tempArray == null)
            {
                m_prevCount = m_floats.Count;
                Release();
                m_result = RenderTextureUtil.Allocate(m_floats.Count, 1, RenderTextureFormat.ARGBFloat, FilterMode.Point);
                m_buffer = new ComputeBuffer(m_floats.Count, sizeof(float));
                m_tempArray = new float[m_floats.Count];
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
                m_buffer = new ComputeBuffer(m_fixedBufferCount, sizeof(float));
                m_tempArray = new float[m_fixedBufferCount];
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
                    m_tempArray[i] = m_defaultValue;
                    if (m_floats.Count > i)
                        m_tempArray[i] = m_floats[i];
                }

                int _kernel = m_shader.FindKernel("CopyFloatData");
                m_buffer.SetData(m_tempArray);
                m_shader.SetTexture(_kernel, "m_result", m_result);
                m_shader.SetBuffer(_kernel, "m_floatBuffer", m_buffer);
                m_shader.Dispatch(_kernel, m_tempArray.Length, 1, 1);
            }
        }

        private void OnDestroy()
        {
            Release();
        }
    }
}