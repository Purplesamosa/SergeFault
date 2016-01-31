using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FreshMeat : GGJBase {

	private delegate void MoveCallback();
	private MoveCallback m_MoveCallback;

	public int m_Id;
	private bool m_LocationReached=false;
	public RectTransform m_LocationRef;
	public Vector2 m_LocationVector;
	public MeatType m_Type=MeatType.MAX;

	public Text m_MoneyText;
	public Text m_FaithText;

	public RectTransform m_RectT;
	private Vector2 m_MoveDir;

	private float m_MinDistanceToReach=4;

	private float m_Money;
	private float m_Faith;

	private float m_MinFaith;
	private float m_MaxFaith;
	private float m_MinMoney;
	private float m_MaxMoney;



	public void RitualClick()
	{
		if (GameManager.Instance.IsGameOver ()||!m_LocationReached)
			return;
		AudienceManager.Instance.TryToRitual (m_Id);
	}

	public void Spawn(MeatType _type,RectTransform _location)
	{
		m_Type = _type;
		m_LocationReached = false;
//		AudienceManager.Instance.GetMoneyLimits (out m_MinMoney, out m_MaxMoney);
//		AudienceManager.Instance.GetFaithLimits (out m_MinFaith, out m_MaxFaith);
		m_Money = Mathf.Ceil (Random.Range (m_MinMoney, m_MaxMoney));
		m_Faith = Mathf.Floor(Random.Range (m_MinFaith, m_MaxFaith));

		m_MoneyText.text = m_Money.ToString();
		m_FaithText.text = m_Faith.ToString();

		m_LocationRef = _location;
		m_LocationVector = new Vector2(m_LocationRef.anchoredPosition.x,AudienceManager.Instance.GetRandomHeight());
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
				m_MoveCallback();
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
		if (!m_LocationReached)
			return;
		m_Faith = Mathf.Max (m_Faith-AudienceManager.Instance.m_FaithDecrement,0);
		m_FaithText.text = m_Faith.ToString();
		if (m_Faith == 0) 
		{
			Leave();
		}
	}

	private void Leave()
	{
		m_LocationReached=false;
		m_LocationVector = new Vector2 (m_LocationVector.x, AudienceManager.Instance.GetInitialYPos ());
		m_MoveCallback += SceneLeft;
		StartCoroutine (MoveToLocation ());
	}

	private void SeatReached()
	{
		if (GameManager.Instance.m_MovingFirstMadaFaka) {
			GameManager.Instance.m_MovingFirstMadaFaka=false;
			BossManager.Instance.deactivateSafeZone ();
		}
		m_MoveCallback -= SeatReached;
		m_LocationReached=true;
	}

	private void SceneLeft()
	{
		m_MoveCallback -= SceneLeft;
		m_LocationReached=true;
		Disappear ();
	}

	private void Disappear()
	{
		m_RectT.anchoredPosition =  new Vector2 (m_LocationVector.x, AudienceManager.Instance.GetInitialYPos ());
		AudienceManager.Instance.RetrieveMeat (m_Id);
		AudioManager.Instance.PlaySfxNoLoop(AudioManager.SfxNoLoop.KillVillagerSound);
	}
}
