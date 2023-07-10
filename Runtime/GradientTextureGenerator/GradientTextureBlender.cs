using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace UltraCombos.VFXToolBox
{
    public class GradientTextureBlender : GTBase
    {
        /// <summary>
        /// 
        /// </summary>
        [SerializeField, LabelText("Select Index")] int m_index = 0;
        public int Index { get { return m_index; } set { m_index = value; } }

        [SerializeField, GradientUsage(true, ColorSpace.Linear)]
        List<Gradient> m_gradient = new List<Gradient>();

        [SerializeField] float m_tweenSpeed = 1;
        public float TweenSpeed { get { return m_tweenSpeed; } set { m_tweenSpeed = value; } }

        [SerializeField] RenderTexture m_outputRT;
        public RenderTexture OutputRT { get { return m_outputRT; } }

        /// <summary>
        /// 
        /// </summary>
        [SerializeField, HideInInspector] Shader m_shader;
        Texture2D[] m_textures = new Texture2D[100]; //100 Magic Number ㄏ
        Texture2DArray m_textureArray;
        Material m_mat;

        /// <summary>
        /// 
        /// </summary>
        public int ClampIndex { get => Mathf.Clamp(m_index, 0, m_gradient.Count-1); }
        public Texture2D GetSubGradientTextureByIndex(int _index) { return m_textures[Mathf.Clamp(_index, 0, m_gradient.Count - 1)]; }
        public Texture2DArray GetAllGradientTextureWith2DArray() { UpdateTextureArray(ref m_textureArray, m_textures, m_gradient.Count); return m_textureArray; }

        /// <summary>
        /// 
        /// </summary>
        void Reset()
        {
            for (var i = 0; i < m_gradient.Count; i++)
            {
                UpdateTexture(ref m_textures[i], m_gradient[i]);
            }
        }

        void Awake()
        {
            m_mat = new Material(m_shader);
            Reset();
            Graphics.Blit(m_textures[ClampIndex], m_outputRT);
        }

        void Update()
        {
            for (var i = 0; i < m_gradient.Count; i++)
            {
                UpdateTexture(ref m_textures[i], m_gradient[i]);
            }

            if (m_outputRT != null && m_mat != null && m_gradient.Count != 0)
            {
                m_mat.SetFloat("m_tweenSpeed", m_tweenSpeed);
                m_mat.SetFloat("m_deltaTime", Time.deltaTime);
                m_mat.SetTexture("m_gradient", m_textures[ClampIndex]);

                RenderTexture _temp = RenderTexture.GetTemporary(m_outputRT.descriptor);
                Graphics.Blit(m_outputRT, _temp);
                Graphics.Blit(_temp, m_outputRT, m_mat);
                RenderTexture.ReleaseTemporary(_temp);
            }
        }
    }
}