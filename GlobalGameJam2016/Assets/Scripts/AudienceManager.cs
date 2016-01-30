using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class AudienceManager : GGJBase {

	static private AudienceManager instance;
	static public AudienceManager Instance
	{
		get{
			return instance;
		}
	}

	[SerializeField]
	private List<FreshMeat> m_FreshMeats=new List<FreshMeat>();
	[SerializeField]
	private List<FreshMeat> m_UsedMeats=new List<FreshMeat>();
	[SerializeField]
	private List<RectTransform> m_Locations=new List<RectTransform>();

	public RectTransform m_BottomHelper;
	public RectTransform m_TopHelper;

	[SerializeField]
	private Vector2 m_ExitPosition;
	[SerializeField]
	private float m_InitialYPos=0;

	private bool m_RitualInProgress=false;

	//TUNING: 
	public float m_MinMoney=5;
	public float m_MaxMoney=15;
	public float m_MinFaith=2;
	public float m_MaxFaith=5;
	
	private float m_MinSpawnRate=2;
	private float m_MaxSpawnRate=4;
	private float m_TickRate=0.1f;
	private float m_MaxMoneyDenominator=1.25f;
	private float m_MaxFaithDenominator=1.25f;

	private float m_SpawnRate;
	private float m_CollectRate=1;
	private float m_Donations = 0;

	public float m_FaithDecrement=1f;

	private float m_FullHouseDelay=0.5f;
	
	void Awake () {
		instance = this;
	}

	void Start()
	{
		InitializeGame ();
	}

	public void InitializeGame()
	{
		StartCoroutine (SpawnTick ());
		StartCoroutine (CollectTick ());
	}

	IEnumerator CollectTick()
	{
		while (!GameManager.Instance.IsGameOver()) 
		{

			//Spawn fresh meat delay
			yield return new WaitForSeconds(m_CollectRate);
			if(m_UsedMeats.Count>0)
			{
				m_Donations=0;
				for(int i=0;i<m_UsedMeats.Count;i++)
					m_Donations+=m_UsedMeats[i].ChargeDonation();
				GameManager.Instance.AddMoney(m_Donations);
				AudioManager.Instance.PlaySfxNoLoop(AudioManager.SfxNoLoop.MoneyChargeSound); //play the money sound
			}
		}
	}

	IEnumerator SpawnTick()
	{
		while (!GameManager.Instance.IsGameOver()) 
		{
			m_SpawnRate=Random.Range(m_MinSpawnRate*(1/GameManager.Instance.GetBarValue(0)),m_MaxSpawnRate*(1/GameManager.Instance.GetBarValue(0)));
			Debug.Log("ESPERAR: "+m_SpawnRate+" BAR: "+GameManager.Instance.GetBarValue(0));
			//Spawn fresh meat delay
			yield return new WaitForSeconds(m_SpawnRate);
			if(m_FreshMeats.Count>0)
			{
				SpawnFreshMeat();
			}
			else
			{
				while(m_FreshMeats.Count==0)
				{
					yield return new WaitForSeconds(m_FullHouseDelay);
				}
			}
		}
	}

	//Spawn a new minion
	private void SpawnFreshMeat()
	{
		if (m_FreshMeats.Count == 0)
			return;

		m_UsedMeats.Add (m_FreshMeats [0]);
		m_FreshMeats [0].Spawn (GetRandomType(),m_Locations[0]);
		m_Locations.RemoveAt (0);
		m_FreshMeats.RemoveAt (0);
	}

	//return an unused minion to the pool
	public void RetrieveMeat(int _idx)
	{
		int idToRetrieve = -1;
		for (int i=0; i<m_UsedMeats.Count; i++) 
		{
			if(m_UsedMeats[i].m_Id==_idx)
			{
				idToRetrieve=i;
				break;
			}
		}
		if (idToRetrieve != -1) 
		{
			m_FreshMeats.Add(m_UsedMeats[idToRetrieve]);
			m_Locations.Add(m_UsedMeats[idToRetrieve].m_LocationRef);
			m_UsedMeats.RemoveAt(idToRetrieve);
		}
	}

	private MeatType GetRandomType()
	{
		return (MeatType)Random.Range(0,(int)MeatType.MAX);
	}

	public void GetMoneyLimits(out float _minVal,out float _maxVal)
	{
		_minVal=Mathf.Floor(m_MinMoney*GameManager.Instance.GetBarValue(1));
		_maxVal=Mathf.Ceil(m_MaxMoney*GameManager.Instance.GetBarValue(1)/m_MaxMoneyDenominator);
	}

	public void GetFaithLimits(out float _minVal,out float _maxVal)
	{
		_minVal=Mathf.Floor(m_MinFaith*GameManager.Instance.GetBarValue(2));
		_maxVal=Mathf.Ceil(m_MaxFaith*GameManager.Instance.GetBarValue(2)/m_MaxFaithDenominator);
	}

	public void TryToRitual(int _idx)
	{
		//make ritual to the choosen one
		for (int i=0; i<m_UsedMeats.Count; i++) {
			if (m_UsedMeats [i].m_Id == _idx) {
				BossManager.Instance.DecreaseAnger (m_UsedMeats [i].ApplyRitual ());
				break;
			}
		}
		//Resolve RitualAssistance
		for (int j=0; j<m_UsedMeats.Count; j++) 
		{
			m_UsedMeats[j].RitualAssistance();
		}
	}

	public float GetRandomHeight()
	{
		return Random.Range (m_BottomHelper.anchoredPosition.y, m_TopHelper.anchoredPosition.y);
	}

	public float GetInitialYPos()
	{
		return m_InitialYPos;
	}

	public Vector2 GetExitPosition()
	{
		return m_ExitPosition;
	}
}
