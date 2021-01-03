using UnityEngine;
using Unity.Burst;
using System.Linq;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using System.Collections.Generic;
using System.Linq;

namespace UltraCombos.VFXTool
{
    public class MeshToVoxal : MonoBehaviour
    {
        public int sdfResolution = 64;
        public uint samplesPerTriangle = 10;
        public RenderTexture outputRenderTexture;
        public Material materialOutput;
        public float scaleBy = 1.0f;
        public Vector3 offset;

        [Space]
        public float m_size = 0.1f;
        [SerializeField] SkinnedMeshRenderer[] m_skinnedMeshs = null;
        [SerializeField] MeshFilter[] m_meshes = null;

        public bool m_debug = false;
        public ComputeShader m_transferCS;
        public ComputeShader MtVImplementation;

        public ComputeBuffer m_vertexBuffer;
        public ComputeBuffer m_indexBuffer;

        Mesh m_tempMesh;
        Vector3[] m_tempArray;

        bool m_firstDisplay = false;

        private void OnEnable() => InitializeInternals();
        private void OnDisable() => DisposeInternals();

        private void OnValidate()
        {
            DisposeInternals();
        }

        // Update is called once per frame
        private void LateUpdate()
        {
            if (m_tempMesh == null) InitializeInternals();

            var _vertexOffset = 0;
            var _indexOffset = 0;
            foreach (var source in m_skinnedMeshs)
            {
                var _offset = SkinnedMeshBake(source, _vertexOffset, _indexOffset, source.transform.localToWorldMatrix);
                _vertexOffset += _offset._vOffset;
                _indexOffset += _offset._iOffset;
            }

            foreach (var source in m_meshes)
            {
                var _offset = MeshBake(source, _vertexOffset, _indexOffset, source.transform.localToWorldMatrix);
                _vertexOffset += _offset._vOffset;
                _indexOffset += _offset._iOffset;
            }

           /* if(!m_firstDisplay)
            {
                m_vertexBuffer.GetData(m_tempArray);
                Debug.Log("===========KJ ===========");
                Debug.Log("Vertex Count : "+ m_vertexBuffer.count);
                for (var i = 0; i < m_tempArray.Length; i++)
                {
                    Debug.Log(i + " : "+m_tempArray[i]);
                }
                Debug.Log("................................");
                Debug.Log("Index Count : " + m_indexBuffer.count);
                int[]  m_tempArray2 = new int[m_indexBuffer.count];
                m_indexBuffer.GetData(m_tempArray2);
                for (var i = 0; i < m_tempArray2.Length; i++)
                {
                    Debug.Log(i + " : " + m_tempArray2[i]);
                }
                Debug.Log("=======================");
                m_firstDisplay = true;
            }*/

            outputRenderTexture = MeshToVoxel(sdfResolution, samplesPerTriangle, m_vertexBuffer, m_indexBuffer, outputRenderTexture);

            if (materialOutput)
            {
                if (!materialOutput.HasProperty("_Texture3D"))
                    Debug.LogError(string.Format("Material output doesn't have property {0}", "_Texture3D"));
                else
                    materialOutput.SetTexture("_Texture3D", outputRenderTexture);
            }
        }

        (int _vOffset, int _iOffset) SkinnedMeshBake(SkinnedMeshRenderer _source, int _vertexOffset, int _indexOffset, Matrix4x4 _transform)
        {
            _source.BakeMesh(m_tempMesh);
            return BakeSource(m_tempMesh, _vertexOffset, _indexOffset, _transform);
        }

        (int _vOffset, int _iOffset) MeshBake(MeshFilter _source, int _vertexOffset, int _indexOffset, Matrix4x4 _transform)
        {
            return BakeSource(_source.mesh, _vertexOffset, _indexOffset, _transform);
        }

        (int _vOffset, int _iOffset) BakeSource(Mesh _mesh, int _vertexOffset, int _indexOffset, Matrix4x4 _transform)
        {
            using (var dataArray = Mesh.AcquireReadOnlyMeshData(_mesh))
            {
                var _data = dataArray[0];
                var _vcount = _data.vertexCount;
                var _icount = _data.GetSubMesh(0).indexCount;

                using (var pos = MemoryUtil.TempJobArray<Vector3>(_vcount))
                using (var index = MemoryUtil.TempJobArray<int>(_icount))
                {
                    _data.GetVertices(pos);
                    _data.GetIndices(index, 0);

                    new ConcatenationJob
                    { m_output = index, m_indexOffset = _vertexOffset }.Run();

                    m_vertexBuffer.SetData(pos, 0, _vertexOffset, _vcount);
                    m_indexBuffer.SetData(index, 0, _indexOffset, _icount);

                    m_transferCS.SetInt("m_offset", _vertexOffset);
                    m_transferCS.SetInt("m_count", _vcount);
                    m_transferCS.SetMatrix("m_transformMatrix", _transform);
                    m_transferCS.SetBuffer(m_transferCS.FindKernel("Core"), "m_vertexBuffer", m_vertexBuffer);
                    m_transferCS.Dispatch(m_transferCS.FindKernel("Core"), Mathf.CeilToInt(_vcount / 8.0f), 1, 1);
                    return (_vcount, _icount);
                }
            }
        }

        void InitializeInternals()
        {
            using (var mesh = new CombinedMesh(m_meshes.Select(smr => smr.sharedMesh).ToArray(), m_skinnedMeshs))
            {
                var _vcount = mesh.Vertices.Length;
                m_vertexBuffer = new ComputeBuffer(mesh.Vertices.Length, sizeof(float)*3);
                m_indexBuffer = new ComputeBuffer(mesh.Indices.Length, sizeof(int));
                m_tempArray = new Vector3[mesh.Vertices.Length];

                // Temporary mesh object
                m_tempMesh = new Mesh();
                m_tempMesh.hideFlags = HideFlags.DontSave;
            }
        }

        void DisposeInternals()
        {
            m_vertexBuffer?.Dispose();
            m_vertexBuffer = null;

            m_indexBuffer?.Dispose();
            m_indexBuffer = null;

            ObjectUtil.Destroy(m_tempMesh);
            m_tempMesh = null;
        }

        private void OnDrawGizmos()
        {
            if (m_vertexBuffer != null && m_debug)
            {
                m_vertexBuffer.GetData(m_tempArray);
                for (var i = 0; i < m_tempArray.Length; i++)
                {
                    Gizmos.DrawWireCube(m_tempArray[i], Vector3.one * m_size);
                }
            }
        }

        public RenderTexture MeshToVoxel(int voxelResolution, uint numSamplesPerTriangle,
        ComputeBuffer _vertexBuffer, ComputeBuffer _indexBuffer, RenderTexture voxels = null)
        {
            int MtV = MtVImplementation.FindKernel("MeshToVoxel");
            int Zero = MtVImplementation.FindKernel("Zero");

            var numTris = _indexBuffer.count / 3;

            MtVImplementation.SetBuffer(MtV, "VertexBuffer", _vertexBuffer);
            MtVImplementation.SetBuffer(MtV, "IndexBuffer", _indexBuffer);
            MtVImplementation.SetInt("tris", numTris);
            MtVImplementation.SetFloats("offset", offset.x, offset.y, offset.z);
            MtVImplementation.SetInt("numSamples", (int)numSamplesPerTriangle);
            MtVImplementation.SetFloat("scale", scaleBy);
            MtVImplementation.SetInt("voxelSide", (int)voxelResolution);

            if (voxels == null)
            {
                voxels = new RenderTexture(voxelResolution, voxelResolution,
                        0, RenderTextureFormat.ARGBHalf);
                voxels.dimension = UnityEngine.Rendering.TextureDimension.Tex3D;
                voxels.enableRandomWrite = true;
                voxels.useMipMap = false;
                voxels.volumeDepth = voxelResolution;
                voxels.Create();
            }

            MtVImplementation.SetBuffer(Zero, "VertexBuffer", _vertexBuffer);
            MtVImplementation.SetBuffer(Zero, "IndexBuffer", _indexBuffer);
            MtVImplementation.SetTexture(Zero, "Voxels", voxels);
            MtVImplementation.Dispatch(Zero, numGroups(voxelResolution, 8),
                numGroups(voxelResolution, 8), numGroups(voxelResolution, 8));

            MtVImplementation.SetTexture(MtV, "Voxels", voxels);
            MtVImplementation.Dispatch(MtV, numGroups(numTris, 512), 1, 1);

            return voxels;
        }

        int numGroups(int totalThreads, int groupSize)
        {
            return (totalThreads + (groupSize - 1)) / groupSize;
        }
    }

    #region Index array concatenation job
    [BurstCompile(CompileSynchronously = true)]
    struct ConcatenationJob : IJob
    {
        public NativeArray<int> m_output;
        public int m_indexOffset;

        public void Execute()
        {
            for (var i = 0; i < m_output.Length; i++)
            {
                m_output[i] += m_indexOffset;
            }
        }
    }
    #endregion
}
