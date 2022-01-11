using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace UltraCombos.VFXToolBox
{
    [ExecuteInEditMode]
    public class TransformToTexture : MonoBehaviour
    {
        public enum Mode
        {
            Position,
            Rotation,
            Scale
        }

        public Mode m_mode = Mode.Position;
        [LabelText("Fixed Buffer Count")]
        public bool m_bFixedBufferCount = false;
        [ShowIf("m_bFixedBufferCount"), LabelText("Count"), Indent]
        public int m_fixedBufferCount = 20;

        [Space]
        public List<Transform> m_transforms = new List<Transform>();
        public RenderTexture Result { get { return m_transforms.Count == 0 ? m_tempTex : m_result; } }

        [SerializeField, HideInInspector] ComputeShader m_shader;
        ComputeBuffer m_buffer;
        [SerializeField, ReadOnly] RenderTexture m_result;

        RenderTexture m_tempTex;
        Vector3[] m_tempArray;

        int m_prevCount = 0;

        public void ForceUpdate()
        {
            Update();
        }

        public void AddTransform(Transform _t)
        {
            m_transforms.Add(_t);
        }

        public void RemoveTransform(Transform _t)
        {
            m_transforms.Remove(_t);
        }

        void Release()
        {
            m_buffer?.Release();
            m_buffer?.Dispose();
            m_buffer = null;

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

            if (m_transforms.Count != m_prevCount || m_buffer == null || m_tempArray == null)
            {
                m_prevCount = m_transforms.Count;
                Release();
                m_result = RenderTextureUtil.Allocate(m_transforms.Count, 1, RenderTextureFormat.ARGBFloat, FilterMode.Point);
                m_buffer = new ComputeBuffer(m_transforms.Count, sizeof(float) * 3);
                m_tempArray = new Vector3[m_transforms.Count];
                return true;
            }

            return true;
        }

        bool StaticCheck()
        {
            if (m_buffer == null || m_tempArray == null || m_buffer.count != m_fixedBufferCount)
            {
                Release();
                m_result = RenderTextureUtil.Allocate(m_fixedBufferCount, 1, RenderTextureFormat.ARGBFloat, FilterMode.Point);
                m_buffer = new ComputeBuffer(m_fixedBufferCount, sizeof(float) * 3);
                m_tempArray = new Vector3[m_fixedBufferCount];
                return true;
            }

            return true;
        }

        private void Start()
        {
            m_tempTex = RenderTextureUtil.Allocate(1, 1, RenderTextureFormat.ARGBFloat, FilterMode.Point);
            m_shader.SetTexture(m_shader.FindKernel("Init"), "m_result", m_tempTex);
            m_shader.Dispatch(m_shader.FindKernel("Init"), 1, 1, 1);
        }

        private void Update()
        {
            if (m_bFixedBufferCount ? StaticCheck() : Check())
            {
                for (int i = 0; i < m_tempArray.Length; i++)
                {
                    m_tempArray[i] = Vector3.one * 100000;
                    if (m_transforms.Count > i)
                    {
                        switch (m_mode)
                        {
                            case Mode.Position:
                                m_tempArray[i] = m_transforms[i].position;
                                break;
                            case Mode.Rotation:
                                m_tempArray[i] = m_transforms[i].eulerAngles;
                                break;
                            default:
                                m_tempArray[i] = m_transforms[i].localScale;
                                break;
                        }
                    }
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
            Destroy(m_tempTex);
        }
    }
}