using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class GameManager : GGJBase {
	
	//make this son of a bitch a singleton
	#region Singleton
	private static GameManager instance;

	public static GameManager Instance
	{
		get
		{
			return instance;
		}
	}
	#endregion

	#region PARTICLES_CONTROL
	public ParticleSystem m_LoseInvestmentFX;
	#endregion

	private bool m_GameStarted = false;

	public StatuePulse[] m_StatuePulses;

	public ScoreDisplayer m_ScoreDisplayer;

	public bool m_ItsFirstMadaFaka = true;
	public bool m_MovingFirstMadaFaka=false;

	//Meat variables
	public float m_MoveSpeed = 10;

	//record game time
	private float m_InitTime;
	private float m_EndTime;
	private float m_SessionLength;

	//Used to fight the boss
	public float m_BonusPoints;
	public float m_RitualPoints;

	//money the player currently has
	[SerializeField]
	private float m_Money;
	private float Money
	{
		get{
			return m_Money;
		}

		set{
			m_Money=value;
			m_ScoreDisplayer.SetScore(m_Money);
		}
	}

	//test to see if game has ended
	private bool m_IsGameOver = false;

	public Sprite[] m_PenaltyIcons;

	//reference to skill bars
	//0 : Time   |   1 : Money  |  2 : Faith
	[SerializeField]
	public SkillBar[] m_SkillBars;

	public bool IsGameOver()
	{
		return m_IsGameOver;
	}

	public Sprite GetPenaltyICon(Penalty _penalty)
	{
		return m_PenaltyIcons [(int)_penalty];
	}

	public bool SpendMoney(float _amount,bool _loseInvest=false)
	{
		if(Money >= _amount) //test to see if money is sufficient
		{
			Money -= _amount;
			if(_loseInvest)
				m_LoseInvestmentFX.Play(true);

			CheckForStatuePulse ();

			return true;  //take money away and return true
		}
		else
		{
			if(_loseInvest)
			{
				Money=0;
				m_LoseInvestmentFX.Play(true);

				CheckForStatuePulse ();

				return true;
			}
			return false; //not enough money
		}
	}

	public bool GetGameStart()
	{
		return m_GameStarted;
	}

	public void GameStarted()
	{
		m_GameStarted = true;
		m_ScoreDisplayer.SetScore (m_Money);
	}


	//reference to skill bars
	//0 : Time   |   1 : Money  |  2 : Faith
	public int GetBarLevel(int _index)
	{
		return	m_SkillBars[_index].GetLevel();
	}

	public void AddMoney(float _amount)
	{
		Money += _amount; //add new funds to total
		CheckForStatuePulse ();
	}

	public void CheckForStatuePulse()
	{
		for (int i=0; i<m_StatuePulses.Length; i++) 
		{
			if(m_SkillBars[i].CanAffordIt(Money))
			{
				m_StatuePulses[i].StartPulse();
			}
			else
				m_StatuePulses[i].StopPulse();
		}
	}

	public void StartGame()
	{
		m_InitTime = Time.time; //get the time that session starts
	}

	public void LoseGame()
	{
		//set time
		m_EndTime = Time.time;
		m_SessionLength = m_EndTime - m_InitTime; //get length of session
		m_IsGameOver = true; 
		//AudioManager.Instance.EndGame();
	}

	public float GetGameLength()
	{
		return m_SessionLength;
	}

	// Use this for initialization
	void Awake () 
	{
		if(instance == null)
		{
			instance = this;
		}
		else
		{
			DestroyImmediate(this);
		}
	}
	
	public void IncrementBonusPoints(float _increment)
	{
		m_BonusPoints += _increment;
	}

	public void DecrementBonusPoints(float _increment)
	{
		m_BonusPoints -= _increment;
	}

	public float GetMoneyBonus(MinionType _type)
	{
		if (_type == MinionType.SonOfARitch||_type==MinionType.RARE)
			return m_SkillBars [1].GetBonus ();
		return 0;
	}

	public float GetFaithBonus(MinionType _type)
	{
		if (_type == MinionType.Martir||_type==MinionType.RARE)
			return m_SkillBars [2].GetBonus ();
		return 0;
	}

	public float GetWillBonus(MinionType _type)
	{
		if (_type == MinionType.Believer||_type==MinionType.RARE)
			return m_SkillBars [0].GetBonus ();
		return 0;
	}
}
