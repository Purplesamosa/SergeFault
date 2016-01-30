using UnityEngine;
using System.Collections;

public class FreshMeat : GGJBase {

	private delegate void MoveCallback();
	private MoveCallback m_MoveCallback;

	public int m_Id;
	private bool m_LocationReached=false;
	public RectTransform m_LocationRef;
	public Vector2 m_LocationVector;
	public MeatType m_Type=MeatType.MAX;

	public RectTransform m_RectT;
	private Vector2 m_MoveDir;

	private float m_MinDistanceToReach=1;

	private float m_Money;
	private float m_Faith;

	private float m_MinFaith;
	private float m_MaxFaith;
	private float m_MinMoney;
	private float m_MaxMoney;

	private float m_FaithDecrement=0.1f;

	public void RitualClick()
	{
		if (GameManager.Instance.IsGameOver ())
			return;
		AudienceManager.Instance.TryToRitual (m_Id);
	}

	public void Spawn(MeatType _type,RectTransform _location)
	{
		m_Type = _type;
		m_LocationReached = false;
		AudienceManager.Instance.GetMoneyLimits (_type, out m_MinMoney, out m_MaxMoney);
		AudienceManager.Instance.GetFaithLimits (_type, out m_MinFaith, out m_MaxFaith);
		m_Money = Random.Range (m_MinMoney, m_MaxMoney);
		m_Faith = Random.Range (m_MinFaith, m_MaxFaith);
		Debug.Log ("ENTRA: " + m_Id.ToString () + " MONEY: " + m_Money.ToString () + " FAITH: " + m_Faith);
		m_LocationRef = _location;
		m_LocationVector = m_LocationRef.anchoredPosition;
		MoveToSeat ();
	}

	public void MoveToSeat()
	{
		m_RectT.anchoredPosition = new Vector2 (m_LocationRef.anchoredPosition.x, AudienceManager.Instance.GetInitialYPos ());
		m_MoveCallback += SeatReached;
		StartCoroutine (MoveToLocation ());
	}

	IEnumerator MoveToLocation()
	{
		while(!m_LocationReached&&!GameManager.Instance.IsGameOver())
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
				m_RectT.anchoredPosition+=m_MoveDir*GameManager.Instance.m_MoveSpeed*Time.deltaTime;
			}
		}
	}

	public void RitualAssistance()
	{
		DecreaseFaith ();
	}

	public float ApplyRitual()
	{
		Disappear ();
		//TODO: apply the ritual animation
		Debug.Log ("Die bitch die!!");
		return m_Faith;
	}

	public float ChargeDonation()
	{
		if (m_LocationReached) 
		{
			return m_Money;
		}
		return 0;
	}

	private void DecreaseFaith()
	{
		m_Faith = Mathf.Max (m_Faith-m_FaithDecrement,0);
		if (m_Faith == 0) 
		{
			Debug.Log("Leaving");
			Leave();
		}
	}

	private void Leave()
	{
		m_LocationReached=false;
		m_LocationVector=AudienceManager.Instance.GetExitPosition();
		m_MoveCallback += SceneLeft;
		StartCoroutine (MoveToLocation ());
	}

	private void SeatReached()
	{
		m_MoveCallback -= SeatReached;
		Debug.Log ("Seat Reached by Meat: " + m_Id);
	}

	private void SceneLeft()
	{
		m_MoveCallback -= SceneLeft;
		Debug.Log ("Scene Left by Meat: " + m_Id);
		Disappear ();
	}

	private void Disappear()
	{
		m_RectT.anchoredPosition = AudienceManager.Instance.GetExitPosition();
		AudienceManager.Instance.RetrieveMeat (m_Id);
	}
}
