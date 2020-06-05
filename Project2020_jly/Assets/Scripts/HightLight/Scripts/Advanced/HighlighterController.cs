using UnityEngine;
using System.Collections;
using HighlightingSystem;

public class HighlighterController : MonoBehaviour
{
	public bool seeThrough = true;
	protected bool _seeThrough = true;

	protected Highlighter h;
	public Color highlightColor=Color.blue;
	// 
	protected void Awake()
	{
		h = GetComponent<Highlighter>();
		if (h == null) { h = gameObject.AddComponent<Highlighter>(); }
	}

	// 
	void OnEnable()
	{
		if (seeThrough) { h.SeeThroughOn(); }
		else { h.SeeThroughOff(); }
	}

	// 
	protected void Start() { }

	// 
	protected void Update()
	{
		if (_seeThrough != seeThrough)
		{
			_seeThrough = seeThrough;
			if (_seeThrough) { h.SeeThroughOn(); }
			else { h.SeeThroughOff(); }
		}

		// Fade in/out constant highlighting with button '1'
		//if (Input.GetKeyDown(KeyCode.Alpha1)) { h.ConstantSwitch(); }

		//// Turn on/off constant highlighting with button '2'
		//else if (Input.GetKeyDown(KeyCode.Alpha2)) { h.ConstantSwitchImmediate(); }
		
		//// Turn off all highlighting modes with button '3'
		//if (Input.GetKeyDown(KeyCode.Alpha3)) { h.Off(); }
	}

	// 
	public void MouseOver()
	{
		// Highlight object for one frame in case MouseOver event has arrived
		h.On(highlightColor);
	}

	// 
	public void Flashing()
	{
		// Switch flashing
		h.FlashingSwitch();
	}

	// 
	public void SeeThrough()
	{
		// Stop flashing
		h.SeeThroughSwitch();
	}
	/// <summary>
	/// 停止当前的高亮
	/// </summary>
	public void StopLighting()
	{
		h.Off();
	}
	/// <summary>
	/// 一个颜色高亮物体
	/// </summary>
	public void ConstantSwitchImmediate()
	{
		h.ConstantParams(highlightColor);
		h.ConstantSwitch();
	}
}