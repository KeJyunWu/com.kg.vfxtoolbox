using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
using UnityEngine.Events;

namespace UltraCombos.VFXToolBox
{
	[System.Serializable]
	public class LayerSetting
	{
		public BlendMode m_blendMode;
		public Texture m_texture;
	}

    public class TextureMixer : MonoBehaviour
    {
        const int READ = 0;
        const int WRITE = 1;
        const int NUM_THREADS = 8;

		public RenderTexture Result { get => m_result != null ? m_result : null; }

		[SerializeField, TitleGroup( "Stsyem Parameter" ), HideIf( "@UnityEngine.Application.isPlaying == true" )]
		Dimention m_dimention;

		[SerializeField, HideIf( "@UnityEngine.Application.isPlaying == true || m_dimention == Dimention.Two" ), LabelText( "Resolution" )]
		Vector3Int m_resolution_3d = new Vector3Int(128, 128, 128);
        public Vector3Int Resolution_3d { get => m_resolution_3d; set => m_resolution_3d = value; }

		[SerializeField, HideIf( "@UnityEngine.Application.isPlaying == true || m_dimention == Dimention.Three" ), LabelText( "Resolution")]
		Vector2Int m_resolution_2d = new Vector2Int( 128, 128 );
		public Vector2Int Resolution_2d { get => m_resolution_2d; set => m_resolution_2d = value; }

		[SerializeField]
        List<LayerSetting> m_layers = new List<LayerSetting>();
        public List<LayerSetting> Layers { get => m_layers; set => m_layers = value ; }

        [TitleGroup("Event")]
        public UnityEvent<RenderTexture> OnEvent = new UnityEvent<RenderTexture>();

        [TitleGroup("Debug")]
        public Material m_viewerMat;

        [ShowIf("m_viewerMat"), Indent, LabelText("Property Name")]
        public string m_viewerMatPropertyName = "_Texture3D";


        [SerializeField,HideInInspector]
        ComputeShader m_shader;
		[SerializeField]
		RenderTexture[] m_swapBuffer;
		RenderTexture m_result;
		int m_initKernel;
        int m_coreKernel;
		int m_validSourceNum;

		RenderTexture CreateRT()
		{
			return m_dimention == Dimention.Three ?
					RenderTextureUtil.Allocate( m_resolution_3d.x, m_resolution_3d.y, 0, m_resolution_3d.z, RenderTextureFormat.ARGBFloat, FilterMode.Point ) :
					RenderTextureUtil.Allocate( m_resolution_2d.x, m_resolution_2d.y, RenderTextureFormat.ARGBFloat, FilterMode.Point );
		}

        void BufferCheck()
        {
            bool _b = false;
            if (m_swapBuffer == null || m_swapBuffer.Length == 0)
            {
                m_swapBuffer = new RenderTexture[2];
                _b = true;
            }
            else
            {
				//Debug.Log( m_swapBuffer.Length);
                if (m_swapBuffer[READ].width != ( m_dimention == Dimention.Three ? m_resolution_3d.x : m_resolution_2d.x) )
                {
                    m_swapBuffer[READ]?.Release();
                    m_swapBuffer[WRITE]?.Release();
					m_result?.Release();
					_b = true;
                }
            }

            if(_b)
            {
				m_swapBuffer[READ] = CreateRT();
				m_swapBuffer[WRITE] = CreateRT();
				m_result = CreateRT();
			}
        }

        void Start()
        {
            m_initKernel = m_shader.FindKernel("Init");
            m_coreKernel = m_shader.FindKernel("Core");

			m_shader.EnableKeyword( m_dimention == Dimention.Three ? "_ThreeDimention" : "_TwoDimention" );
			m_shader.DisableKeyword( m_dimention == Dimention.Three ? "_TwoDimention" : "_ThreeDimention" );
		}

		void Update()
        {
            if (m_layers != null && m_layers.Count != 0)
            {
                BufferCheck();
                var _filter =
                    from _layer in m_layers
                    where m_dimention == Dimention.Two ?
					_layer != null && ( _layer.m_texture as Texture2D != null || (_layer.m_texture as RenderTexture !=null && _layer.m_texture.dimension == UnityEngine.Rendering.TextureDimension.Tex2D)) :
					_layer != null && (_layer.m_texture as Texture3D != null || (_layer.m_texture as RenderTexture != null && _layer.m_texture.dimension == UnityEngine.Rendering.TextureDimension.Tex3D))
					select _layer;

                List<LayerSetting> _data = _filter.ToList();
				m_validSourceNum = _data.Count;

				if (m_swapBuffer[READ] != null)
                {
                    int m_dispatchX = Mathf.CeilToInt(m_swapBuffer[READ].width / NUM_THREADS);
                    int m_dispatchY = Mathf.CeilToInt(m_swapBuffer[READ].height / NUM_THREADS);
                    int m_dispatchZ = Mathf.Max(Mathf.CeilToInt(m_swapBuffer[READ].volumeDepth / NUM_THREADS),1);

					m_shader.SetInt( "m_dimention", m_dimention == Dimention.Two ? 2 : 3 );
					string _suffix = m_dimention == Dimention.Two ? "_2d" : "_3d";

					//Init
					m_shader.SetTexture(m_initKernel, "m_output"+ _suffix, m_swapBuffer[READ]);
                    m_shader.Dispatch(m_initKernel, m_dispatchX, m_dispatchY, m_dispatchZ);

					//Core
					for (var i = 0; i < _data.Count; i++)
                    {
						m_shader.SetInt( "m_blendMode", (int)_data[i].m_blendMode);
						m_shader.SetTexture(m_coreKernel, "m_intput1"+ _suffix, m_swapBuffer[READ]);
						m_shader.SetTexture( m_coreKernel, "m_intput2"+ _suffix, _data[i].m_texture );
                        m_shader.SetTexture(m_coreKernel, "m_output"+ _suffix, m_swapBuffer[WRITE]);
                        m_shader.Dispatch(m_coreKernel, m_dispatchX, m_dispatchY, m_dispatchZ);
                        RenderTextureUtil.Swap(m_swapBuffer);
					}

					Graphics.Blit(m_swapBuffer[READ], m_result );
                }

                OnEvent?.Invoke(Result);

                if (m_viewerMat)
                {
                    if (!m_viewerMat.HasProperty(m_viewerMatPropertyName))
                        Debug.LogError(string.Format("Material output doesn't have property {0}", m_viewerMatPropertyName));
                    else
                        m_viewerMat.SetTexture(m_viewerMatPropertyName, Result);
                }
            }
        }

        void OnDestroy()
        {
            if (m_swapBuffer != null)
            {
                for (var i = 0; i < m_swapBuffer.Length; i++)
                    m_swapBuffer[i].Release();
            }
			m_result?.Release();

		}
    }
}
