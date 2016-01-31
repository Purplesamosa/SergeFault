using UnityEngine;
using System.Collections;

public class ShakeTheObject : MonoBehaviour {


	public FadeIn m_FadeIn;

	private bool m_Finished = false;
	[SerializeField]
	private float m_XShake;
	[SerializeField]
	private float m_FallSpeed;
	[SerializeField]
	private float m_TimeUntilBegin;


	[SerializeField]
	private float m_XStart;
	[SerializeField]
	private float m_Threshold;

	[SerializeField]
	private float m_TimeUntilFallDown;

	private float m_Timer;

	private bool m_Right = true;
	private bool m_Falling = false;

	private Vector3 m_Return;

	void Start()
	{
		m_Return = transform.position;
		m_XStart = transform.position.x;
	}

	public void StartDestruction()
	{
		StartCoroutine(BreakObject());
	}

	private IEnumerator BreakObject()
	{
		yield return new WaitForSeconds(m_TimeUntilBegin);
		StartCoroutine(BeginToFall());
		while(!m_Finished)
		{
			if(!m_Falling)
			{
				if(m_Right)
				{
					Debug.Log("HERE 1");
					transform.Translate(m_XShake*Time.deltaTime,0,0);
					if(transform.position.x > m_XStart + m_Threshold)
					{
						m_Right = false;
					}
				}
				else if(!m_Right)
				{
					Debug.Log("HERE 1");
					transform.Translate(-m_XShake*Time.deltaTime,0,0);
					if(transform.position.x < m_XStart - m_Threshold)
					{
						m_Right = true;
					}
				}
				yield return 0;
			}
			else
			{
				if(m_Right)
				{
					Debug.Log("HERE 1");
					transform.Translate(m_XShake*Time.deltaTime,-m_FallSpeed*Time.deltaTime,0);
					if(transform.position.x > m_XStart + m_Threshold)
					{
						m_Right = false;
					}
				}
				else if(!m_Right)
				{
					Debug.Log("HERE 1");
					transform.Translate(-m_XShake*Time.deltaTime,-m_FallSpeed*Time.deltaTime,0);
					if(transform.position.x < m_XStart - m_Threshold)
					{
						m_Right = true;
					}
				}
				yield return 0;
			}
			if(m_FadeIn)
			{
				if(transform.position.y < -215)
				{
					m_FadeIn.StartFadeIn();
				}
			}
		}
	}

	private IEnumerator BeginToFall()
	{
		yield return new WaitForSeconds(m_TimeUntilFallDown);
		m_Falling = true;
	}

}
