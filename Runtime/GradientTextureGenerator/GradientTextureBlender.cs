using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace UltraCombos.VFXToolBox
{
    public class GradientTextureBlender : GTBase
    {
        [SerializeField, GradientUsage(true, ColorSpace.Linear)]
        List<Gradient> m_gradient = new List<Gradient>();

        [Space]
        [SerializeField] int m_index = 0;
        public int Index { get { return m_index; } set { m_index = value; } }

        [SerializeField] float m_tweenSpeed = 1;
        public float TweenSpeed { get { return m_tweenSpeed; } set { m_tweenSpeed = value; } }

        [Space]
        [SerializeField] RenderTexture m_outputRT;
        public RenderTexture OutputRT { get { return m_outputRT; } }

        [SerializeField, HideInInspector] Shader m_shader;
        Texture2D[] m_texture = new Texture2D[100]; //100 Magic Number ㄏ
        Material m_mat;

        public int ClampIndex { get => Mathf.Clamp(m_index, 0, m_gradient.Count-1); }

        void Reset()
        {
            for (var i = 0; i < m_gradient.Count; i++)
            {
                UpdateTexture(ref m_texture[i], m_gradient[i]);
            }
        }

        void Awake()
        {
            m_mat = new Material(m_shader);
            Reset();
            Graphics.Blit(m_texture[ClampIndex], m_outputRT);
        }

        void Update()
        {
            if (m_outputRT != null && m_mat != null && m_gradient.Count != 0)
            {
                m_mat.SetFloat("m_tweenSpeed", m_tweenSpeed);
                m_mat.SetFloat("m_deltaTime", Time.deltaTime);
                m_mat.SetTexture("m_gradient", m_texture[ClampIndex]);

                RenderTexture _temp = RenderTexture.GetTemporary(m_outputRT.descriptor);
                Graphics.Blit(m_outputRT, _temp);
                Graphics.Blit(_temp, m_outputRT, m_mat);
                RenderTexture.ReleaseTemporary(_temp);
            }
        }
    }
}