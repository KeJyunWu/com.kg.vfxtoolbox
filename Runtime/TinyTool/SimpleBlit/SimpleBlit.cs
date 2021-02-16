using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class SimpleBlit : MonoBehaviour
{
    [SerializeField] Material m_mat;
    [SerializeField] RenderTexture m_rt;

    // Update is called once per frame
    void Update()
    {
        if(m_mat != null && m_rt != null)
        {
            Graphics.Blit(null, m_rt, m_mat);
        }
    }
}
