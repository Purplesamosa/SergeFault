﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class GameManager : MonoBehaviour {
	
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

	public Text m_MoneyText;
	public bool m_ItsFirstMadaFaka = true;
	public bool m_MovingFirstMadaFaka=false;

	//Meat variables
	public float m_MoveSpeed = 100;

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
			m_MoneyText.text=m_Money.ToString();
		}
	}

	//test to see if game has ended
	private bool m_IsGameOver = false;



	//reference to skill bars
	//0 : Time   |   1 : Money  |  2 : Faith
	[SerializeField]
	public SkillBar[] m_SkillBars;

	public bool IsGameOver()
	{
		return m_IsGameOver;
	}

	public bool SpendMoney(float _amount)
	{
		if(Money >= _amount) //test to see if money is sufficient
		{
			Money -= _amount;
			return true;  //take money away and return true
		}
		else
		{
			return false; //not enough money
		}
	}


	//reference to skill bars
	//0 : Time   |   1 : Money  |  2 : Faith
	public float GetBarValue(int _index)
	{
		float temp=m_SkillBars[_index].GetValue();
		return	temp;
	}

	public void AddMoney(float _amount)
	{
		Money += _amount; //add new funds to total
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
}
