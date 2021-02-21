using UnityEngine;
using Unity.Burst;
using System.Linq;
using Unity.Collections;
using Unity.Jobs;
using Sirenix.OdinInspector;

namespace UltraCombos.VFXToolBox
{
    public class UltraSDF : MonoBehaviour
    {
        [TitleGroup("Stsyem")]
        [SerializeField] int m_resolution = 64;
        public int Resolution { get => m_resolution; set => m_resolution = value; }

        [SerializeField] uint m_samplesPerTriangle = 10000;
        public uint SamplesPerTriangle { get => m_samplesPerTriangle; set => m_samplesPerTriangle = value; }

        [SerializeField] bool m_doSDF = false;
        public bool DoSDF { get => m_doSDF; set => m_doSDF = value; }

        [ShowIf("m_doSDF"), Indent]
        [SerializeField] float m_postProcessThickness = 0.01f;
        public float PostProcessThickness { get => m_postProcessThickness; set => m_postProcessThickness = value; }

        [Space]
        [SerializeField] Transform m_container;
        public Transform Container { get => m_container; set => m_container = value; }

        [SerializeField] SkinnedMeshRenderer[] m_skinnedMeshes = null;
        public SkinnedMeshRenderer[] SkinnedMeshes { get => m_skinnedMeshes; set => m_skinnedMeshes = value; }

        [SerializeField] MeshFilter[] m_meshes = null;
        public MeshFilter[] Meshes { get => m_meshes; set => m_meshes = value; }

        [TitleGroup("Debug")]
        public bool m_drawContainer = false;
        public bool m_drawVertex = false;
        [ShowIf("m_drawVertex"),Indent, LabelText("Size")]
        public float m_vertexSize = 0.1f;
        public Material m_viewerMat;
        [ShowIf("m_viewerMat"), Indent, LabelText("Property Name")]
        public string m_viewerMatPropertyName = "_Texture3D";

        [TitleGroup("Info")]
        [SerializeField, Sirenix.OdinInspector.ReadOnly, LabelText("SDF Pixel Format")]
        string m_SDFPixelFormat = "X : Distance, YZW : Direction";

        [SerializeField, HideInInspector] ComputeShader m_transferCS;
        [SerializeField, HideInInspector] ComputeShader m_MtVImplementationCS;
        [SerializeField, HideInInspector] ComputeShader m_JFAImplementationCS;

        RenderTexture m_result;
        public RenderTexture Result { get => m_result; set => m_result = value; }
        ComputeBuffer m_vertexBuffer;
        ComputeBuffer m_indexBuffer;
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
            foreach (var source in m_skinnedMeshes)
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

            Result = MeshToVoxel(m_resolution, m_samplesPerTriangle, m_vertexBuffer, m_indexBuffer, Result);

            if (m_doSDF)
                FloodFillToSDF(Result);

            if (m_viewerMat)
            {
                if (!m_viewerMat.HasProperty(m_viewerMatPropertyName))
                    Debug.LogError(string.Format("Material output doesn't have property {0}", m_viewerMatPropertyName));
                else
                    m_viewerMat.SetTexture(m_viewerMatPropertyName, Result);
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
            using (var mesh = new CombinedMesh(m_meshes.Select(smr => smr.sharedMesh).ToArray(), m_skinnedMeshes))
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
            if(m_drawContainer && m_container!=null)
            {
                Gizmos.DrawWireCube(m_container.transform.position, m_container.transform.localScale);
            }

            if (m_vertexBuffer != null && m_drawVertex)
            {
                m_vertexBuffer.GetData(m_tempArray);
                for (var i = 0; i < m_tempArray.Length; i++)
                {
                    Gizmos.DrawWireCube(m_tempArray[i], Vector3.one * m_vertexSize);
                }
            }
        }

        public RenderTexture MeshToVoxel(int voxelResolution, uint numSamplesPerTriangle,
        ComputeBuffer _vertexBuffer, ComputeBuffer _indexBuffer, RenderTexture voxels = null)
        {
            int MtV = m_MtVImplementationCS.FindKernel("MeshToVoxel");
            int Zero = m_MtVImplementationCS.FindKernel("Zero");

            var numTris = _indexBuffer.count / 3;

            m_MtVImplementationCS.SetBuffer(MtV, "VertexBuffer", _vertexBuffer);
            m_MtVImplementationCS.SetBuffer(MtV, "IndexBuffer", _indexBuffer);
            m_MtVImplementationCS.SetVector("_boundsMin", m_container.position - m_container.localScale / 2);
            m_MtVImplementationCS.SetVector("_boundsMax", m_container.position + m_container.localScale / 2);
            m_MtVImplementationCS.SetInt("tris", numTris);
            m_MtVImplementationCS.SetInt("numSamples", (int)numSamplesPerTriangle);
            m_MtVImplementationCS.SetInt("voxelSide", (int)voxelResolution);

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

            m_MtVImplementationCS.SetBuffer(Zero, "VertexBuffer", _vertexBuffer);
            m_MtVImplementationCS.SetBuffer(Zero, "IndexBuffer", _indexBuffer);
            m_MtVImplementationCS.SetTexture(Zero, "Voxels", voxels);
            m_MtVImplementationCS.Dispatch(Zero, numGroups(voxelResolution, 8),
                numGroups(voxelResolution, 8), numGroups(voxelResolution, 8));

            m_MtVImplementationCS.SetTexture(MtV, "Voxels", voxels);
            m_MtVImplementationCS.Dispatch(MtV, numGroups(numTris, 512), 1, 1);

            return voxels;
        }

        public void FloodFillToSDF(RenderTexture voxels)
        {
            int dispatchCubeSize = voxels.width;
            m_JFAImplementationCS.SetInt("dispatchCubeSide", dispatchCubeSize);

            m_JFAImplementationCS.SetTexture(m_JFAImplementationCS.FindKernel("Preprocess"), "Voxels", voxels);
            m_JFAImplementationCS.Dispatch(m_JFAImplementationCS.FindKernel("Preprocess"), numGroups(voxels.width, 8),
                    numGroups(voxels.height, 8), numGroups(voxels.volumeDepth, 8));

            m_JFAImplementationCS.SetTexture(m_JFAImplementationCS.FindKernel("JFA"), "Voxels", voxels);

            for (int offset = voxels.width / 2; offset >= 1; offset /= 2)
            {
                m_JFAImplementationCS.SetInt("samplingOffset", offset);
                m_JFAImplementationCS.Dispatch(m_JFAImplementationCS.FindKernel("JFA"), numGroups(voxels.width, 8),
                    numGroups(voxels.height, 8), numGroups(voxels.volumeDepth, 8));
            }

            m_JFAImplementationCS.SetFloat("postProcessThickness", m_postProcessThickness);
            m_JFAImplementationCS.SetTexture(m_JFAImplementationCS.FindKernel("Postprocess"), "Voxels", voxels);

            m_JFAImplementationCS.Dispatch(m_JFAImplementationCS.FindKernel("Postprocess"), numGroups(voxels.width, 8),
                numGroups(voxels.height, 8), numGroups(voxels.volumeDepth, 8));
        }
        int numGroups(int totalThreads, int groupSize)
        {
            return (totalThreads + (groupSize - 1)) / groupSize;
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
}
