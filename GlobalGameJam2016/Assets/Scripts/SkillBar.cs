using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SkillBar : MonoBehaviour {

	[SerializeField]
	private Slider m_Slider;

	//max value of bar
	[SerializeField]
	private float m_MaxValue;

	//max value of bar
	[SerializeField]
	private float m_MinValue;

	//value of the skill bar
	[SerializeField]
	private float m_Value;

	//the price to use the button to increase the value
	public float m_Cost;

	#region Increase Value

	//key to play with
	private byte m_CoroutineKey;



	//value to increment with
	[SerializeField]
	private float m_Increment;
	//how often to check over the increment
	[SerializeField]
	private float m_IncrementCheckRate;


	#endregion

	//value for slider to have a smoothing effect
	private float m_CurVisualValue;
	//how fast the smoothing should function
	[SerializeField]
	private float m_VisualSpeed;

	//value to decrement with
	[SerializeField]
	private float m_Decrement;
	//how often to check over the Decrement
	[SerializeField]
	private float m_DecrementCheckRate;


	void Start()
	{
		StartCoroutine(CheckDecrease());
	}

	void Update()
	{
		RunSmoothing();
	}

	private void RunSmoothing()
	{
		m_CurVisualValue += (m_Value - m_CurVisualValue) * m_VisualSpeed * Time.deltaTime;
		m_Slider.value = m_CurVisualValue;
	}

	//getter used by GameManager
	public float GetValue()
	{
		return m_Value;
	}

	//add amount to value if higher than max, make it max
	public void IncreaseValue()
	{
		//spend money to increment
		if(GameManager.Instance.SpendMoney(m_Cost))
		{
			if(m_Value < m_MaxValue)
			{
				m_Value = Mathf.Min(m_Value + m_Increment,m_MaxValue);
			}
		}
	}

	//subtract amount from value if less than 0, make it 0
	public void DecreaseValue(float _amount)
	{
		if(m_Value > m_MinValue)
		{
			m_Value = Mathf.Max(m_Value - _amount,m_MinValue);
		}
	}

	private byte GetNewKey()
	{
		return m_CoroutineKey++;
	}

	private IEnumerator CheckIncrease(byte _key)
	{
		while( m_CoroutineKey == _key)
		{
			IncreaseValue();
			yield return new WaitForSeconds(m_IncrementCheckRate);
		}
	}

	private IEnumerator CheckDecrease()
	{
		while(!GameManager.Instance.IsGameOver())
		{
			DecreaseValue(m_Decrement);
			yield return new WaitForSeconds(m_DecrementCheckRate);
		}
	}

	//gets called when the button is Down
	public void OnBtnDown()
	{
		GetNewKey();
		StartCoroutine(CheckIncrease(m_CoroutineKey));
	}

	//gets called when the button is up
	public void OnBtnUp()
	{
		GetNewKey();
	}
}
