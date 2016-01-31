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
	private float m_InvestmentToLoseSmall=30;
	private float m_InvestmentToLoseBig=60;

	private float m_BlasfemyRageSmall=5;
	private float m_BlasfemyRageBig=10;


	private int[] m_SmallPenaltyChances=new int[6]{95,80,60,40,20,0};
	
	private int m_SmallPenaltyChancesIDx=0;
	#endregion

	#region ANIM_CONTROL
	public Animator m_GillotinaAnim;
	public ParticleSystem m_BloodFX;
	#endregion

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
		_minVal=m_MinionsData[(int)_type].m_MinMoney;
		_maxVal=m_MinionsData[(int)_type].m_MinMoney;
	}

	//get faith limits
	public void GetFaithLimits(MinionType _type, out float _minVal,out float _maxVal)
	{
		//TODO: agregar bonuse de las estatuas
		_minVal=m_MinionsData[(int)_type].m_MinFaith;
		_maxVal=m_MinionsData[(int)_type].m_MaxFaith;
	}

	//get will limits
	public void GetWillLimits(MinionType _type, out float _minVal,out float _maxVal)
	{
		//TODO: agregar bonuse de las estatuas
		_minVal=m_MinionsData[(int)_type].m_MinWill;
		_maxVal=m_MinionsData[(int)_type].m_MaxWill;
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

	public Penalty GetRandomPenalty()
	{
		int randVal=Random.Range(0,101);
		
		if(randVal<=m_SmallPenaltyChances[m_SmallPenaltyChancesIDx])
		{
			m_SmallPenaltyChancesIDx++;
			if(m_SmallPenaltyChancesIDx==m_SmallPenaltyChances.Length)
				m_SmallPenaltyChancesIDx=1;  
			return (Penalty)Random.Range(0,4);
		}
		else
		{
			m_SmallPenaltyChancesIDx=1;
			return (Penalty)Random.Range(4,8);
		}
	}

	public void ResetMinion(int _idx)
	{
		m_Minions [_idx].m_Alive=false;
		m_Minions [_idx].Disappear ();
	}
	#endregion


	#region INITIALIZE
	//TODO: call this when the initialization is needed

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

		StartCoroutine (RitualSteps (_idx));
	}

	IEnumerator RitualSteps(int _idx)
	{
		m_GillotinaAnim.SetBool ("StartAnim", true);
		AudioManager.Instance.PlaySfxNoLoop (AudioManager.SfxNoLoop.MACHINE_STARTS);

		//if the minion is alive then apply a ritual to it
		if (m_Minions [_idx].m_Alive) {
			BossManager.Instance.DecreaseAnger (m_Minions [_idx].GetFaith ());
			ResetMinion (_idx);
		}

		yield return 0;
		DecrementWill ();
		yield return new WaitForSeconds (1);
		while(ResolvePenalties ())
			yield return new WaitForSeconds(0.5f);
		CollectProfits ();
		yield return new WaitForSeconds(4);
		m_GillotinaAnim.SetBool ("ContinueAnim", true);
		AudioManager.Instance.PlaySfxNoLoop (AudioManager.SfxNoLoop.MACHINE_REWIND);
		yield return new WaitForSeconds(1);
		ReturnToFront ();


		m_RitualInProgress = false;
	}

	private void CollectProfits()
	{
		m_Donations = 0;
		for (int i=0; i<m_Minions.Length; i++) 
		{
			if(m_Minions[i].m_Alive)
			{
				m_Donations+=m_Minions[i].ChargeDonation();
			}
		}
		GameManager.Instance.AddMoney (m_Donations);
	}

	private void DecrementWill(int _idx=-1)
	{
		//decrement the will to all living minions on the front at the moment of the ritual
		for (int i=0; i<m_Minions.Length; i++) 
		{
			if(!m_Minions[i].m_Alive)
				continue;
			m_Minions[i].DecreaseWill(m_WillDecrement,_idx==i);
		}
	}

	private void DecreaseWillToARandomMinion(int _idx=-1)
	{

		//decrement the will to all living minions on the front at the moment of the ritual
		for (int i=0; i<m_Minions.Length; i++) 
		{
			if(!m_Minions[i].m_Alive||m_Minions[i].GetWill()==0)
				continue;
			m_Minions[i].DecreaseWill(m_WillDecrement,_idx==i);
			break;
		}
	}

	private bool ResolvePenalties()
	{
		for (int i=0; i<m_Minions.Length; i++) 
		{
			if(!m_Minions[i].m_Alive||m_Minions[i].GetWill()>0)
				continue;
			ApplyPenalty(m_Minions[i].GetPenalty(),i);
			RetreatMinion(i);
			return true;
		}
		return false;
	}

	private void RetreatMinion(int _idx)
	{
		if (m_Minions [_idx].m_Alive && m_Minions [_idx].GetWill () == 0) 
		{
			m_Minions[_idx].m_Alive=false;
			m_Minions[_idx].LeaveTheFront();
		}
	}

	private void ApplyPenalty(Penalty _penalty,int _idx)
	{
		switch(_penalty)
		{
			case Penalty.LoseInvestment_Small: //retrieve a small amount of money
				GameManager.Instance.SpendMoney(m_InvestmentToLoseSmall,true);
				//TODO: penalty FX
			break;
			case Penalty.LoseInvestment_Big: //retrieve big amount of money
				GameManager.Instance.SpendMoney(m_InvestmentToLoseBig,true);
				//TODO: penalty FX
			break;

			case Penalty.Difamation_Small: //remove -1 will to 1 random minions
				m_Minions[_idx].m_DifamationFx.Play(true);
				DecreaseWillToARandomMinion(_idx);
			break;
			case Penalty.Difamation_Big: //remove -1 will to all minions
				m_Minions[_idx].m_DifamationFx.Play(true);
				DecrementWill(_idx);
			break;

			case Penalty.Blasfemy_Small: //add a small amount of rage to the GOD
				BossManager.Instance.penaltyGodStep(m_BlasfemyRageSmall);
			break;
			case Penalty.Blasfemy_Big: //add a big amount of rage to the GOD
				BossManager.Instance.penaltyGodStep(m_BlasfemyRageBig);
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
				GameManager.Instance.m_SkillBars[i].DropLevel();
			}
		} else {
			for(int i=0;i<GameManager.Instance.m_SkillBars.Length;i++)
			{
				if(GameManager.Instance.m_SkillBars[i].DropLevel())
					return;
			}
		}
	}

	public void ReturnToFront()
	{
		for(int i=0;i<m_Minions.Length;i++)
		{
			if(m_Minions[i].m_Alive||m_Minions[i].m_Moving)
				continue;
			m_Minions[i].Spawn(GetRandomType());
		}
	}
	#endregion


	#region MINIONDATA_CLASS
	[System.Serializable]
	public class MinionData
	{
		public float m_MinMoney;
		public float m_MaxMoney;
		public float m_MinFaith;
		public float m_MaxFaith;
		public float m_MinWill;
		public float m_MaxWill;
	}
	#endregion
}
