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

        protected Texture2D CreateTexture()
        {
            Texture2D _tex = new Texture2D(m_resolution, 1, TextureFormat.RGBAFloat, false, false);
            _tex.Apply();
            return _tex;
        }

        protected void UpdateTexture(ref Texture2D _texture, Gradient _gradient)
        {
            if (_texture == null)
                _texture = CreateTexture();

            if (_texture.width != m_resolution)
            {
                if (Application.isPlaying)
                    Destroy(_texture);
                else
                    DestroyImmediate(_texture);
                _texture = CreateTexture();
            }

            try
            {
                for (int i = 0; i < m_resolution; i++)
                {
                    _texture.SetPixel(i, 0, _gradient.Evaluate(i / (float)m_resolution));
                }
                _texture.Apply();
            }
            catch { }
        }

        protected Texture2DArray CreateTextureArray(int _depth)
        {
            Texture2DArray _tex = new Texture2DArray(m_resolution, 1, _depth, TextureFormat.RGBAFloat, false);
            _tex.Apply();
            return _tex;
        }

        protected void UpdateTextureArray(ref Texture2DArray _textureArray, Texture2D[] _textures, int _count)
        {
            if (_textureArray == null)
                _textureArray = CreateTextureArray(_count);

            if (_textureArray.width != m_resolution)
            {
                if (Application.isPlaying)
                    Destroy(_textureArray);
                else
                    DestroyImmediate(_textureArray);
                _textureArray = CreateTextureArray(_count);
            }

            try
            {
                for (int i = 0; i < _count; i++)
                {
                    _textureArray.SetPixels(_textures[i].GetPixels(0), i, 0);
                }
                _textureArray.Apply();
            }
            catch { }
        }
    }
}