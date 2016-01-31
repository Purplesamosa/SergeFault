using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StatuePulse : MonoBehaviour
{
	public RectTransform m_Rekt;
	public Image m_Image;
	public float m_MaxSize;
	public float m_PulseDuration;
	public float m_PulseInterval;
	private bool m_IsPulsing;
	private bool m_Phase;
	private float m_ETA;

	private Vector3 m_MaxScale;
	private Vector3 m_CurScale;
	private Color m_PulseColor = Color.white;

	// Use this for initialization
	void Start ()
	{
		m_IsPulsing = false;
		m_MaxScale = new Vector3 (m_MaxSize, m_MaxSize, 1);
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (m_IsPulsing) Pulse ();
	}

	void Pulse()
	{
		m_ETA += Time.deltaTime;
		if(m_Phase)
		{
			m_Rekt.localScale = Vector3.Lerp(Vector3.one, m_MaxScale, Mathf.Sqrt(m_ETA/m_PulseDuration));
			m_PulseColor.a = 1 - Mathf.Sqrt(m_ETA/m_PulseDuration);
			if(m_ETA >= m_PulseDuration)
			{
				m_PulseColor.a = 0;
				m_ETA = 0;
				m_Phase = false;
			}
			m_Image.color = m_PulseColor;
		}
		else
		{
			if(m_ETA >= m_PulseInterval)
			{
				m_Phase = true;
				m_ETA = 0f;
				m_PulseColor.a = 1;
			}
			m_Rekt.localScale = Vector3.one;
			m_Image.color = m_PulseColor;
		}
	}

	public void StartPulse()
	{
		if (m_IsPulsing)
			return;
		m_IsPulsing = true;
		m_Phase = true;
		m_ETA = 0f;
		m_PulseColor.a = 1;
		m_Rekt.localScale = Vector3.one;
		m_Image.color = m_PulseColor;
		m_Image.enabled = true;
	}

	public void StopPulse()
	{
		m_IsPulsing = false;
		m_Image.enabled = false;
	}
}
