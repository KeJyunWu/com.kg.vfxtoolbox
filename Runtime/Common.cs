using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UltraCombos.VFXToolBox
{
    public class Common
    {
        public static void Swap<T>(T[] buffer)
        {
            T tmp = buffer[0];
            buffer[0] = buffer[1];
            buffer[1] = tmp;
        }

        public static RenderTexture CreateRT(int _width, int _height, int _depth, int _volume, RenderTextureFormat _format, FilterMode _filterMode)
        {
            RenderTexture _rt = new RenderTexture(_width, _height, _depth, _format);
            _rt.dimension = UnityEngine.Rendering.TextureDimension.Tex3D;
            _rt.filterMode = _filterMode;
            _rt.volumeDepth = _volume;
            _rt.enableRandomWrite = true;
            _rt.Create();
            return _rt;
        }

        public static RenderTexture CreateRT(int _width, int _height, RenderTextureFormat _format, FilterMode _filterMode)
        {
            RenderTexture _rt = new RenderTexture(_width, _height, 0, _format);
            _rt.filterMode = _filterMode;
            _rt.enableRandomWrite = true;
            _rt.Create();
            return _rt;
        }
    }
}
