using UnityEngine;

public class MorphingEffect : MonoBehaviour
{
    [SerializeField, HideInInspector]
    ComputeShader m_shader;

    [SerializeField]
    RenderTexture m_source;

    [SerializeField]
    RenderTexture m_output;

    [SerializeField]
    float m_tweenSpeed = 1.0f;

    void Update()
    {
        if (m_shader == null || m_source == null || m_output == null)
            return;

        m_shader.SetFloat("DeltaTime", Time.deltaTime);
        m_shader.SetFloat("TweenSpeed", m_tweenSpeed);
        m_shader.SetTexture(0, "Source", m_source);
        m_shader.SetTexture(0, "Output", m_output);
        m_shader.Dispatch(0, 1024 / 32, 1024 / 32, 1);
    }
}
