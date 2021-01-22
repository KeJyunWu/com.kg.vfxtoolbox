using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UltraCombos.VFXToolBox
{
    [ExecuteInEditMode]
    public class GradientTexture : MonoBehaviour
    {
        [SerializeField, Range(1, 256)] int m_resolution = 128;
        public int Resolution { get { return m_resolution; } set { m_resolution = value; } }

        [SerializeField] Gradient m_gradientColor;
        public Gradient GradientColor { get { return m_gradientColor; } set { m_gradientColor = value; } }

        [SerializeField]
        Texture2D m_result;
        public Texture2D Result { get { return m_result; } }

        private void Reset()
        {
            if (m_result != null)
            {
                if (Application.isPlaying)
                    Destroy(m_result);
                else
                    DestroyImmediate(m_result);
            }

            m_result = new Texture2D(m_resolution, 1, TextureFormat.ARGB32, false);
        }

        // Update is called once per frame
        void Update()
        {
            if (m_result == null || m_result.width != m_resolution)
                Reset();

            for (var i = 0; i < m_resolution; i++)
            {
                m_result.SetPixel(i, 0, m_gradientColor.Evaluate((float)i / (float)m_resolution));
            }
            m_result.Apply(false);
        }
    }
}