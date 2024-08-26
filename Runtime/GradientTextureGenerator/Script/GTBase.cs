using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UltraCombos.VFXToolBox
{
    public class GTBase : MonoBehaviour
    {
        [SerializeField]
        protected bool m_alwaysUpdateTexture = true;

        [SerializeField, Range(2, 256)]
        protected int m_resolution = 128;
        public int Resolution { get { return m_resolution; } set { m_resolution = value; } }

        Color Tween(Color current, Color target, float omega, float dt)
        {
            float _exp = Mathf.Exp(-omega * dt);
            return Color.Lerp(target, current, _exp);
        }

        protected Texture2D TextureCreating()
        {
            Texture2D _tex = new Texture2D(m_resolution, 1, TextureFormat.RGBAFloat, false, false);
            _tex.filterMode = FilterMode.Bilinear;
            _tex.Apply();
            return _tex;
        }

        protected void TextureChecking(ref Texture2D _texture)
        {
            if (_texture == null)
                _texture = TextureCreating();

            if (_texture.width != m_resolution)
            {
                if (Application.isPlaying)
                    Destroy(_texture);
                else
                    DestroyImmediate(_texture);
                _texture = TextureCreating();
            }
        }

        protected void TextureInjection(ref Texture2D _texture, Gradient _gradient)
        {
            if (_texture == null)
                return;
            try
            {
                float _inv = 1f / (_texture.width - 1);
                for (int i = 0; i < m_resolution; i++)
                {
                    _texture.SetPixel(i, 0, _gradient.Evaluate(i* _inv));
                }
                _texture.Apply();
            }
            catch { }
        }

        protected void TextureTransition(Texture2D _result, Texture2D _target, float _speed)
        {
            try
            {
                float _inv = 1f / (_result.width - 1);
                for (int i = 0; i < m_resolution; i++)
                {
                    _result.SetPixel(i, 0, Tween(_result.GetPixel(i,0), _target.GetPixel(i,0), _speed, Time.deltaTime)) ;
                }
                _result.Apply();
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