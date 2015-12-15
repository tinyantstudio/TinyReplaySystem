using UnityEngine;
using System;

[RequireComponent(typeof(UILabel))]
public class FPSCounter : MonoBehaviour {
	
	const float fpsMeasurePeriod = 0.5f;
	private int m_FpsAccumulator = 0;
	private float m_FpsNextPeriod = 0;
	private int m_CurrentFps;
	const string display = "{0} FPS";
	private UILabel m_GuiText;
	
	private void Start()
	{
		m_FpsNextPeriod = Time.realtimeSinceStartup + fpsMeasurePeriod;
		this.m_GuiText = this.GetComponent<UILabel>();
	}
	
	private void Update()
	{
		// measure average per second.
		m_FpsAccumulator ++;
		if(Time.realtimeSinceStartup > m_FpsNextPeriod)
		{
			m_CurrentFps = (int)(m_FpsAccumulator / fpsMeasurePeriod);
			m_FpsAccumulator = 0;
			m_FpsNextPeriod += fpsMeasurePeriod;
			m_GuiText.text = string.Format(display, m_CurrentFps);
		}
	}
}
