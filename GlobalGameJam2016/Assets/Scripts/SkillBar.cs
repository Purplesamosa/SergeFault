using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SkillBar : MonoBehaviour {

	[SerializeField]
	private Image[] m_Balls;

	//ID used to call the correct music
	[SerializeField]
	private AudioManager.SfxLoop m_SliderEnum;

	private int m_Level = 0;

	//the price to use the button to increase the value each level (3 possibles)
	public float[] m_Costs;


	void Update()
	{
		if(Input.GetKeyDown(KeyCode.A))
		{
			DropLevel();
		}
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
			m_Balls[m_Level++].color = Color.red;
		}
	}

	public int GetLevel()
	{
		return m_Level;
	}

	public void DropLevel()
	{
		if(m_Level > 0)
		{
			m_Balls[--m_Level].color = Color.grey;
		}
	}
}
