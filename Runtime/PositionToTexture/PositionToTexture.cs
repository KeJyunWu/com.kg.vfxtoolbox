using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UltraCombos.VFXToolBox
{
    [ExecuteInEditMode]
    public class PositionToTexture : MonoBehaviour
    {
        public List<Transform> m_transforms = new List<Transform>();

        public RenderTexture Result { get { return m_result; } }

        [SerializeField, HideInInspector] ComputeShader m_shader;
        ComputeBuffer m_positionBuffer;
        [SerializeField, ReadOnly] RenderTexture m_result;
        Vector3[] m_tempArray;

        int m_prevCount = 0;

        void Release()
        {
            m_positionBuffer?.Release();
            m_positionBuffer?.Dispose();
            m_positionBuffer = null;

            if(m_result!=null)
                m_result?.Release();
        }

        bool Check()
        {
            if (m_transforms == null || m_transforms.Count == 0)
            {
                Release();
                return false;
            }

            if (m_transforms.Count != m_prevCount || m_positionBuffer == null || m_tempArray == null)
            {
                m_prevCount = m_transforms.Count;
                Release();
                m_result = RenderTextureUtil.Allocate(m_transforms.Count, 1, RenderTextureFormat.ARGBFloat, FilterMode.Point);
                m_positionBuffer = new ComputeBuffer(m_transforms.Count, sizeof(float) * 3);
                m_tempArray = new Vector3[m_transforms.Count];
                return true;
            }

            return true;
        }

        private void Update()
        {
            if (Check())
            {
                for (int i = 0; i < m_tempArray.Length; i++)
                {
                    m_tempArray[i] = m_transforms[i].position;
                }

                int _kernel = m_shader.FindKernel("Core");
                m_positionBuffer.SetData(m_tempArray);
                m_shader.SetTexture(_kernel, "m_result", m_result);
                m_shader.SetBuffer(_kernel, "m_positionBuffer", m_positionBuffer);
                m_shader.Dispatch(_kernel, m_tempArray.Length, 1, 1);
            }
        }

        private void OnDestroy()
        {
            Release();
        }
    }
}