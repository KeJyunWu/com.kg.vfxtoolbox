using UnityEngine;
using System.Collections;
using Sirenix.OdinInspector;

[ExecuteAlways]
public class GaussianBlur : MonoBehaviour 
{
	public enum Algo
	{
		TWO_PASS,
		TWO_PASS_LINEAR_SAMPLING
	}

	public enum Quality
	{
		LITTLE_KERNEL,
		MEDIUM_KERNEL,
		BIG_KERNEL
	};

	public Algo algo;
	public Quality quality;
	public float sigma = 10f;

	public Texture m_input;
	public RenderTexture m_output;
	public RenderTexture m_output2;

	[SerializeField,ReadOnly]
	Shader m_Shader;
	[SerializeField, ReadOnly]
	Material m_Material;

	private void OnValidate()
	{
		Init ();
	}

	private void OnEnable()
	{
		Init ();
	}

	private void Init()
	{
		switch (algo)
		{
			case Algo.TWO_PASS: m_Shader = Shader.Find("hidden/two_pass_gaussian_blur"); break;
			case Algo.TWO_PASS_LINEAR_SAMPLING: m_Shader = Shader.Find("hidden/two_pass_linear_sampling_gaussian_blur"); break;
		}
		m_Material = new Material (m_Shader);
		m_Material.EnableKeyword (quality.ToString ());
	}

	private void Update()
	{
		if (m_input == null || m_output == null || m_output2 == null)
			return;

		m_Material.SetFloat("_Sigma",sigma);
		m_Material.SetVector("_TexelSize", new Vector2(1.0f / m_input.width, 1.0f/m_input.height));
		Graphics.Blit(m_input, m_output, m_Material, 0);
		Graphics.Blit(m_output, m_output2, m_Material,1);
	}
}
