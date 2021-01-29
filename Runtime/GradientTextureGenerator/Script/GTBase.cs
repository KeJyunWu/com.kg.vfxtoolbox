using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UltraCombos.VFXToolBox
{
    public class GTBase : MonoBehaviour
    {
        [SerializeField, Range(1, 256)]
        protected int m_resolution = 128;
        public int Resolution { get { return m_resolution; } set { m_resolution = value; } }

        protected Texture2D CreateTex()
        {
            Texture2D _tex = new Texture2D(m_resolution, 1, TextureFormat.RGBAFloat, false);
            _tex.Apply();
            return _tex;
        }

        protected void UpdateTexture(ref Texture2D _tex, Gradient _gradient)
        {
            if (_tex == null)
                _tex = CreateTex();

            if (_tex.width != m_resolution)
            {
                if (Application.isPlaying)
                    Destroy(_tex);
                else
                    DestroyImmediate(_tex);
                _tex = CreateTex();
            }

            try
            {
                for (int i = 0; i < m_resolution; i++)
                {
                    _tex.SetPixel(i, 0, _gradient.Evaluate(i / (float)m_resolution));
                }
                _tex.Apply();
            }
            catch { }
        }
    }
}