using UnityEngine;

public class FeedbackEffect : MonoBehaviour
{
    [SerializeField,HideInInspector]
    ComputeShader m_shader;

    [SerializeField]
    RenderTexture m_source;

    [SerializeField]
    RenderTexture m_output;

    [SerializeField]
    float m_injectionSpeed = 1;

    [SerializeField]
    float m_decaySpeed = 1;

    // Update is called once per frame
    void Update()
    {
        if (m_shader == null || m_source == null || m_output == null)
            return;

        m_shader.SetFloat("DeltaTime", Time.deltaTime);
        m_shader.SetFloat("InjectionSpeed", m_injectionSpeed);
        m_shader.SetFloat("DecaySpeed", m_decaySpeed);

        m_shader.SetTexture(0, "Source", m_source);
        m_shader.SetTexture(0, "Output", m_output);
        m_shader.Dispatch(0, 1024 / 32, 1024 / 32, 1);
        m_shader.SetTexture(1, "Output", m_output);
        m_shader.Dispatch(1, 1024 / 32, 1024 / 32, 1);
    }
}
