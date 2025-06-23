using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(SplineContainer))]
public class SplineToTexture : MonoBehaviour
{
    [SerializeField]
    RenderTexture m_outpurRT;

    SplineContainer m_SplineContainer;
    Texture2D m_dataMap;

    Texture2D TextureCreating()
    {
        Texture2D _tex = new Texture2D(m_outpurRT.width, 1, TextureFormat.RGBAFloat, false, false);
        _tex.filterMode = FilterMode.Point;
        _tex.Apply();
        return _tex;
    }

    void TextureInjection(Spline _spline)
    {
        if (m_dataMap == null)
            return;
        try
        {
            float _inv = 1f / (m_dataMap.width - 1);
            for (int i = 0; i < m_outpurRT.width; i++)
            {
                Vector3 _p = _spline.EvaluatePosition(i * _inv);
                _p += transform.position;
                //Debug.Log(i);
                m_dataMap.SetPixel(i, 0, new Color(_p.x, _p.y, _p.z,1));
            }
            m_dataMap.Apply();
            //Debug.Log("===============");
        }
        catch { }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (m_outpurRT != null)
        {
            m_SplineContainer = GetComponent<SplineContainer>();
            m_dataMap = TextureCreating();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (m_outpurRT != null)
        {
            Spline _s = m_SplineContainer.Spline;
            TextureInjection(m_SplineContainer.Spline);
            Graphics.Blit(m_dataMap, m_outpurRT);
        }
    }
}
