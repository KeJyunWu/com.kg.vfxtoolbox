using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[ExecuteAlways]
public class SimpleBlit : MonoBehaviour
{
    [SerializeField] Material m_mat;
    [SerializeField] RenderTexture m_rt;

    [SerializeField]
    float m_interval = 0;

    float m_timeStamp;

    // Update is called once per frame
    void Update()
    {
        if(m_mat != null && m_rt != null && Time.time> m_timeStamp + m_interval)
        {
            m_timeStamp = Time.time;
            Graphics.Blit(null, m_rt, m_mat);
        }
    }
}
