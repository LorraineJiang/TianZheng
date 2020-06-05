using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EdgeDetectMode
{
	Thin = 0,
	Thick = 1,
}
[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Edge Detection (Geometry)")]
public class EdgeDetectEffectNormals : PostEffectsBase
{

	public EdgeDetectMode mode = EdgeDetectMode.Thin;
	public float sensitivityDepth = 1.0f;
	public float sensitivityNormals = 1.0f;

	public float edgesOnly = 0.0f;
	public Color edgesOnlyBgColor = Color.white;

	public Shader edgeDetectShader;
	private Material edgeDetectMaterial = null;

	public bool CheckResources()
	{
		CheckSupport(true);

		edgeDetectMaterial = CheckShaderAndCreateMaterial(edgeDetectShader, edgeDetectMaterial);

		if (!isSupported)
			ReportAutoDisable();
		return isSupported;
	}

	[ImageEffectOpaque]
	public void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (CheckResources() == false)
		{
			Graphics.Blit(source, destination);
			return;
		}

		Vector2 sensitivity = new Vector2(sensitivityDepth, sensitivityNormals);

		source.filterMode = FilterMode.Point;

		edgeDetectMaterial.SetVector("sensitivity", new Vector4(sensitivity.x, sensitivity.y, 1.0f, sensitivity.y));
		edgeDetectMaterial.SetFloat("_BgFade", edgesOnly);

		Vector4 vecCol = edgesOnlyBgColor;
		edgeDetectMaterial.SetVector("_BgColor", vecCol);

		if (mode == EdgeDetectMode.Thin)
		{
			Graphics.Blit(source, destination, edgeDetectMaterial, 0);
		}
		else
		{
			Graphics.Blit(source, destination, edgeDetectMaterial, 1);
		}
	}
}
