using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UltraCombos.VFXToolBox
{
    public static class Common
    {
#if UNITY_EDITOR
        public static Texture2D ConvertRTtoTex2D(RenderTexture _RT)
        {
            //RenderTexture.active = _RT;
            Texture2D _tex = new Texture2D(_RT.width, _RT.height, TextureFormat.RGBAFloat, false);
            //_tex.ReadPixels(new Rect(0, 0, _RT.width, _RT.height), 0, 0);
          //  RenderTexture.active = null;
            
            Graphics.CopyTexture(_RT, _tex);
          //  _tex.Apply();
            return _tex;
        }

        public static void SaveRenderTextureIntoPNG(RenderTexture _RT, string _fullPath, string _name)
        {
            RenderTexture.active = _RT;
            Texture2D _tex = new Texture2D(_RT.width, _RT.height, TextureFormat.RGBAFloat, false);
            _tex.ReadPixels(new Rect(0, 0, _RT.width, _RT.height), 0, 0);
            RenderTexture.active = null;

            byte[] _bytes = _tex.EncodeToPNG();
            if (!Directory.Exists(_fullPath))
            {
                Directory.CreateDirectory(_fullPath);
            }
            File.WriteAllBytes(_fullPath + _name + ".png", _bytes);
        }

        public static Texture SaveRenderTextureToPNG(RenderTexture _RT, string _assetPath, string _name)
        {
            Texture2D _tex = new Texture2D(_RT.width, _RT.height, TextureFormat.RGBAFloat, false);
            _tex.filterMode = FilterMode.Point;
            RenderTexture.active = _RT;
            _tex.ReadPixels(new Rect(0, 0, _RT.width, _RT.height), 0, 0);
            RenderTexture.active = null;

            byte[] _bytes = ImageConversion.EncodeToEXR(_tex, Texture2D.EXRFlags.None);
            if (!Directory.Exists(_assetPath))
            {
                Directory.CreateDirectory(_assetPath);
            }
            File.WriteAllBytes(Application.dataPath + "/" + _assetPath + _name + ".exr", _bytes);

            string _p = "Assets/" + _assetPath + _name + ".exr";

            AssetDatabase.ImportAsset(_p, ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);
            var _tImporter = AssetImporter.GetAtPath(_p) as TextureImporter;
            _tImporter.mipmapEnabled = false;
            _tImporter.filterMode = FilterMode.Point;
            _tImporter.textureCompression = TextureImporterCompression.Uncompressed;
            _tImporter.npotScale = TextureImporterNPOTScale.None;
            _tImporter.isReadable = true;
            AssetDatabase.ImportAsset(_p, ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);

            return AssetDatabase.LoadAssetAtPath<Texture>(_p);
        }

        public static void SaveAsset(Object _asset, string _path, string _name)
        {
            if (_asset == null)
                return;

            if (!Directory.Exists(_path))
                Directory.CreateDirectory(_path);

            AssetDatabase.CreateAsset(_asset, _path + _name);
        }

        public static Texture2D Screenshot(int _resWidth, int _resHeight, Camera _camera, TextureFormat _format = TextureFormat.RGBAFloat)
        {
            RenderTexture rt = RenderTexture.GetTemporary(_resWidth, _resHeight, 24);
            _camera.targetTexture = rt;

            Texture2D _texture = new Texture2D(_resWidth, _resHeight, _format, false);
            _camera.Render();
            RenderTexture.active = rt;
            _texture.ReadPixels(new Rect(0, 0, _resWidth, _resHeight), 0, 0);
            _camera.targetTexture = null;
            RenderTexture.active = null;
            _texture.Apply();

            RenderTexture.ReleaseTemporary(rt);
            return _texture;
        }
#endif
    }

    public static class ObjectUtil
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

    public static class MathUtil
    {
        public static float TriangleArea(float3 v1, float3 v2, float3 v3)
          => math.length(math.cross(v2 - v1, v3 - v1)) / 2;
    }

    public static class MemoryUtil
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

    public static class RenderTextureUtil
    {
        public static void Swap<T>(T[] buffer)
        {
            T tmp = buffer[0];
            buffer[0] = buffer[1];
            buffer[1] = tmp;
        }

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

        public static RenderTexture Allocate(int _width, int _height, int _depth, int _volume, RenderTextureFormat _format, FilterMode _filterMode)
        {
            RenderTexture _rt = new RenderTexture(_width, _height, _depth, _format);
            _rt.dimension = UnityEngine.Rendering.TextureDimension.Tex3D;
            _rt.volumeDepth = _volume;
            _rt.filterMode = _filterMode;
            _rt.wrapMode = TextureWrapMode.Repeat;
            _rt.useMipMap = false;
            _rt.enableRandomWrite = true;
            _rt.Create();
            return _rt;
        }

        public static RenderTexture Allocate(int _width, int _height, RenderTextureFormat _format, FilterMode _filterMode)
        {
            RenderTexture _rt = new RenderTexture(_width, _height, 0, _format);
            _rt.filterMode = _filterMode;
            _rt.wrapMode = TextureWrapMode.Repeat;
            _rt.useMipMap = false;
            _rt.enableRandomWrite = true;
            _rt.Create();
            return _rt;
        }

        public static RenderTexture Allocate(RenderTexture _source)
        {
            RenderTexture _rt = new RenderTexture(_source);
            _rt.Create();
            return _rt;
        }

        public static RenderTexture Allocate(RenderTextureDescriptor _descriptor)
        {
            RenderTexture _rt = new RenderTexture(_descriptor);
            _rt.Create();
            return _rt;
        }
    }
}
