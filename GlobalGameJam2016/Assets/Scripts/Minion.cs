using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class Minion : GGJBase {

	#region MINION_VARIABLES
	public int m_IDx;
	public bool m_Alive=false;

	//Definitive values
	private float m_Money; //profits
	private float m_Faith; //damage to the gods
	private float m_Will; //duration
	private MinionType m_Type;
	private Penalty m_Penalty;

	//Limit values
	private float m_MinFaith;
	private float m_MaxFaith;
	private float m_MinMoney;
	private float m_MaxMoney;
	private float m_MinWill;
	private float m_MaxWill;
	#endregion



	#region MOVEMENT_VARIABLES
	public bool m_Moving=false;
	public RectTransform m_LocationRef;
	public Vector2 m_LocationVector;

	private delegate void MoveCallback();
	private MoveCallback m_MoveCallback;
	
	public RectTransform m_RectT;
	private Vector2 m_MoveDir;
	
	private float m_MinDistanceToReach=4;

	#endregion

	#region TEXT_FILES
	public TextMeshProUGUI m_MoneyText;
	public TextMeshProUGUI m_FaithText;
	public TextMeshProUGUI m_WillText;
	public TextMeshProUGUI m_PenaltyText;
	#endregion

	#region SPAWN_INITIALIZE
	public void Spawn(MinionType _type)
	{
		//Initialize variables
		m_Type = _type;

		m_Penalty = AudienceManager.Instance.GetRandomPenalty ();
		//Get cur limits
		AudienceManager.Instance.GetMoneyLimits (m_Type, out m_MinMoney, out m_MaxMoney);
		AudienceManager.Instance.GetFaithLimits (m_Type, out m_MinFaith, out m_MaxFaith);
		AudienceManager.Instance.GetWillLimits (m_Type, out m_MinWill, out m_MaxWill);

		//Round values to integers
		m_Money = Mathf.Floor (Random.Range (m_MinMoney, m_MaxMoney)+GameManager.Instance.GetMoneyBonus(m_Type));
		m_Faith = Mathf.Floor (Random.Range (m_MinFaith, m_MaxFaith)+GameManager.Instance.GetFaithBonus(m_Type));
		m_Will = Mathf.Floor (Random.Range (m_MinWill, m_MaxWill)+GameManager.Instance.GetWillBonus(m_Type));

		//Assign values to the text TODO: add icons?
		m_MoneyText.text = m_Money.ToString();
		m_FaithText.text = m_Faith.ToString();
		m_WillText.text = m_Will.ToString ();
		m_PenaltyText.text = m_Penalty.ToString (); 

		MoveToFront ();
	}
	#endregion


	#region MOVING_FUNCTIONS


	public void LeaveTheFront()
	{

		//retrieve the minion from the front
		m_LocationVector = new Vector2 (m_LocationVector.x, AudienceManager.Instance.GetInitialYPos ());
		//setup callback
		m_MoveCallback += FrontLeft;

		m_Moving=true;
		//leave the front
		StartCoroutine (MoveToLocation ());
	}

	public void MoveToFront()
	{

		//Move the minion to the front
		m_LocationVector = new Vector2(m_LocationRef.anchoredPosition.x,AudienceManager.Instance.GetRandomHeight());
		///set it to initial position
		m_RectT.anchoredPosition = new Vector2 (m_LocationRef.anchoredPosition.x, AudienceManager.Instance.GetInitialYPos ());
		//setup callback
		m_MoveCallback += FrontReached;
		m_Moving = true;
		//move to the front
		StartCoroutine (MoveToLocation ());
	}
	
	IEnumerator MoveToLocation()
	{
		//while the minion is moving and the game still running
		while(m_Moving&&!GameManager.Instance.IsGameOver())
		{
			//Debug.Log("SIGO: "+m_IDx);
			//wait for a frame
			yield return 0;
			//validate snap distance
			if(Vector2.Distance(m_RectT.anchoredPosition,m_LocationVector)<m_MinDistanceToReach)
			{
					//callback
					m_MoveCallback();
					m_Moving=false;
					//fix position to the final position
					m_RectT.anchoredPosition=m_LocationVector;
			}else
			{
				//get move direction and normalize this direction
				m_MoveDir=m_LocationVector-m_RectT.anchoredPosition;
				m_MoveDir.Normalize();
				//move a step forward
				m_RectT.anchoredPosition+=m_MoveDir*GameManager.Instance.m_MoveSpeed*Time.deltaTime;
			}
		}
	}

	private void FrontReached()
	{
		if (GameManager.Instance.m_MovingFirstMadaFaka) {
			GameManager.Instance.m_MovingFirstMadaFaka=false;
			BossManager.Instance.deactivateSafeZone ();
		}
		m_MoveCallback -= FrontReached;
		//m_Moving = false;
		m_Alive = true;
	}
	
	private void FrontLeft()
	{
		m_MoveCallback -= FrontLeft;
		//m_Moving = false;
		m_Alive = false;
	}
	
	public void Disappear()
	{
		///set it to initial position
		m_RectT.anchoredPosition = new Vector2 (m_LocationRef.anchoredPosition.x, AudienceManager.Instance.GetInitialYPos ());
	}

	#endregion

	#region BUTTON_FUNCTIONS
	//called when the minion is clicked
	public void MinionClicked()
	{
		//ignore the click if moving or the game is over
		if (GameManager.Instance.IsGameOver ()||m_Moving)
			return;
		//try to execute a Ritual
		AudienceManager.Instance.TryToRitual (m_IDx);
	}
	#endregion

	#region PHASING_FUNCTIONS
	//Take the money from this minion
	public float ChargeDonation()
	{
		if (m_Will == 0 || m_Moving || GameManager.Instance.IsGameOver () || !m_Alive)
			return 0;
		return m_Money;
	}

	//Decrease Will in this minion, return true if there is a runner on the front Will=0
	public bool DecreaseWill(float _amount)
	{
		if (m_Moving)
			return false;
		//decrease the Will value by _amount
		m_Will= Mathf.Max (m_Will-_amount,0);
		//update text for this variable
		m_WillText.text = m_Will.ToString();
		//if there is no more will then this minion will leave the front
		return m_Will == 0;
	}

	public float GetFaith()
	{
		//return the damage that will be caused to God
		return m_Faith;
	}

	public float GetWill()
	{
		//return the damage that will be caused to God
		return m_Will;
	}

	public Penalty GetPenalty()
	{
		return m_Penalty;
	}
	#endregion

}