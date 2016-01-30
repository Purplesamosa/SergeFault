using UnityEngine;
using System.Collections;

public class FreshMeat : GGJBase {

	public int m_Id;
	private bool m_LocationReached=false;
	public RectTransform m_LocationRef;
	public Vector2 m_LocationVector;
	public MeatType m_Type=MeatType.MAX;

	public RectTransform m_RectT;
	private Vector2 m_MoveDir;
	private float m_MoveSpeed=10;
	private float m_MinDistanceToReach=1;

	private float m_Money;
	private float m_Faith;

	private float m_MinFaith;
	private float m_MaxFaith;
	private float m_MinMoney;
	private float m_MaxMoney;

	private float m_FaithDecrement=0.1f;

	public void Spawn(MeatType _type,RectTransform _location)
	{
		m_LocationReached = false;
		AudienceManager.Instance.GetMoneyLimits (_type, out m_MinMoney, out m_MaxMoney);
		AudienceManager.Instance.GetFaithLimits (_type, out m_MinFaith, out m_MaxFaith);
		m_Money = Random.Range (m_MinMoney, m_MaxMoney);
		m_Faith = Random.Range (m_MinFaith, m_MaxFaith);
		m_LocationRef = _location;
		m_LocationVector = m_LocationRef.anchoredPosition;
		MoveToSeat ();
	}

	public void MoveToSeat()
	{
		m_RectT.anchoredPosition = new Vector2 (m_LocationRef.anchoredPosition.x, AudienceManager.Instance.GetInitialYPos ());
		StartCoroutine (MoveToLocation ());
	}

	IEnumerator MoveToLocation()
	{
		while(!m_LocationReached)
		{
			yield return 0;
			if(Vector2.Distance(m_RectT.anchoredPosition,m_LocationVector)<m_MinDistanceToReach)
			{
				m_LocationReached=true;
				m_RectT.anchoredPosition=m_LocationVector;
			}else
			{
				m_MoveDir=m_LocationVector-m_RectT.anchoredPosition;
				m_MoveDir.Normalize();
				m_RectT.anchoredPosition+=m_MoveDir*m_MoveSpeed*Time.deltaTime;
			}
		}
	}

	private void DecreaseFaith()
	{
		m_Faith = Mathf.Max (m_Faith-m_FaithDecrement,0);
		if (m_Faith == 0) 
		{
			Leave();
		}
	}

	private void Leave()
	{
		m_LocationReached=false;
		m_LocationVector=AudienceManager.Instance.GetExitPosition();
		StartCoroutine (MoveToLocation ());
	}
}
