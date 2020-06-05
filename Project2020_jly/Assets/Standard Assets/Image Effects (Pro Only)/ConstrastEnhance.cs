using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Contrast Enhance (Unsharp Mask)")]
public class ContrastEnhance : PostEffectsBase
{
	public float intensity = 0.5f;
	public float threshhold = 0.0f;

	private Material separableBlurMaterial;
	private Material contrastCompositeMaterial;

	public float blurSpread = 1.0f;

	public Shader separableBlurShader = null;
	public Shader contrastCompositeShader = null;

	public bool CheckResources()
	{
		CheckSupport(false);

		contrastCompositeMaterial = CheckShaderAndCreateMaterial(contrastCompositeShader, contrastCompositeMaterial);
		separableBlurMaterial = CheckShaderAndCreateMaterial(separableBlurShader, separableBlurMaterial);

		if (!isSupported)
			ReportAutoDisable();
		return isSupported;
	}

	public void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (CheckResources() == false)
		{
			Graphics.Blit(source, destination);
			return;
		}

		RenderTexture halfRezColor = RenderTexture.GetTemporary(source.width / 2, source.height / 2, 0);
		RenderTexture quarterRezColor = RenderTexture.GetTemporary(source.width / 4, source.height / 4, 0);
		RenderTexture secondQuarterRezColor = RenderTexture.GetTemporary(source.width / 4, source.height / 4, 0);

		// ddownsample

		Graphics.Blit(source, halfRezColor);
		Graphics.Blit(halfRezColor, quarterRezColor);

		// blur

		separableBlurMaterial.SetVector("offsets", new Vector4(0.0f, (blurSpread * 1.0f) / quarterRezColor.height, 0.0f, 0.0f));
		Graphics.Blit(quarterRezColor, secondQuarterRezColor, separableBlurMaterial);
		separableBlurMaterial.SetVector("offsets", new Vector4((blurSpread * 1.0f) / quarterRezColor.width, 0.0f, 0.0f, 0.0f));
		Graphics.Blit(secondQuarterRezColor, quarterRezColor, separableBlurMaterial);

		// composite

		contrastCompositeMaterial.SetTexture("_MainTexBlurred", quarterRezColor);
		contrastCompositeMaterial.SetFloat("intensity", intensity);
		contrastCompositeMaterial.SetFloat("threshhold", threshhold);
		Graphics.Blit(source, destination, contrastCompositeMaterial);

		RenderTexture.ReleaseTemporary(halfRezColor);
		RenderTexture.ReleaseTemporary(quarterRezColor);
		RenderTexture.ReleaseTemporary(secondQuarterRezColor);
	}
}
