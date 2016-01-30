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

	public MeatT[] m_Types;

	[SerializeField]
	private List<FreshMeat> m_FreshMeats=new List<FreshMeat>();
	[SerializeField]
	private List<FreshMeat> m_UsedMeats=new List<FreshMeat>();
	[SerializeField]
	private List<RectTransform> m_Locations=new List<RectTransform>();

	[SerializeField]
	private Vector2 m_ExitPosition;
	[SerializeField]
	private float m_InitialYPos=0;

	private bool m_RitualInProgress=false;

	//TUNING: 
	private float m_MinSpawnRate=4;
	private float m_MaxSpawnRate=6;
	private float m_TickRate=0.1f;
	private float m_MaxMoneyDenominator=1.25f;
	private float m_MaxFaithDenominator=1.25f;

	private float m_SpawnRate;
	private float m_CollectRate=1;
	private float m_Donations = 0;

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
			}
		}
	}

	IEnumerator SpawnTick()
	{
		while (!GameManager.Instance.IsGameOver()) 
		{
			m_SpawnRate=Random.Range(m_MinSpawnRate,m_MaxSpawnRate)*((100+GameManager.Instance.GetBarValue(0))/100);
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

	public void GetMoneyLimits(MeatType _type,out float _minVal,out float _maxVal)
	{
		for (int i=0; i<m_Types.Length; i++) {
			if(m_Types[i].m_Type==_type)
			{
				_minVal=Mathf.Floor(m_Types[i].m_MinMoney*GameManager.Instance.GetBarValue(1));
				_maxVal=Mathf.Ceil(m_Types[i].m_MaxMoney*GameManager.Instance.GetBarValue(1)/m_MaxMoneyDenominator);
				return;
			}
		}

		_minVal = _maxVal = 0;
	}

	public void GetFaithLimits(MeatType _type,out float _minVal,out float _maxVal)
	{
		for (int i=0; i<m_Types.Length; i++) {
			if(m_Types[i].m_Type==_type)
			{
				_minVal=Mathf.Floor(m_Types[i].m_MinFaith*GameManager.Instance.GetBarValue(2));
				_maxVal=Mathf.Ceil(m_Types[i].m_MaxFaith*GameManager.Instance.GetBarValue(2)/m_MaxFaithDenominator);
				return;
			}
		}
		_minVal = _maxVal = 0;
	}

	public void TryToRitual(int _idx)
	{
		for (int i=0; i<m_UsedMeats.Count; i++) 
		{
			if(m_UsedMeats[i].m_Id==_idx)
			{
				BossManager.Instance.DecreaseAnger(m_UsedMeats[i].ApplyRitual());
			}
			else
				m_UsedMeats[i].RitualAssistance();
		}
	}

	public float GetInitialYPos()
	{
		return m_InitialYPos;
	}

	public Vector2 GetExitPosition()
	{
		return m_ExitPosition;
	}


	[System.Serializable]
	public class MeatT
	{
		public string m_Name;
		public MeatType m_Type;
		public float m_MinMoney;
		public float m_MaxMoney;
		public float m_MinFaith;
		public float m_MaxFaith;
	}
}
