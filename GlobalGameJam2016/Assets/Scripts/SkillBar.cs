using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SkillBar : MonoBehaviour {

	[SerializeField]
	private Image[] m_Balls;

	//ID used to call the correct music
	[SerializeField]
	private AudioManager.SfxNoLoop m_SliderEnum;

	#region PARTICLE_CONTROL
	public ParticleSystem m_LvlUpParticles;
	public ParticleSystem m_LootFx;
	#endregion
	private int m_Level = 0;

	//the price to use the button to increase the value each level (3 possibles)
	public float[] m_Costs;

	public float[] m_Bonus;


	public bool CanAffordIt(float _money)
	{
		if (m_Level < m_Costs.Length) 
		{
			if(m_Level==(m_Costs.Length-1))
				return false;
			return _money>=m_Costs[m_Level];
		}
		return false;
	}
	
	//gets called when the button is Down
	public void OnBtnDown()
	{
		if(m_Level < 3)
		{
			TryToLevelUp();
		}
	}

	private void TryToLevelUp()
	{
		if(GameManager.Instance.SpendMoney(m_Costs[m_Level]))
		{
			m_Balls[m_Level++].enabled = true;
			m_LvlUpParticles.Play(true);

			AudioManager.Instance.PlaySfxNoLoop(m_SliderEnum);
		}
	}

	public int GetLevel()
	{
		if(m_Level<3)
			return m_Level;

		return 2;
	}

	public float GetBonus()
	{
		return m_Bonus[GetLevel()];
	}

	public bool DropLevel()
	{
		if(m_Level > 0)
		{
			m_Balls[--m_Level].enabled = false;
			m_LootFx.Play (true);
			GameManager.Instance.CheckForStatuePulse();
			return true;
		}
		return false;
	}
}
