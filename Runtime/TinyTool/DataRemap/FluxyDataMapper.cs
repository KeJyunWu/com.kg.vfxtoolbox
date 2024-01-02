using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FluxyDataMapper : MonoBehaviour
{
    [SerializeField]
    Texture m_soruce;

    [SerializeField]
    RenderTexture m_output;

    [SerializeField]
    Texture m_mask;

    [SerializeField]
    float m_forceScale = 1;

    [SerializeField, HideInInspector]
    Shader m_shader;

    Material m_mat;

    private void Awake()
    {
        m_mat = new Material(m_shader);
    }

    // Update is called once per frame
    void Update()
    {
        m_mat.SetTexture("_Source", m_soruce);
        m_mat.SetTexture("_Mask", m_mask);
        m_mat.SetFloat("_ForceScale", m_forceScale);
        Graphics.Blit(m_soruce, m_output, m_mat);
    }
}
