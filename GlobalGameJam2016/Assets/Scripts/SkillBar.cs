using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SkillBar : MonoBehaviour {

	[SerializeField]
	private Image m_Slider;

	//ID used to call the correct music
	[SerializeField]
	private AudioManager.SfxLoop m_SliderEnum;

	//max value of bar
	[SerializeField]
	private float m_MaxValue;

	//min value of bar
	[SerializeField]
	private float m_MinValue=1;

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
		m_Slider.fillAmount = m_CurVisualValue;
	}

	//getter used by GameManager
	public float GetValue()
	{
		return Mathf.Lerp (m_MinValue, m_MaxValue, m_Value);
	}

	//add amount to value if higher than max, make it max
	public void IncreaseValue()
	{
		//spend money to increment
		if(m_Value < 1)
		{
			if(GameManager.Instance.SpendMoney(m_Cost))
			{
				AudioManager.Instance.PlaySfxLoop(m_SliderEnum);
				m_Value = Mathf.Min(m_Value + m_Increment,1);
			}
			else
			{
				AudioManager.Instance.StopSfxLoop(m_SliderEnum);
			}
		}
	}

	//subtract amount from value if less than 0, make it 0
	public void DecreaseValue(float _amount)
	{
		if(m_Value > 0)
		{
			m_Value = Mathf.Max(m_Value - _amount,0);
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
		if(GameManager.Instance.IsGameOver())
			return;
		GetNewKey();
		StartCoroutine(CheckIncrease(m_CoroutineKey));
	}

	//gets called when the button is up
	public void OnBtnUp()
	{
		if(GameManager.Instance.IsGameOver())
			return;
		AudioManager.Instance.StopSfxLoop(m_SliderEnum);
		GetNewKey();
	}
}
