using UnityEngine;
using Unity.Burst;
using System.Linq;
using Unity.Collections;
using Unity.Jobs;
using Sirenix.OdinInspector;
using System.Collections.Generic;

namespace UltraCombos.VFXToolBox
{
    public enum DataType
    {
        Raw,
        Processed
    }

    public enum MatrixType
    {
        Target,
        Self,
        None
    }

    public class PointCacher : MonoBehaviour
    {
        [TitleGroup("System")]
        [SerializeField] int m_pointCount = 5;
        public int PointCount { get => m_pointCount; set => m_pointCount = value; }

        [SerializeField] List<SkinnedMeshRenderer> m_skinnedMeshes = new List<SkinnedMeshRenderer>();
        public List<SkinnedMeshRenderer> SkinnedMeshes { get => m_skinnedMeshes; set => m_skinnedMeshes = value; }

        [SerializeField] List<MeshFilter> m_meshes = new List<MeshFilter>();
        public List<MeshFilter> Meshes { get => m_meshes; set => m_meshes = value; }

        [SerializeField] bool m_requestBufferData = false;
        public bool RequestBufferData { get => m_requestBufferData; set => m_requestBufferData = value; }

        [SerializeField] bool m_requestRTData = false;
        public bool RequestRTData { get => m_requestRTData; set => m_requestRTData = value; }

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

        [TitleGroup("Debug"), LabelText("Draw Gizmos")]
        public bool m_drawDebugGizmos = false;
        [ShowIf("m_drawDebugGizmos"), Indent, LabelText("Data Type")]
        public DataType m_debugDataType = DataType.Raw;
        [ShowIf("m_drawDebugGizmos"), Indent, LabelText("Size")]
        public float m_gizmosSize = 0.1f;
        [ShowIf("m_drawDebugGizmos"), Indent, LabelText("LineLength")]
        public float m_gizmosLineLength = 2;

        [SerializeField, HideInInspector] ComputeShader m_pointCacherCS;

        //Result Stored Map
        RenderTexture m_positionMap;
        public RenderTexture PositionMap { get => m_positionMap;}
        RenderTexture m_velocityMap;
        public RenderTexture VelocityMap { get => m_velocityMap; }
        RenderTexture m_normalMap;
        public RenderTexture NormalMap { get => m_normalMap; }

        //Result Stored Buffer
        ComputeBuffer m_positionSampledBuffer;
        public ComputeBuffer PositionSampledBuffer { get => m_positionSampledBuffer; }
        ComputeBuffer m_velocityBuffer;
        public ComputeBuffer VelocityBuffer { get => m_velocityBuffer; }
        ComputeBuffer m_normalSampledBuffer;
        public ComputeBuffer NormalSampledBuffer { get => m_normalSampledBuffer; }

        //Compute Data Buffer
        ComputeBuffer m_samplePointsBuffer;
        ComputeBuffer m_rawIndexBuffer;
        ComputeBuffer m_rawNormalBuffer;
        (ComputeBuffer m_positionBuffer1, ComputeBuffer m_positionBuffer2) m_rawPositionBuffer;

        Mesh m_tempMesh;
        Vector3[] m_tempRawPositionDebugArray;
        Vector3[] m_tempRawNormalDebugArray;
        Vector3[] m_tempProcessedPositionDebugArray;
        Vector3[] m_tempProcessedNormalDebugArray;
        Vector3[] m_positionSampledArray;
        public Vector3[] PositionSampledArray { get { if (PositionSampledBuffer != null) { PositionSampledBuffer.GetData(m_positionSampledArray); return m_positionSampledArray; } else { return null; } } }

        private void OnEnable() => InitializeInternals();
        private void OnDisable() => DisposeInternals();

        public void Reset()
        {
            DisposeInternals();
        }

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
                Matrix4x4 _OWmatrix = m_matrixType == MatrixType.Target && m_targetTransform != null ? m_targetTransform.localToWorldMatrix : source.transform.localToWorldMatrix;
                Matrix4x4 _WOmatrix = m_matrixType == MatrixType.Target && m_targetTransform != null ? m_targetTransform.worldToLocalMatrix : source.transform.worldToLocalMatrix;
                var _offset = MeshBake(source, _vertexOffset, _indexOffset, _OWmatrix, _WOmatrix);
                _vertexOffset += _offset._vOffset;
                _indexOffset += _offset._iOffset;
            }

            foreach (var source in m_skinnedMeshes)
            {
                Matrix4x4 _OWmatrix = m_matrixType == MatrixType.Target && m_targetTransform != null ? m_targetTransform.localToWorldMatrix : source.transform.localToWorldMatrix;
                Matrix4x4 _WOmatrix = m_matrixType == MatrixType.Target && m_targetTransform != null ? m_targetTransform.worldToLocalMatrix : source.transform.worldToLocalMatrix;
                var _offset = SkinnedMeshBake(source, _vertexOffset, _indexOffset, _OWmatrix, _WOmatrix);
                _vertexOffset += _offset._vOffset;
                _indexOffset += _offset._iOffset;
            }

            TransferData();

            (m_rawPositionBuffer.m_positionBuffer1, m_rawPositionBuffer.m_positionBuffer2) = (m_rawPositionBuffer.m_positionBuffer2, m_rawPositionBuffer.m_positionBuffer1);
        }

        (int _vOffset, int _iOffset) SkinnedMeshBake(SkinnedMeshRenderer _source, int _vertexOffset, int _indexOffset, Matrix4x4 _objectToWorld, Matrix4x4 _worldToObject)
        {
            _source.BakeMesh(m_tempMesh);
            return BakeSource(m_tempMesh, _vertexOffset, _indexOffset, _objectToWorld, _worldToObject);
        }

        (int _vOffset, int _iOffset) MeshBake(MeshFilter _source, int _vertexOffset, int _indexOffset, Matrix4x4 _objectToWorld, Matrix4x4 _worldToObject)
        {
            return BakeSource(_source.mesh, _vertexOffset, _indexOffset, _objectToWorld, _worldToObject);
        }

        (int _vOffset, int _iOffset) BakeSource(Mesh _mesh, int _vertexOffset, int _indexOffset, Matrix4x4 _objectToWorld, Matrix4x4 _worldToObject)
        {
            using (var dataArray = Mesh.AcquireReadOnlyMeshData(_mesh))
            {
                var _data = dataArray[0];
                var _vcount = _data.vertexCount;
                var _icount = _data.GetSubMesh(0).indexCount;

                using (var pos = MemoryUtil.TempJobArray<Vector3>(_vcount))
                using (var index = MemoryUtil.TempJobArray<int>(_icount))
                using (var nrm = MemoryUtil.TempJobArray<Vector3>(_vcount))
                {
                    _data.GetVertices(pos);
                    _data.GetNormals(nrm);
                    _data.GetIndices(index, 0);

                    new ConcatenationJob
                    { m_output = index, m_indexOffset = _vertexOffset }.Run();

                    m_rawPositionBuffer.m_positionBuffer1.SetData(pos, 0, _vertexOffset, _vcount);
                    m_rawIndexBuffer.SetData(index, 0, _indexOffset, _icount);
                    m_rawNormalBuffer.SetData(nrm, 0, _vertexOffset, _vcount);

                    m_pointCacherCS.SetInt("m_offset", _vertexOffset);
                    m_pointCacherCS.SetInt("m_count", _vcount);
                    m_pointCacherCS.SetMatrix("m_objectToWorldMatrix", _objectToWorld);
                    m_pointCacherCS.SetMatrix("m_worldToObjectMatrix", _worldToObject);

                    int _kernel = m_pointCacherCS.FindKernel("Transform");
                    m_pointCacherCS.SetInt("m_matrixType", (int)m_matrixType);
                    m_pointCacherCS.SetVector("m_positionOffset", m_offset);
                    m_pointCacherCS.SetVector("m_positionScale", m_scale);
                    m_pointCacherCS.SetVector("m_positionFlip", new Vector3(m_flipX?-1:1, m_flipY ? -1 : 1, m_flipZ ? -1 : 1));
                    m_pointCacherCS.SetBuffer(_kernel, "PositionTransformBuffer", m_rawPositionBuffer.m_positionBuffer1);
                    m_pointCacherCS.SetBuffer(_kernel, "NormalTransformBuffer", m_rawNormalBuffer);
                    m_pointCacherCS.Dispatch(_kernel, Mathf.CeilToInt(_vcount / 8.0f), 1, 1);
                    return (_vcount, _icount);
                }
            }
        }

        void SetTransferKernel(int _kernel)
        {
            m_pointCacherCS.SetBuffer(_kernel, "SamplePoints", m_samplePointsBuffer);
            m_pointCacherCS.SetBuffer(_kernel, "PositionBuffer1", m_rawPositionBuffer.m_positionBuffer1);
            m_pointCacherCS.SetBuffer(_kernel, "PositionBuffer2", m_rawPositionBuffer.m_positionBuffer2);
            m_pointCacherCS.SetBuffer(_kernel, "NormalBuffer", m_rawNormalBuffer);

            m_pointCacherCS.SetBuffer(_kernel, "PositionSampledBuffer", m_positionSampledBuffer);
            m_pointCacherCS.SetBuffer(_kernel, "NormalSampledBuffer", m_normalSampledBuffer);
            m_pointCacherCS.SetBuffer(_kernel, "VelocityBuffer", m_velocityBuffer);

            m_pointCacherCS.SetTexture(_kernel, "PositionMap", m_positionMap);
            m_pointCacherCS.SetTexture(_kernel, "VelocityMap", m_velocityMap);
            m_pointCacherCS.SetTexture(_kernel, "NormalMap", m_normalMap);
        }

        void TransferData()
        {
            m_pointCacherCS.SetInt("SampleCount", m_pointCount);
            m_pointCacherCS.SetFloat("FrameRate", 1 / Time.deltaTime);

            if (m_requestRTData)
            {
                int _kernel = m_pointCacherCS.FindKernel("TransferData_RT");
                SetTransferKernel(_kernel);
                var width = m_positionMap.width;
                var height = m_positionMap.height;
                m_pointCacherCS.Dispatch(_kernel, width / 8, height / 8, 1);
            }

            if(m_requestBufferData)
            {
                int _kernel = m_pointCacherCS.FindKernel("TransferData_Buffer");
                SetTransferKernel(_kernel);
                m_pointCacherCS.Dispatch(_kernel, m_pointCount/8, 1, 1);
            }
        }

        void InitializeInternals()
        {
            if (m_meshes.Count == 0 && m_skinnedMeshes.Count == 0)
                return;

            using (var mesh = new CombinedMesh(m_meshes.Select(smr => smr.sharedMesh).ToArray(), m_skinnedMeshes))
            {
                using (var points = SamplePointGenerator.Generate
                      (mesh, m_pointCount))
                {
                    m_samplePointsBuffer = new ComputeBuffer(m_pointCount, SamplePoint.SizeInByte);
                    m_samplePointsBuffer.SetData(points);
                }

                var _vcount = mesh.Vertices.Length;
                m_rawPositionBuffer.m_positionBuffer1 = new ComputeBuffer(mesh.Vertices.Length, sizeof(float) * 3);
                m_rawPositionBuffer.m_positionBuffer2 = new ComputeBuffer(mesh.Vertices.Length, sizeof(float) * 3);
                m_rawNormalBuffer = new ComputeBuffer(mesh.Vertices.Length, sizeof(float) * 3);
                m_rawIndexBuffer = new ComputeBuffer(mesh.Indices.Length, sizeof(int));
                m_velocityBuffer = new ComputeBuffer(m_pointCount, sizeof(float) * 3);
                m_positionSampledBuffer = new ComputeBuffer(m_pointCount, sizeof(float) * 3);
                m_normalSampledBuffer = new ComputeBuffer(m_pointCount, sizeof(float) * 3);
                m_positionSampledArray = new Vector3[m_pointCount];

                m_tempRawPositionDebugArray = new Vector3[mesh.Vertices.Length];
                m_tempRawNormalDebugArray = new Vector3[mesh.Vertices.Length];
                m_tempProcessedPositionDebugArray = new Vector3[m_pointCount];
                m_tempProcessedNormalDebugArray = new Vector3[m_pointCount];
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

            m_positionSampledBuffer?.Dispose();
            m_positionSampledBuffer = null;

            m_normalSampledBuffer?.Dispose();
            m_normalSampledBuffer = null;

            m_velocityBuffer?.Dispose();
            m_velocityBuffer = null;

            m_rawNormalBuffer?.Dispose();
            m_rawNormalBuffer = null;

            m_rawPositionBuffer.m_positionBuffer1?.Dispose();
            m_rawPositionBuffer.m_positionBuffer1 = null;

            m_rawPositionBuffer.m_positionBuffer2?.Dispose();
            m_rawPositionBuffer.m_positionBuffer2 = null;

            m_rawIndexBuffer?.Dispose();
            m_rawIndexBuffer = null;

            ObjectUtil.Destroy(m_positionMap);
            m_positionMap = null;

            ObjectUtil.Destroy(m_velocityMap);
            m_velocityMap = null;

            ObjectUtil.Destroy(m_normalMap);
            m_normalMap = null;

            ObjectUtil.Destroy(m_tempMesh);
            m_tempMesh = null;

            m_tempRawPositionDebugArray = null;
            m_tempRawNormalDebugArray = null;
            m_tempProcessedPositionDebugArray = null;
            m_tempProcessedNormalDebugArray = null;
            m_positionSampledArray = null;
        }

        void RenderGizmos(ref Vector3[] _posArray, ref Vector3[] _normalArray)
        {
            for (var i = 0; i < _posArray.Length; i++)
            {
                Vector3 _p = _posArray[i];
                Gizmos.DrawWireCube(_p, Vector3.one * m_gizmosSize);
                Vector3 _n = _normalArray[i];
                Gizmos.DrawLine(_p, _p + _n * m_gizmosLineLength);
            }
        }

        private void OnDrawGizmos()
        {
            if (m_rawPositionBuffer.m_positionBuffer1 != null && m_drawDebugGizmos)
            {
                if (m_debugDataType == DataType.Raw)
                {
                    m_rawPositionBuffer.m_positionBuffer1.GetData(m_tempRawPositionDebugArray);
                    m_rawNormalBuffer.GetData(m_tempRawNormalDebugArray);
                    RenderGizmos(ref m_tempRawPositionDebugArray, ref m_tempRawNormalDebugArray);
                }
                else
                {
                    m_positionSampledBuffer.GetData(m_tempProcessedPositionDebugArray);
                    m_normalSampledBuffer.GetData(m_tempProcessedNormalDebugArray);
                    RenderGizmos(ref m_tempProcessedPositionDebugArray, ref m_tempProcessedNormalDebugArray);
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
