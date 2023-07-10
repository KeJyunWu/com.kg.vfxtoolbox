using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Feedback : MonoBehaviour
{
    [SerializeField]
    Vector2Int m_resolutions = new Vector2Int(500,500);

    [SerializeField]
    Texture m_source;

    [SerializeField]
    float m_decayFactor = 1;

    [SerializeField]
    RenderTexture m_outputResult;

    [SerializeField, HideInInspector]
    Material m_mat;

    Material m_matInstance;
    List<RenderTexture> m_rts = new List<RenderTexture>();

    RenderTexture CreateRT()
    {
        RenderTexture _rt = new RenderTexture(m_resolutions.x, m_resolutions.y, 0, RenderTextureFormat.ARGBFloat);
        _rt.enableRandomWrite = false;
        _rt.Create();
        return _rt;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_rts.Add(CreateRT());
        m_rts.Add(CreateRT());
        m_matInstance = new Material(m_mat);
    }

    // Update is called once per frame
    void Update()
    {
        m_matInstance.SetTexture("_Source", m_source);
        m_matInstance.SetTexture("_Source2", m_rts[1]);
        m_matInstance.SetFloat("_DecayFactor", m_decayFactor);
        m_matInstance.SetFloat("_DeltaTime", Time.deltaTime);
        Graphics.Blit(null, m_rts[0], m_matInstance);

        if (m_outputResult != null)
            Graphics.Blit(m_rts[0], m_outputResult);

        RenderTexture _r = m_rts[0];
        m_rts[0] = m_rts[1];
        m_rts[1] = _r;
    }

    private void OnDestroy()
    {
        for(var i=0; i< m_rts.Count; i++)
            m_rts[i].Release();
    }
}
