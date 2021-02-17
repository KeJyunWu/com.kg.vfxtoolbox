using UnityEngine;
using Sirenix.OdinInspector;

namespace UltraCombos.VFXToolBox
{
    public class PointCacherToVolume : MonoBehaviour
    {
        [SerializeField] int m_resolution = 128;
        public int Resolution { get => m_resolution; set => m_resolution = value; }

        [SerializeField] PointCacher m_pointCacher;
        public PointCacher PointCacher { get => m_pointCacher; set => m_pointCacher = value; }

        [SerializeField] Transform m_container;
        public Transform Container { get => m_container; set => m_container = value; }

        [SerializeField] ComputeShader m_computeShader;

        [TitleGroup("Debug")]
        public bool m_drawContainer = false;

        RenderTexture m_result;

        // Start is called before the first frame update
        void Start()
        { 
            m_result = new RenderTexture(m_resolution, m_resolution,
                    0, RenderTextureFormat.ARGBFloat);
            m_result.dimension = UnityEngine.Rendering.TextureDimension.Tex3D;
            m_result.enableRandomWrite = true;
            m_result.useMipMap = false;
            m_result.volumeDepth = m_resolution;
            m_result.Create();
        }

        // Update is called once per frame
        void Update()
        {
            int _injectKernel = m_computeShader.FindKernel("Inject");
            m_computeShader.SetTexture(_injectKernel, "m_result", m_result);
            m_computeShader.SetBuffer(_injectKernel, "m_velocityBuffer", m_pointCacher.VelocityBuffer);
            m_computeShader.SetBuffer(_injectKernel, "m_positionSampledBuffer", m_pointCacher.PositionSampledBuffer);
            m_computeShader.SetVector("m_containerCenter", m_container.position);
            m_computeShader.SetVector("m_containerScale", m_container.localScale);
            m_computeShader.Dispatch(_injectKernel, Mathf.CeilToInt(m_pointCacher.PointCount/8.0f),1,1);


            //int _deacyKernel = m_computeShader.FindKernel("Deacy");
            //m_computeShader.SetTexture(_deacyKernel, "m_result", m_result);


            //m_computeShader.Dispatch()
        }

        private void OnDrawGizmos()
        {
            if (m_drawContainer && m_container != null)
            {
                Gizmos.DrawWireCube(m_container.transform.position, m_container.transform.localScale);
            }
        }

        private void OnDestroy()
        {
            m_result?.Release();
            m_result = null;
        }
    }
}