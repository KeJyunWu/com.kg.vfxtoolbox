using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;

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

    static class ObjectUtil
    {
        public static void Destroy(Object o)
        {
            if (o == null) return;
            if (Application.isPlaying)
                Object.Destroy(o);
            else
                Object.DestroyImmediate(o);
        }
    }

    static class MathUtil
    {
        public static float TriangleArea(float3 v1, float3 v2, float3 v3)
          => math.length(math.cross(v2 - v1, v3 - v1)) / 2;
    }

    static class MemoryUtil
    {
        public static NativeArray<T> Array<T>(int length) where T : struct
            => new NativeArray<T>(length, Allocator.Persistent,
                                  NativeArrayOptions.UninitializedMemory);

        public static NativeArray<T> TempArray<T>(int length) where T : struct
            => new NativeArray<T>(length, Allocator.Temp,
                                  NativeArrayOptions.UninitializedMemory);

        public static NativeArray<T> TempJobArray<T>(int length) where T : struct
            => new NativeArray<T>(length, Allocator.TempJob,
                                  NativeArrayOptions.UninitializedMemory);
    }

    static class RenderTextureUtil
    {
        public static RenderTexture AllocateHalf(int width, int height)
          => Allocate(width, height, RenderTextureFormat.ARGBHalf);

        public static RenderTexture AllocateFloat(int width, int height)
          => Allocate(width, height, RenderTextureFormat.ARGBFloat);

        public static RenderTexture
          Allocate(int width, int height, RenderTextureFormat format)
        {
            var rt = new RenderTexture(width, height, 0, format);
            rt.hideFlags = HideFlags.DontSave;
            rt.enableRandomWrite = true;
            rt.Create();
            return rt;
        }
    }
}
