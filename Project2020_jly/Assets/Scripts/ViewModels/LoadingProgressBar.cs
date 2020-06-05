using Loxodon.Framework.ViewModels;

/// <summary>
/// 加载进度条的vm
/// </summary>
public class LoadingProgressBar : ViewModelBase
{
	private float progress;
	private string tip;
	private bool enable;

	public bool Enable
	{
		get { return this.enable; }
		set { this.Set<bool>(ref this.enable, value, "Enable"); }
	}

	public float Progress
	{
		get { return this.progress; }
		set { this.Set<float>(ref this.progress, value, "Progress"); }
	}

	public string Tip
	{
		get { return this.tip; }
		set { this.Set<string>(ref this.tip, value, "Tip"); }
	}
}
