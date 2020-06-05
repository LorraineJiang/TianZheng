using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Loxodon.Framework.Views;
using Loxodon.Framework.Views.Animations;
using DG.Tweening;

/// <summary>
/// loxodon框架下的窗口缩放动画，适用于开启和关闭
/// </summary>
public class ScaleWindowAnimation : UIAnimation
{
    public Vector3 from;
    public Vector3 to;
	public float duration = 0.3f;
	private IUIView view;

	void OnEnable()
	{
		this.view = this.GetComponent<IUIView>();
		switch (this.AnimationType)
		{
			case AnimationType.EnterAnimation:
				this.view.EnterAnimation = this;
				break;
			case AnimationType.ExitAnimation:
				this.view.ExitAnimation = this;
				break;
			case AnimationType.ActivationAnimation:
				if (this.view is IWindowView)
					(this.view as IWindowView).ActivationAnimation = this;
				break;
			case AnimationType.PassivationAnimation:
				if (this.view is IWindowView)
					(this.view as IWindowView).PassivationAnimation = this;
				break;
		}

		if (this.AnimationType == AnimationType.ActivationAnimation || this.AnimationType == AnimationType.EnterAnimation)
		{
			this.view.CanvasGroup.gameObject.transform.localScale = from;
		}
	}
	public override IAnimation Play()
    {
		this.view.CanvasGroup.transform.localScale = from;
		this.view.CanvasGroup.transform.DOScale(to, duration).
			OnStart(this.OnStart).OnComplete(this.OnEnd).Play();
		return this;
	}
}
