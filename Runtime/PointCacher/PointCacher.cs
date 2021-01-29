using UnityEngine;
using Unity.Burst;
using System.Linq;
using Unity.Collections;
using Unity.Jobs;
using Sirenix.OdinInspector;

namespace UltraCombos.VFXToolBox
{
    public enum MatrixType
    {
        Target,
        Self,
        None
    }

    public class PointCacher : MonoBehaviour
    {
        [TitleGroup("Stsyem Parameter")]
        [SerializeField] int m_pointCount = 5;
        public int PointCount { get => m_pointCount; set => m_pointCount = value; }

        [SerializeField] SkinnedMeshRenderer[] m_skinnedMeshes = null;
        public SkinnedMeshRenderer[] SkinnedMeshes { get => m_skinnedMeshes; set => m_skinnedMeshes = value; }

        [SerializeField] MeshFilter[] m_meshes = null;
        public MeshFilter[] Meshes { get => m_meshes; set => m_meshes = value; }

        [TitleGroup("Data Post Process")]
        [SerializeField] MatrixType m_matrixType =  MatrixType.None;
        public MatrixType MatrixType { get => m_matrixType; set => m_matrixType = value; }

        [ShowIf("m_matrixType", MatrixType.Target)]
        [SerializeField] Transform m_targetTransform;
        public Transform TargetTransform { get => m_targetTransform; set => m_targetTransform = value; }

        [SerializeField] Vector3 m_offset = Vector3.zero;
        public Vector3 Offset { get => m_offset; set => m_offset = value; }

        [SerializeField] Vector3 m_scale = Vector3.one;
        public Vector3 Scale { get => m_scale; set => m_scale = value; }

        [SerializeField] bool m_flipX = false;
        public bool FlipX { get => m_flipX; set => m_flipX = value; }

        [SerializeField] bool m_flipY = false;
        public bool FlipY { get => m_flipY; set => m_flipY = value; }

        [SerializeField] bool m_flipZ = false;
        public bool FlipZ { get => m_flipZ; set => m_flipZ = value; }

        [TitleGroup("Debug")]
        public bool m_drawVertex = false;
        [ShowIf("m_drawVertex"), Indent, LabelText("Size")]
        public float m_gizmosSize = 0.1f;

        [SerializeField, HideInInspector] ComputeShader m_pointCacherCS;

        RenderTexture m_positionMap;
        public RenderTexture PositionMap { get => m_positionMap;}

        ComputeBuffer m_vertexTransferredBuffer;
        public ComputeBuffer VertexTransferredBuffer { get => m_vertexTransferredBuffer; }

        Vector3[] m_vertexTransferredArray;
        public Vector3[] VertexTransferredArray { get { if (VertexTransferredBuffer != null) { VertexTransferredBuffer.GetData(m_vertexTransferredArray); return m_vertexTransferredArray; } else { return null; } } }

        RenderTexture m_velocityMap;
        RenderTexture m_normalMap;

        ComputeBuffer m_samplePointsBuffer;
        ComputeBuffer m_vertexBuffer;

        ComputeBuffer m_indexBuffer;
        ComputeBuffer m_uvBuffer;

        Mesh m_tempMesh;
        Vector3[] m_tempArray;

        private void OnEnable() => InitializeInternals();
        private void OnDisable() => DisposeInternals();

        private void OnValidate()
        {
            DisposeInternals();
        }

        private void LateUpdate()
        {
            if (m_tempMesh == null) InitializeInternals();

            var _vertexOffset = 0;
            var _indexOffset = 0;

            foreach (var source in m_meshes)
            {
                Matrix4x4 _matrix = m_matrixType == MatrixType.Target && m_targetTransform != null ? m_targetTransform.localToWorldMatrix : source.transform.localToWorldMatrix;
                var _offset = MeshBake(source, _vertexOffset, _indexOffset, _matrix);
                _vertexOffset += _offset._vOffset;
                _indexOffset += _offset._iOffset;
            }

            foreach (var source in m_skinnedMeshes)
            {
                Matrix4x4 _matrix = m_matrixType == MatrixType.Target && m_targetTransform != null ? m_targetTransform.localToWorldMatrix : source.transform.localToWorldMatrix;
                var _offset = SkinnedMeshBake(source, _vertexOffset, _indexOffset, _matrix);
                _vertexOffset += _offset._vOffset;
                _indexOffset += _offset._iOffset;
            }

            TransferData();
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

                    m_pointCacherCS.SetInt("m_offset", _vertexOffset);
                    m_pointCacherCS.SetInt("m_count", _vcount);
                    m_pointCacherCS.SetMatrix("m_transformMatrix", _transform);

                    int _kernel = m_pointCacherCS.FindKernel("Transform");
                    m_pointCacherCS.SetInt("m_matrixType", (int)m_matrixType);
                    m_pointCacherCS.SetVector("m_vertexOffset", m_offset);
                    m_pointCacherCS.SetVector("m_vertexScale", m_scale);
                    m_pointCacherCS.SetVector("m_vertexFlip", new Vector3(m_flipX?-1:1, m_flipY ? -1 : 1, m_flipZ ? -1 : 1));
                    m_pointCacherCS.SetBuffer(_kernel, "m_vertexBuffer", m_vertexBuffer);
                    m_pointCacherCS.Dispatch(_kernel, Mathf.CeilToInt(_vcount / 8.0f), 1, 1);
                    return (_vcount, _icount);
                }
            }
        }

        void TransferData()
        {
            int _kernel = m_pointCacherCS.FindKernel("TransferData");

            m_pointCacherCS.SetInt("SampleCount", m_pointCount);
            m_pointCacherCS.SetFloat("FrameRate", 1 / Time.deltaTime);

            m_pointCacherCS.SetBuffer(_kernel, "SamplePoints", m_samplePointsBuffer);
            m_pointCacherCS.SetBuffer(_kernel, "PositionBuffer", m_vertexBuffer);
            m_pointCacherCS.SetBuffer(_kernel, "PositionTransferredBuffer", m_vertexTransferredBuffer);

            m_pointCacherCS.SetTexture(_kernel, "PositionMap", m_positionMap);
            m_pointCacherCS.SetTexture(_kernel, "VelocityMap", m_velocityMap);
            m_pointCacherCS.SetTexture(_kernel, "NormalMap", m_normalMap);

            var width = m_positionMap.width;
            var height = m_positionMap.height;
            m_pointCacherCS.Dispatch(_kernel, width / 8, height / 8, 1);
        }

        void InitializeInternals()
        {
            using (var mesh = new CombinedMesh(m_meshes.Select(smr => smr.sharedMesh).ToArray(), m_skinnedMeshes))
            {
                using (var points = SamplePointGenerator.Generate
                      (mesh, m_pointCount))
                {
                    m_samplePointsBuffer = new ComputeBuffer(m_pointCount, SamplePoint.SizeInByte);
                    m_samplePointsBuffer.SetData(points);
                }

                var _vcount = mesh.Vertices.Length;
                m_vertexBuffer = new ComputeBuffer(mesh.Vertices.Length, sizeof(float) * 3);
                m_indexBuffer = new ComputeBuffer(mesh.Indices.Length, sizeof(int));
                m_uvBuffer = new ComputeBuffer(mesh.Vertices.Length, sizeof(float)*2);
                m_vertexTransferredBuffer = new ComputeBuffer(m_pointCount, sizeof(float) * 3);
                m_vertexTransferredArray = new Vector3[m_pointCount];
                m_tempArray = new Vector3[m_vertexBuffer.count];
            }

            var width = 256;
            var height = (((m_pointCount + width - 1) / width + 7) / 8) * 8;
            m_positionMap = RenderTextureUtil.AllocateFloat(width, height);
            m_velocityMap = RenderTextureUtil.AllocateHalf(width, height);
            m_normalMap = RenderTextureUtil.AllocateHalf(width, height);

            // Temporary mesh object
            m_tempMesh = new Mesh();
            m_tempMesh.hideFlags = HideFlags.DontSave;
        }

        void DisposeInternals()
        {
            m_samplePointsBuffer?.Dispose();
            m_samplePointsBuffer = null;

            m_vertexTransferredBuffer?.Dispose();
            m_vertexTransferredBuffer = null;

            m_vertexBuffer?.Dispose();
            m_vertexBuffer = null;

            m_indexBuffer?.Dispose();
            m_indexBuffer = null;

            m_uvBuffer?.Dispose();
            m_uvBuffer = null;

            ObjectUtil.Destroy(m_positionMap);
            m_positionMap = null;

            ObjectUtil.Destroy(m_velocityMap);
            m_velocityMap = null;

            ObjectUtil.Destroy(m_normalMap);
            m_normalMap = null;

            ObjectUtil.Destroy(m_tempMesh);
            m_tempMesh = null;

            m_tempArray = null;
            m_vertexTransferredArray = null;
        }

        private void OnDrawGizmos()
        {
            if (m_vertexBuffer != null && m_drawVertex)
            {
                m_vertexBuffer.GetData(m_tempArray);
                for (var i = 0; i < m_tempArray.Length; i++)
                {
                    Gizmos.DrawWireCube(m_tempArray[i], Vector3.one * m_gizmosSize);
                }
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
}
