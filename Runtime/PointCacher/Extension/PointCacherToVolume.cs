using UnityEngine;
using Sirenix.OdinInspector;

namespace UltraCombos.VFXToolBox
{
    public class PointCacherToVolume : MonoBehaviour
    {
        [TitleGroup("System")]
        [SerializeField] RenderTexture m_result;
        public RenderTexture Result { get => m_result; set => m_result = value; }

        [SerializeField] int m_resolution = 128;
        public int Resolution { get => m_resolution; set => m_resolution = value; }

        [SerializeField] PointCacher m_pointCacher;
        public PointCacher PointCacher { get => m_pointCacher; set => m_pointCacher = value; }

        [SerializeField] Transform m_container;
        public Transform Container { get => m_container; set => m_container = value; }

        [TitleGroup("Velocity Volume")]
        [SerializeField, Range(0, 4)] float m_drag = 0.1f;
        public float Drag { get => m_drag; set => m_drag = value; }

        [TitleGroup("Debug")]
        public bool m_drawContainer = false;
        public Material m_viewerMat;
        [ShowIf("m_viewerMat"), Indent, LabelText("Property Name")]
        public string m_viewerMatPropertyName = "_Texture3D";

        [SerializeField, HideInInspector] ComputeShader m_computeShader;
        // Start is called before the first frame update
        void Start()
        {
            if (m_result == null || m_result.dimension != UnityEngine.Rendering.TextureDimension.Tex3D)
            {
                m_result = new RenderTexture(m_resolution, m_resolution,
                        0, RenderTextureFormat.ARGBFloat);
                m_result.dimension = UnityEngine.Rendering.TextureDimension.Tex3D;
                m_result.enableRandomWrite = true;
                m_result.useMipMap = false;
                m_result.volumeDepth = m_resolution;
                m_result.Create();
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (m_pointCacher == null || m_pointCacher.VelocityBuffer == null || m_pointCacher.VelocityBuffer.count ==0)
                return;

            float _drag = Mathf.Exp(-m_drag * Time.deltaTime);

            int _injectKernel = m_computeShader.FindKernel("Inject");
            m_computeShader.SetTexture(_injectKernel, "m_result", m_result);
            m_computeShader.SetBuffer(_injectKernel, "m_velocityBuffer", m_pointCacher.VelocityBuffer);
            m_computeShader.SetBuffer(_injectKernel, "m_positionSampledBuffer", m_pointCacher.PositionSampledBuffer);
            m_computeShader.SetVector("m_containerCenter", m_container.position);
            m_computeShader.SetVector("m_containerScale", m_container.localScale);
            m_computeShader.Dispatch(_injectKernel, Mathf.CeilToInt(m_pointCacher.PointCount/8.0f),1,1);

            int _decayKernel = m_computeShader.FindKernel("Decay");
            m_computeShader.SetFloat("m_drag", _drag);
            m_computeShader.SetTexture(_decayKernel, "m_result", m_result);
            m_computeShader.Dispatch(_decayKernel, Mathf.CeilToInt(m_result.width / 8.0f), Mathf.CeilToInt(m_result.height / 8.0f), Mathf.CeilToInt(m_result.volumeDepth / 8.0f));

            if (m_viewerMat)
            {
                if (!m_viewerMat.HasProperty(m_viewerMatPropertyName))
                    Debug.LogError(string.Format("Material output doesn't have property {0}", m_viewerMatPropertyName));
                else
                    m_viewerMat.SetTexture(m_viewerMatPropertyName, m_result);
            }
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

#if UNITY_EDITOR
        [OnInspectorGUI]
        private void OnInspectorGUI()
        {
            UnityEditor.EditorGUILayout.Space();
            UnityEditor.EditorGUILayout.HelpBox("If result(Output RT) is not specified or the format of it is not 3D volume, the script will regenerate one according to the resolution on the inspector", UnityEditor.MessageType.Info);
        }
#endif
    }
}