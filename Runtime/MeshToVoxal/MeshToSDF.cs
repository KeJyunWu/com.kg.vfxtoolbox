using UnityEngine;
using Unity.Burst;
using System.Linq;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using System.Collections.Generic;

namespace UltraCombos.VFXTool
{
    public class MeshToSDF : MonoBehaviour
    {
        [Header("[ Stsyem Parameter]")]
        public int sdfResolution = 64;
        public uint samplesPerTriangle = 10;
        public bool doSDF = false;
        public float postProcessThickness = 0.01f;
        [SerializeField] SkinnedMeshRenderer[] m_skinnedMeshs = null;
        [SerializeField] MeshFilter[] m_meshes = null;
        public RenderTexture outputRenderTexture;
        public Material materialOutput;
        public Transform m_container;

        [Header("[ Debug ]")]
        public bool m_drawGizmos = false;
        public float m_gizmosSize = 0.1f;

        [Header("[ Resources ]")]
        public ComputeShader m_transferCS;
        public ComputeShader MtVImplementation;
        public ComputeShader JFAImplementation;
        public ComputeBuffer m_vertexBuffer;
        public ComputeBuffer m_indexBuffer;

        Mesh m_tempMesh;
        Vector3[] m_tempArray;

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

            outputRenderTexture = MeshToVoxel(sdfResolution, samplesPerTriangle, m_vertexBuffer, m_indexBuffer, outputRenderTexture);

            if (doSDF)
                FloodFillToSDF(outputRenderTexture);

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
            if (m_vertexBuffer != null && m_drawGizmos)
            {
                m_vertexBuffer.GetData(m_tempArray);
                for (var i = 0; i < m_tempArray.Length; i++)
                {
                    Gizmos.DrawWireCube(m_tempArray[i], Vector3.one * m_gizmosSize);
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
            MtVImplementation.SetVector("_boundsMin", m_container.position - m_container.localScale / 2);
            MtVImplementation.SetVector("_boundsMax", m_container.position + m_container.localScale / 2);
            MtVImplementation.SetInt("tris", numTris);
            MtVImplementation.SetInt("numSamples", (int)numSamplesPerTriangle);
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

        public void FloodFillToSDF(RenderTexture voxels)
        {
            int dispatchCubeSize = voxels.width;
            JFAImplementation.SetInt("dispatchCubeSide", dispatchCubeSize);

            JFAImplementation.SetTexture(JFAImplementation.FindKernel("Preprocess"), "Voxels", voxels);
            JFAImplementation.Dispatch(JFAImplementation.FindKernel("Preprocess"), numGroups(voxels.width, 8),
                    numGroups(voxels.height, 8), numGroups(voxels.volumeDepth, 8));

            JFAImplementation.SetTexture(JFAImplementation.FindKernel("JFA"), "Voxels", voxels);

            /*JFAImplementation.Dispatch(JFA, numGroups(voxels.width, 8),
                numGroups(voxels.height, 8), numGroups(voxels.volumeDepth, 8)); */

            for (int offset = voxels.width / 2; offset >= 1; offset /= 2)
            {
                JFAImplementation.SetInt("samplingOffset", offset);
                JFAImplementation.Dispatch(JFAImplementation.FindKernel("JFA"), numGroups(voxels.width, 8),
                    numGroups(voxels.height, 8), numGroups(voxels.volumeDepth, 8));
            }

            JFAImplementation.SetFloat("postProcessThickness", postProcessThickness);
            JFAImplementation.SetTexture(JFAImplementation.FindKernel("Postprocess"), "Voxels", voxels);

            JFAImplementation.Dispatch(JFAImplementation.FindKernel("Postprocess"), numGroups(voxels.width, 8),
                numGroups(voxels.height, 8), numGroups(voxels.volumeDepth, 8));
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
