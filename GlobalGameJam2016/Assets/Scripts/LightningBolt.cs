using UnityEngine;
using System.Collections;

public class LightningBolt : MonoBehaviour
{

	public LineRenderer m_Bolt;
	private float m_Alpha;
	private Color m_BoltColor = Color.white;
	private Vector3 m_StartPos;
	private Vector3 m_EndPos;
	private Vector3 m_CurPos;

	private bool m_BoltActive;
	private float m_BoltETA;
	private float m_BoltSpeed = 2.5f;
	// Use this for initialization
	void Start ()
	{
		m_StartPos = new Vector3 ();
		m_EndPos = new Vector3 ();
		m_CurPos = new Vector3 ();
		m_BoltActive = false;

		m_StartPos.z = 0;
		m_StartPos.y = 350;

		m_EndPos.z = 0;
		m_EndPos.y = 0;

		m_CurPos.z = 0;
	}

	void Update()
	{
		if(m_BoltActive) UpdateBolt();
	}

	public void GenerateBolt()
	{
		if(m_BoltActive) return;

		m_StartPos.x = Random.Range (-300, 300);
		m_EndPos.x = Random.Range (50, 250) * (m_StartPos.x < 0 ? 1 : -1);

		for(int i = 0; i < 10; ++i)
		{
			if(i == 0) m_Bolt.SetPosition(i, m_StartPos);
			else if(i == 9) m_Bolt.SetPosition(i, m_EndPos);
			else
			{
				m_CurPos.x = m_StartPos.x + ((m_EndPos.x - m_StartPos.x) * 0.1f * i) + Random.Range(-35, 35);
				m_CurPos.y = m_StartPos.y + ((m_EndPos.y - m_StartPos.y) * 0.1f * i) + Random.Range(-35, 35);
				m_Bolt.SetPosition(i, m_CurPos);
			}
		}

		m_BoltActive = true;
		m_BoltETA = 0f;
		m_BoltColor = Color.white;
		m_Bolt.SetColors (m_BoltColor, m_BoltColor);
	}

	void UpdateBolt()
	{
		m_BoltETA += Time.deltaTime * m_BoltSpeed;
		m_BoltColor.a = Mathf.Lerp (0f, 1f, Mathf.Min (1f, Mathf.Max(2f - (m_BoltETA * 2f), 0f)));
		m_Bolt.SetColors (m_BoltColor, m_BoltColor);
		if(2f - (m_BoltETA * 2f) <= 0f) m_BoltActive = false;
	}
}
