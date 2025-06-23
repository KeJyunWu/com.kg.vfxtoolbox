using UnityEngine;
using System;

public class GrayscaleToVectorField : MonoBehaviour
{

    public ComputeShader m_computeShader;
    public Texture m_grayscaleMap;
    public Texture GrayscaleMap { set => m_grayscaleMap = value;  }
    public RenderTexture m_outputResult;
    public Vector2 m_velocityNormalizeRange = new Vector2(-1, 1);
    public float m_intensity = 1;
    public float m_windowSize = 10;


    // Update is called once per frame
    void Update()
    {
        if (m_outputResult == null || m_computeShader == null || m_grayscaleMap == null)
            return;

        m_computeShader.GetKernelThreadGroupSizes(0, out uint x, out uint y, out uint z);
        int threadGroupsX = (m_grayscaleMap.width + (int)x - 1) / (int)x;
        int threadGroupsY = (m_grayscaleMap.height + (int)y - 1) / (int)y;
        int threadGroupsZ = (int)z;

        m_computeShader.SetFloat("Intensity", m_intensity);
        m_computeShader.SetFloat("WindowSize", m_windowSize);
        m_computeShader.SetVector("ValueRange", m_velocityNormalizeRange);
        m_computeShader.SetTexture(0, "GrayMap", m_grayscaleMap);
        m_computeShader.SetTexture(0, "VecFieldMap", m_outputResult);
        m_computeShader.Dispatch(0, threadGroupsX, threadGroupsY, threadGroupsZ);
    }
}
