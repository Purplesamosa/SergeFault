using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class AudienceManager : GGJBase {

	#region SINGLETON_SHIT
	//Singleton shit
	static private AudienceManager instance;
	static public AudienceManager Instance
	{
		get{
			return instance;
		}
	}

	void Awake () {
		instance = this;
	}

	#endregion

	#region MINIONDATA_VARIABLES
	[SerializeField]
	private Minion[] m_Minions;
	[SerializeField]
	private MinionData[] m_MinionsData;
	#endregion

	#region LOCATION_VARAIBLES
	//helper that has the initial height for minions
	public RectTransform m_BottomHelper;
	//helper that has the max height for minion position on the front
	public RectTransform m_TopHelper;
	//helper that has the initial position for minion spawn
	public RectTransform m_InitializeHelper;
	#endregion
	
	#region PHASING_CONTROL
	private bool m_RitualInProgress=false;
	//the sum of all donations this after-ritual
	private float m_Donations = 0;
	//the amount of will to decrement on each ritual
	private float m_WillDecrement=1f;
	#endregion
	
	#region PENALTY_TUNNING
	private float m_InvestmentToLoseSmall=100;
	private float m_InvestmentToLoseBig=200;

	private float m_BlasfemyRageSmall=2;
	private float m_BlasfemyRageBig=5;
	#region

	#region SPAWN_CONTROL
	//Spawn all possible minions
	private void SpawnMinions()
	{
		for (int i=0; i<m_Minions.Length; i++)
		{
			//if this minion is not alive then send it to the front
			if(!m_Minions[i].m_Alive)
			{
				m_Minions[i].Spawn(GetRandomType());
			}
		}
	}

	//get money limits
	public void GetMoneyLimits(MinionType _type, out float _minVal,out float _maxVal)
	{
		//TODO: agregar bonuse de las estatuas
		_minVal=m_MinionsData[(int)MinionType].m_MinMoney;
		_maxVal=m_MinionsData[(int)MinionType].m_MinMoney;
	}

	//get faith limits
	public void GetFaithLimits(MinionType _type, out float _minVal,out float _maxVal)
	{
		//TODO: agregar bonuse de las estatuas
		_minVal=m_MinionsData[(int)MinionType].m_MinFaith;
		_maxVal=m_MinionsData[(int)MinionType].m_MaxFaith;
	}

	//get will limits
	public void GetWillLimits(MinionType _type, out float _minVal,out float _maxVal)
	{
		//TODO: agregar bonuse de las estatuas
		_minVal=m_MinionsData[(int)MinionType].m_MinWill;
		_maxVal=m_MinionsData[(int)MinionType].m_MaxWill;
	}

	//return a random height between the helpers
	public float GetRandomHeight()
	{
		return Random.Range (m_BottomHelper.anchoredPosition.y, m_TopHelper.anchoredPosition.y);
	}

	//return the initial height where the minions will be spawned
	public float GetInitialYPos()
	{
		return m_InitializeHelper.anchoredPosition.y;
	}

	//TODO: change this to probabilities
	private MinionType GetRandomType()
	{
		return (MinionType)Random.Range(0,(int)MinionType.MAX);
	}


	#endregion


	#region INITIALIZE
	//TODO: call this when the initialization is needed
	void Start()
	{
		InitializeGame ();
	}

	public void InitializeGame()
	{
		SpawnMinions ();
	}
	#endregion

	#region RITUAL_CONTROL
	public void TryToRitual(int _idx)
	{
		if (m_RitualInProgress)
			return;
		m_RitualInProgress = true;
		//if the minion is alive then apply a ritual to it
		if (m_Minions [_idx].m_Alive) 
		{
			BossManager.Instance.DecreaseAnger (m_Minions [_idx].GetFaith ());
		}

		DecrementWill ();
		ResolvePenalties();
	}

	private void DecrementWill()
	{
		//decrement the will to all living minions on the front at the moment of the ritual
		for (int i=0; i<m_Minions.Length; i++) 
		{
			if(!m_Minions[i].m_Alive)
				continue;
			m_Minions[i].DecreaseWill(m_WillDecrement);
		}
	}

	private void DecreaseWillToARandomMinion()
	{
		//decrement the will to all living minions on the front at the moment of the ritual
		for (int i=0; i<m_Minions.Length; i++) 
		{
			if(!m_Minions[i].m_Alive)
				continue;
			m_Minions[i].DecreaseWill(m_WillDecrement);
		}
	}

	private void ResolvePenalties()
	{
		for (int i=0; i<m_Minions.Length; i++) 
		{
			if(!m_Minions[i].m_Alive||m_Minions[i].GetWill()>0)
				continue;
			ApplyPenalty(m_Minions[i].GetPenalty());
			RetreatMinion(i);
			i=-1; //force 
		}
	}

	private void RetreatMinion(int _idx)
	{
		if (m_Minions [_idx].m_Alive && m_Minions [_idx].GetWill () == 0) 
		{
			m_Minions[_idx].m_Alive=false;
			m_Minions[_idx].LeaveTheFront();
		}
	}

	private void ApplyPenalty(Penalty _penalty)
	{
		switch(_penalty)
		{
			case Penalty.LoseInvestment_Small: //retrieve a small amount of money
				GameManager.Instance.SpendMoney(m_InvestmentToLoseSmall);
				//TODO: penalty FX
			break;
			case Penalty.LoseInvestment_Big: //retrieve big amount of money
				GameManager.Instance.SpendMoney(m_InvestmentToLoseBig);
				//TODO: penalty FX
			break;

			case Penalty.Difamation_Small: //remove -1 will to 1 random minions
				DecreaseWillToARandomMinion();
			break;
			case Penalty.Difamation_Big: //remove -1 will to all minions
				DecrementWill();
			break;

			case Penalty.Blasfemy_Small: //add a small amount of rage to the GOD
				BossManager.Instance.PenaltyGotStep(m_BlasfemyRageSmall);
			break;
			case Penalty.Blasfemy_Big: //add a big amount of rage to the GOD
				BossManager.Instance.PenaltyGotStep(m_BlasfemyRageBig);
			break;

			case Penalty.Loot_Small: //remove 1 upgrade to a random statue if possible
				RemoveRandomUpgrades();
			break;
			case Penalty.Loot_Big: //remove 1 upgrade to all statues if possible
				RemoveRandomUpgrades(true);
			break;
		}
	}

	private void RemoveRandomUpgrades(bool _all=false)
	{
		if (_all) {
			for(int i=0;i<GameManager.Instance.m_SkillBars.Length;i++)
			{
<<<<<<< HEAD
				GameManager.Instance.m_SkillBars[i].Downgrade();
=======
				m_Donations=0;
				for(int i=0;i<m_UsedMeats.Count;i++)
					m_Donations+=m_UsedMeats[i].ChargeDonation();
				GameManager.Instance.AddMoney(m_Donations);
				AudioManager.Instance.PlaySfxNoLoop(AudioManager.SfxNoLoop.MoneyChargeSound); //play the money sound
>>>>>>> Development
			}
		} else {
			GameManager.Instance.m_SkillBars[Random.Range(0,GameManager.Instance.m_SkillBars.Length)].Downgrade();
		}
	}

	//collect profits if possible
	public void CollectDonations()
	{
		m_Donations=0;
		for (int i=0; i<m_Minions.Length; i++) 
		{
			m_Donations += m_Minions [i].ChargeDonation ();
		}
		GameManager.Instance.AddMoney(m_Donations);
	}
	#endregion



	IEnumerator CollectTick()
	{
		while (!GameManager.Instance.IsGameOver()) 
		{

<<<<<<< HEAD
=======
//			m_SpawnRate=Random.Range(m_MinSpawnRate-GameManager.Instance.GetBarValue(0),m_MaxSpawnRate-GameManager.Instance.GetBarValue(0));
			m_SpawnRate=Mathf.Max(m_SpawnRate,1);
>>>>>>> Development
			//Spawn fresh meat delay
			yield return new WaitForSeconds(m_CollectRate);
			if(m_UsedMeats.Count>0)
			{

			}
		}
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

<<<<<<< HEAD
=======
	private MeatType GetRandomType()
	{
		return (MeatType)Random.Range(0,(int)MeatType.MAX);
	}

/*	public void GetMoneyLimits(out float _minVal,out float _maxVal)
	{
		_minVal=Mathf.Floor(m_MinMoney*GameManager.Instance.GetBarValue(1));
		_maxVal=Mathf.Ceil(m_MaxMoney*GameManager.Instance.GetBarValue(1)/m_MaxMoneyDenominator);
	}

	public void GetFaithLimits(out float _minVal,out float _maxVal)
	{
		_minVal=Mathf.Floor(m_MinFaith*GameManager.Instance.GetBarValue(2));
		//_maxVal=Mathf.Ceil(m_MaxFaith*GameManager.Instance.GetBarValue(2));
		_maxVal=Mathf.Ceil(m_MaxFaith*GameManager.Instance.GetBarValue(2)/m_MaxFaithDenominator);
	}
*/
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
>>>>>>> Development


	#region MINIONDATA_CLASS
	[System.Serializable]
	public class MinionData
	{
		public float m_MinMoney;
		public float m_MaxMonet;
		public float m_MinFaith;
		public float m_MaxFaith;
		public float m_MinWill;
		public float m_MaxWill;
	}
	#endregion
}
