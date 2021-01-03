using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;

namespace UltraCombos.VFXTool
{
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

}
