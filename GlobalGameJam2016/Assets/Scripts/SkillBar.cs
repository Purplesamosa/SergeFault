using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SkillBar : MonoBehaviour {

	[SerializeField]
	private Image m_Slider;

	//ID used to call the correct music
	[SerializeField]
	private AudioManager.SfxLoop m_SliderEnum;

	private int m_Level = 0;

	//the price to use the button to increase the value each level (3 possibles)
	public float[] m_Costs;


	//gets called when the button is Down
	public void OnBtnDown()
	{
		TryToLevelUp();
	}

	private void TryToLevelUp()
	{
		if(GameManager.Instance.SpendMoney(m_Costs[m_Level]))
		{
			++m_Level;
		}
	}

	public int GetLevel()
	{
		return m_Level;
	}

	public void DropLevel()
	{
		--m_Level;
	}
}
